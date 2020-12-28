using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DataBrowser.Interfaces;
using EndPointConnector.Interfaces.Sdmx;
using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.ParserSdmx;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;

namespace DataBrowser.INF.SdmxCache
{
    public class SdmxCacheEngine : ISdmxCache
    {
        private readonly ILogger<SdmxCacheEngine> _logger;
        private readonly IRequestContext _requestContext;
        private readonly SdmxCacheConfig _sdmxCacheConfig;
        private readonly SdmxParser _sdmxParser;

        public SdmxCacheEngine(IRequestContext requestContext, IOptionsSnapshot<SdmxCacheConfig> sdmxCacheConfig,
            ILogger<SdmxCacheEngine> logger, ILoggerFactory loggerFactory)
        {
            _requestContext = requestContext;
            _sdmxParser = new SdmxParser(loggerFactory);
            _sdmxCacheConfig = sdmxCacheConfig.Value;
            _logger = logger;
        }

        public int ExpiredTime => _sdmxCacheConfig.ExpiredTime;
        public bool DisableSdmxCache => _sdmxCacheConfig.DisableSdmxCache;
        public bool DisableGlobalCache => _sdmxCacheConfig.DisableGlobalCache;
        public bool DisableNamespace => _sdmxCacheConfig.DisableNamespace;

        public async Task<ISdmxObjects> GetSdmxObjectsAsync(string key)
        {
            if (_sdmxCacheConfig.DisableSdmxCache || _requestContext != null && _requestContext.IgnoreCache)
            {
                _logger.LogDebug("Cache Sdmx disable");
                return null;
            }

            var result = await getAsync(key, "SdmxObjects");
            if (result != null) return _sdmxParser.GetSdmxObjectsFromSdmxXml(result);
            return null;
        }

        public async Task<XmlDocument> GetSdmxXmlAsync(string key)
        {
            if (_sdmxCacheConfig.DisableSdmxCache || _requestContext != null && _requestContext.IgnoreCache)
            {
                _logger.LogDebug("Cache Sdmx disable");
                return null;
            }

            var xmlDocument = new XmlDocument();
            var result = await getAsync(key, "SdmxObjects");
            if (result != null)
            {
                xmlDocument.LoadXml(result);
                return xmlDocument;
            }

            return null;
        }

        public async Task SetSdmxObjectsAsync(string key, ISdmxObjects sdmxObjects)
        {
            if (_sdmxCacheConfig.DisableSdmxCache || _requestContext != null && _requestContext.IgnoreCache)
            {
                _logger.LogDebug("Cache Sdmx disable");
                return;
            }

            await setAsync(key, _sdmxParser.GetSdmxXmlFromSdmxObjects(sdmxObjects), "SdmxObjects");
        }

        public async Task SetSdmxXmlAsync(string key, XmlDocument sdmxXml)
        {
            if (_sdmxCacheConfig.DisableSdmxCache || _requestContext != null && _requestContext.IgnoreCache)
            {
                _logger.LogDebug("Cache Sdmx disable");
                return;
            }

            using (var stringWriter = new StringWriterUTF8())
            {
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    sdmxXml.WriteTo(xmlTextWriter);
                    await xmlTextWriter.FlushAsync();
                    await setAsync(key, stringWriter.GetStringBuilder().ToString(), "SdmxObjects");
                }
            }
        }

        public async Task<bool> InvalidSdmxDataAsync(bool onlyCurrentNode = false, bool onlyCurrentUser = false)
        {
            if (_sdmxCacheConfig.DisableSdmxCache || _requestContext != null && _requestContext.IgnoreCache)
            {
                _logger.LogDebug("Cache Sdmx disable");
                return false;
            }

            if (onlyCurrentUser && string.IsNullOrWhiteSpace(_requestContext.UserGuid))
            {
                _logger.LogDebug("Invalid cache UserGuid is null");
                return false;
            }

            if (onlyCurrentNode && string.IsNullOrWhiteSpace(_requestContext.NodeCode))
            {
                _logger.LogDebug("Invalid cache NodeCode is null");
                return false;
            }

            using (var conn = createOpenedConnection())
            {
                var whereStr = "";
                if (onlyCurrentUser) whereStr = "WHERE [UserGuid]=@UserGuid";
                if (onlyCurrentNode) whereStr = onlyCurrentUser ? " AND [Domain]=@Domain" : "WHERE [Domain]=@Domain";

                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText = $@"DELETE FROM [SdmxObjects] {whereStr}"
                })
                {
                    scdCommand.Parameters.Add(new SqliteParameter("@UserGuid", DbType.String)
                        {Value = _requestContext.UserGuid});
                    scdCommand.Parameters.Add(new SqliteParameter("@Domain", DbType.String)
                        {Value = _requestContext.NodeCode});
                    await scdCommand.ExecuteNonQueryAsync();
                }

                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText = $@"DELETE FROM [SdmxJson] {whereStr}"
                })
                {
                    scdCommand.Parameters.Add(new SqliteParameter("@UserGuid", DbType.String)
                        {Value = _requestContext.UserGuid});
                    scdCommand.Parameters.Add(new SqliteParameter("@Domain", DbType.String)
                        {Value = _requestContext.NodeCode});
                    await scdCommand.ExecuteNonQueryAsync();
                }
            }

            return true;
        }


        public async Task<string> GetGlobalGenericAsync(string key, bool crossDomain)
        {
            if (_sdmxCacheConfig.DisableGlobalCache)
            {
                _logger.LogDebug("Cache Global disable");
                return null;
            }

            if (!crossDomain && string.IsNullOrEmpty(_requestContext.NodeCode))
            {
                _logger.LogDebug("Invalid cache Domain is null");
                return null;
            }

            var domain = "domain@";
            if (!crossDomain) domain += _requestContext.NodeCode;
            using (var conn = createOpenedConnection())
            {
                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText =
                        @"SELECT CacheValue FROM [SdmxGeneric] WHERE [Key]=@Key AND [Domain]=@Domain AND [Expired]>@CurrentTime"
                })
                {
                    scdCommand.Parameters.Add(new SqliteParameter("@Key", DbType.String) {Value = key});
                    scdCommand.Parameters.Add(new SqliteParameter("@Domain", DbType.String) {Value = domain});
                    scdCommand.Parameters.Add(new SqliteParameter("@CurrentTime", DbType.Int32)
                        {Value = DateTime.UtcNow});
                    var objValue = await scdCommand.ExecuteScalarAsync();
                    if (objValue != null)
                    {
                        var result = (string) objValue;
                        if (_logger.IsEnabled(LogLevel.Trace))
                        {
                            _logger.LogTrace("Result Cache");
                            _logger.LogTrace(result);
                        }

                        return result;
                    }
                }

                _logger.LogDebug($"Key [{key}] not found");
                return null;
            }
        }

        public async Task<bool> SetGlobalGenericAsync(string key, string value, bool crossDomain)
        {
            if (_sdmxCacheConfig.DisableGlobalCache)
            {
                _logger.LogDebug("Cache Global disable");
                return false;
            }

            if (!crossDomain && string.IsNullOrEmpty(_requestContext.NodeCode))
            {
                _logger.LogDebug("Invalid cache Domain is null");
                return false;
            }

            var domain = "domain@";
            if (!crossDomain) domain += _requestContext.NodeCode;

            using (var conn = createOpenedConnection())
            {
                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText = @"DELETE FROM [SdmxGeneric] WHERE [Key]=@Key AND [Domain]=@Domain"
                })
                {
                    scdCommand.Parameters.Add(new SqliteParameter("@Key", DbType.String) {Value = key});
                    scdCommand.Parameters.Add(new SqliteParameter("@Domain", DbType.String) {Value = domain});
                    await scdCommand.ExecuteNonQueryAsync();
                }

                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText =
                        @"INSERT INTO [SdmxGeneric] ([Key], [CacheValue], [Expired], [Domain]) VALUES (@Key, @CacheValue, @Expired, @Domain)"
                })
                {
                    scdCommand.Parameters.Add(new SqliteParameter("@Key", DbType.String) {Value = key});
                    scdCommand.Parameters.Add(new SqliteParameter("@CacheValue", DbType.String) {Value = value});
                    scdCommand.Parameters.Add(new SqliteParameter("@Domain", DbType.String) {Value = domain});
                    scdCommand.Parameters.Add(new SqliteParameter("@Expired", DbType.Int32)
                        {Value = DateTime.UtcNow.AddSeconds(_sdmxCacheConfig.ExpiredTime)});
                    await scdCommand.ExecuteNonQueryAsync();
                }
            }

            return true;
        }

        public async Task<bool> InvalidGlobalGenericAsync(bool crossDomain = false)
        {
            if (_sdmxCacheConfig.DisableGlobalCache)
            {
                _logger.LogDebug("Cache Global disable");
                return false;
            }

            _logger.LogDebug("Start");
            if (!crossDomain && string.IsNullOrEmpty(_requestContext.NodeCode))
            {
                _logger.LogDebug("Invalid cache Domain is null");
                return false;
            }

            var domain = "domain@";
            if (crossDomain) domain += _requestContext.NodeCode;

            using (var conn = createOpenedConnection())
            {
                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText = @"DELETE FROM [SdmxGeneric] WHERE [Domain]=@Domain"
                })
                {
                    scdCommand.Parameters.Add(new SqliteParameter("@Domain", DbType.String) {Value = domain});
                    await scdCommand.ExecuteNonQueryAsync();
                }
            }

            _logger.LogDebug("End");
            return true;
        }

        public string CreateKey(params string[] elements)
        {
            if (elements == null || elements.Length <= 0) return null;

            return elements.Aggregate("",
                (current, next) => current + ":" + next);
        }

        public async Task SetJsonAsync(string key, object obj)
        {
            if (_sdmxCacheConfig.DisableSdmxCache || _requestContext != null && _requestContext.IgnoreCache)
            {
                _logger.LogDebug("Cache Sdmx disable");
                return;
            }

            await setAsync(key, JsonConvert.SerializeObject(obj), "SdmxJson");
        }

        public async Task<T> GetJsonAsync<T>(string key)
        {
            if (_sdmxCacheConfig.DisableSdmxCache || _requestContext != null && _requestContext.IgnoreCache)
            {
                _logger.LogDebug("Cache Sdmx disable");
                return default;
            }

            var str = await getAsync(key, "SdmxJson");
            if (str != null) return JsonConvert.DeserializeObject<T>(str);
            return default;
        }

        public async Task ClearCacheAsync(List<string> nodesCode)
        {
            _logger.LogDebug("START ClearCacheAsync");
            if (nodesCode == null && nodesCode.Count == 0) return;

            using (var conn = createOpenedConnection())
            {
                foreach (var nodeCode in nodesCode)
                {
                    using (var clearCacheCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"DELETE FROM [SdmxObjects] WHERE Domain=@Domain"
                    })
                    {
                        clearCacheCommand.Parameters.Add(new SqliteParameter("@Domain", DbType.String)
                            {Value = nodeCode});
                        await clearCacheCommand.ExecuteNonQueryAsync();
                    }

                    using (var clearCacheCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"DELETE FROM [SdmxGeneric] WHERE Domain=@Domain"
                    })
                    {
                        clearCacheCommand.Parameters.Add(new SqliteParameter("@Domain", DbType.String)
                            {Value = nodeCode});
                        await clearCacheCommand.ExecuteNonQueryAsync();
                    }

                    using (var clearCacheCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"DELETE FROM [SdmxJson] WHERE Domain=@Domain"
                    })
                    {
                        clearCacheCommand.Parameters.Add(new SqliteParameter("@Domain", DbType.String)
                            {Value = nodeCode});
                        await clearCacheCommand.ExecuteNonQueryAsync();
                    }
                }
            }

            _logger.LogDebug("END ClearCacheAsync");
        }

        private async Task<string> getAsync(string key, string tableName)
        {
            if (string.IsNullOrWhiteSpace(_requestContext.UserGuid))
            {
                _logger.LogDebug("Invalid cache UserGuid is null");
                return null;
            }

            if (string.IsNullOrWhiteSpace(_requestContext.NodeCode))
            {
                _logger.LogDebug("Invalid cache NodeCode is null");
                return null;
            }

            using (var conn = createOpenedConnection())
            {
                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText =
                        $@"SELECT CacheValue FROM [{tableName}] WHERE [Key]=@Key AND [UserGuid]=@UserGuid AND [Domain]=@Domain AND [Expired]>@CurrentTime"
                })
                {
                    scdCommand.Parameters.Add(new SqliteParameter("@Key", DbType.String) {Value = key});
                    scdCommand.Parameters.Add(new SqliteParameter("@UserGuid", DbType.String)
                        {Value = _requestContext.UserGuid});
                    scdCommand.Parameters.Add(new SqliteParameter("@Domain", DbType.String)
                        {Value = _requestContext.NodeCode});
                    scdCommand.Parameters.Add(new SqliteParameter("@CurrentTime", DbType.Int32)
                        {Value = DateTime.UtcNow});
                    var objValue = await scdCommand.ExecuteScalarAsync();
                    if (objValue != null)
                    {
                        var result = (string) objValue;
                        if (_logger.IsEnabled(LogLevel.Trace))
                        {
                            _logger.LogTrace("Result Cache");
                            _logger.LogTrace(result);
                        }

                        return result;
                    }
                }

                _logger.LogDebug($"Key [{key}] not found");
                return null;
            }
        }

        private async Task setAsync(string key, string strValue, string tableName)
        {
            if (string.IsNullOrWhiteSpace(_requestContext.UserGuid) ||
                string.IsNullOrWhiteSpace(_requestContext.NodeCode)) return;

            using (var conn = createOpenedConnection())
            {
                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText =
                        $@"DELETE FROM [{tableName}] WHERE [Key]=@Key AND [Domain]=@Domain AND [UserGuid]=@UserGuid"
                })
                {
                    scdCommand.Parameters.Add(new SqliteParameter("@Key", DbType.String) {Value = key});
                    scdCommand.Parameters.Add(new SqliteParameter("@UserGuid", DbType.String)
                        {Value = _requestContext.UserGuid});
                    scdCommand.Parameters.Add(new SqliteParameter("@Domain", DbType.String)
                        {Value = _requestContext.NodeCode});
                    await scdCommand.ExecuteNonQueryAsync();
                }

                using (var sqliteCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText =
                        $@"INSERT INTO [{tableName}] ([Key], [Domain], [UserGuid], [CacheValue], [Expired]) VALUES (@Key, @Domain, @UserGuid, @CacheValue, @Expired)"
                })
                {
                    sqliteCommand.Parameters.Add(new SqliteParameter("@Key", DbType.String) {Value = key});
                    sqliteCommand.Parameters.Add(new SqliteParameter("@UserGuid", DbType.String)
                        {Value = _requestContext.UserGuid});
                    sqliteCommand.Parameters.Add(new SqliteParameter("@Domain", DbType.String)
                        {Value = _requestContext.NodeCode});
                    sqliteCommand.Parameters.Add(new SqliteParameter("@CacheValue", DbType.String) {Value = strValue});
                    sqliteCommand.Parameters.Add(new SqliteParameter("@Expired", DbType.Int32)
                        {Value = DateTime.UtcNow.AddSeconds(_sdmxCacheConfig.ExpiredTime)});
                    await sqliteCommand.ExecuteNonQueryAsync();
                }
            }
        }

        private SqliteConnection createOpenedConnection()
        {
            var connString = _sdmxCacheConfig.ConnectionString;
            var result = File.Exists(connString);
            var conn = new SqliteConnection(connString);
            conn.Open();
            if (!result) InizializzateNoSqlDb(conn);

            return conn;
        }

        private void InizializzateNoSqlDb(SqliteConnection conn)
        {
            _logger.LogDebug("Start");
            using (var scdCommand = new SqliteCommand
            {
                Connection = conn,
                CommandText = @"SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'SdmxObjects';"
            })
            {
                var exist = (long) scdCommand.ExecuteScalar() > 0;

                if (!exist)
                {
                    using (var scdCommandCreate = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"CREATE TABLE IF NOT EXISTS
                                            [SdmxObjects](
                                            [Id]     INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                            [Key]   NVARCHAR NULL,
                                            [UserGuid]   NVARCHAR NULL,
                                            [Domain]   NVARCHAR NULL,
                                            [CacheValue]   NVARCHAR NULL,
                                            [Expired] INTEGER NOT NULL
                                        )"
                    })
                    {
                        scdCommandCreate.ExecuteNonQuery();
                    }

                    using (var scdCommandCreate = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"CREATE TABLE IF NOT EXISTS
                                            [SdmxGeneric](
                                            [Id]     INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                            [Key]   NVARCHAR NULL,
                                            [Domain]   NVARCHAR NULL,
                                            [CacheValue]   NVARCHAR NULL,
                                            [Expired] INTEGER NOT NULL
                                        )"
                    })
                    {
                        scdCommandCreate.ExecuteNonQuery();
                    }

                    using (var scdCommandCreate = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"CREATE TABLE IF NOT EXISTS
                                            [SdmxJson](
                                            [Id]     INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                            [Key]   NVARCHAR NULL,
                                            [UserGuid]   NVARCHAR NULL,
                                            [Domain]   NVARCHAR NULL,
                                            [CacheValue]   NVARCHAR NULL,
                                            [Expired] INTEGER NOT NULL
                                        )"
                    })
                    {
                        scdCommandCreate.ExecuteNonQuery();
                    }
                }
            }

            _logger.LogDebug("End");
        }

        public class StringWriterUTF8 : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}
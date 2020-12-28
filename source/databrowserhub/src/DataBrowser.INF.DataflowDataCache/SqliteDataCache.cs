using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Dto;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using EndPointConnector.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataBrowser.INF.DataflowDataCache
{
    public class SqliteDataCache : IDataflowDataCache
    {
        private readonly DataflowDataCacheConfig _dataCacheConfig;
        private readonly IRepository<Hub> _hubRepository;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<SqliteDataCache> _logger;
        private readonly IRepository<Node> _nodeRepository;
        private readonly IRequestContext _requestContext;

        public SqliteDataCache(ILogger<SqliteDataCache> logger,
            IOptionsSnapshot<DataflowDataCacheConfig> dataCacheConfig,
            IRequestContext requestContext,
            IRepository<Node> nodeRepository,
            IRepository<Hub> hubRepository,
            IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _dataCacheConfig = dataCacheConfig.Value;
            _requestContext = requestContext;
            _nodeRepository = nodeRepository;
            _hubRepository = hubRepository;
            _hostEnvironment = hostEnvironment;
        }

        public async Task SetJsonStatForDataflowData(DataFromDataflowRequest dataFromDataflowRequest,
            GetDataFromDataflowResponse cacheValue, Dataset dataset)
        {
            if (!checkCacheEnableForCurrentData(dataFromDataflowRequest.DataflowId,
                _requestContext?.IgnoreCache, _dataCacheConfig?.IsEnable, _dataCacheConfig?.ExclusionList))
            {
                return;
            }

            InizializeDbIfNotExist();

            _logger.LogDebug(
                $"DataflowData SetJsonStatForDataflowData: {_requestContext.NodeId}\t{_requestContext.UserLang}\t{dataFromDataflowRequest.DataflowId}");

            _logger.LogDebug(
                $"DataflowData clear cache data: {_requestContext.NodeId}\t{_requestContext.UserLang}\t{dataFromDataflowRequest.DataflowId}");
            //await clearCacheAsync();

            _logger.LogDebug(
                $"DataflowData create cache key: {_requestContext.NodeId}\t{_requestContext.UserLang}\t{dataFromDataflowRequest.DataflowId}");

            using (var conn = new SqliteConnection(GetConnectionStringBuilder().ConnectionString))
            {
                conn.Open();

                string cacheInfoId = null;
                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText =
                        @"SELECT Id FROM [CacheInfo] WHERE [NodeId]=@NodeId AND [DataflowId]=@DataflowId LIMIT 1"
                })
                {
                    scdCommand.Parameters.Add(new SqliteParameter("@NodeId", DbType.Int32)
                    { Value = _requestContext.NodeId });
                    scdCommand.Parameters.Add(new SqliteParameter("@DataflowId", DbType.String)
                    { Value = dataFromDataflowRequest.DataflowId });
                    var cacheInfoIdObj = await scdCommand.ExecuteScalarAsync();
                    if (cacheInfoIdObj != null)
                    {
                        cacheInfoId = (string)cacheInfoIdObj;
                    }
                }

                if (string.IsNullOrWhiteSpace(cacheInfoId))
                {
                    var ttl = _dataCacheConfig.Expiration;
                    var node = await _nodeRepository.GetByIdAsync(_requestContext.NodeId);
                    if (node?.TtlDataflow != null)
                    {
                        ttl = node.TtlDataflow.Value;
                    }

                    using (var sqliteCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText =
                            @"INSERT INTO [CacheInfo] ([Id], [NodeId], [DataflowId], [TTL]) VALUES (@Id, @NodeId, @DataflowId, @TTL)"
                    })
                    {
                        cacheInfoId = Guid.NewGuid().ToString();
                        sqliteCommand.Parameters.Add(new SqliteParameter("@Id", DbType.String) { Value = cacheInfoId });
                        sqliteCommand.Parameters.Add(new SqliteParameter("@NodeId", DbType.String)
                        { Value = _requestContext.NodeId });
                        sqliteCommand.Parameters.Add(new SqliteParameter("@DataflowId", DbType.String)
                        { Value = dataFromDataflowRequest.DataflowId });
                        sqliteCommand.Parameters.Add(new SqliteParameter("@TTL", DbType.String) { Value = ttl });
                        await sqliteCommand.ExecuteNonQueryAsync();
                    }
                }

                var filterStr = dataFromDataflowRequest != null
                    ? JsonConvert.SerializeObject(dataFromDataflowRequest.DataCriterias)
                    : "[]";

                var dataType = DataType.AllData;
                if (cacheValue.ItemsCount == 0 &&
                    string.IsNullOrWhiteSpace(cacheValue.JsonData)
                )
                {
                    dataType = DataType.NoData;
                }
                else if (cacheValue.ItemsCount > 0)
                {
                    if (cacheValue.ItemsFrom > -1 &&
                        cacheValue.ItemsTo > -1)
                    {
                        if (cacheValue.ItemsFrom > 0 || cacheValue.ItemsTo < cacheValue.ItemsCount - 1)
                        {
                            dataType = DataType.PartialData;
                        }
                    }
                }

                var accessNumber = 1L;
                var cacheFileId = Guid.NewGuid();
                var cacheInfoData = await GetOnlyCachedKeyInfoDataflowData(dataFromDataflowRequest.DataflowId,
                    dataFromDataflowRequest.DataCriterias, true, dataset);
                if (cacheInfoData != null)
                {
                    try
                    {
                        using (var scdCommand = new SqliteCommand
                        {
                            Connection = conn,
                            CommandText = @"SELECT DataCache FROM [CacheFiles] WHERE Id=@Id"
                        })
                        {
                            scdCommand.Parameters.Add(new SqliteParameter("@Id", DbType.String)
                            { Value = cacheInfoData.CacheFileId.ToString() });
                            var filename = (await scdCommand.ExecuteScalarAsync());
                            if (filename !=  DBNull.Value &&
                                !string.IsNullOrWhiteSpace((string)filename))
                            {
                                var filePath = getPathCacheFileWithLang(_requestContext.NodeId, _requestContext.UserLang) + $"/{filename}";
                                _logger.LogDebug($"Delete old file {filePath}");
                                File.Delete(filePath);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Some error during remove old file cache: {ex.Message}", ex);
                    }

                    using (var scdCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"DELETE FROM [CacheFiles] WHERE Id=@Id"
                    })
                    {
                        scdCommand.Parameters.Add(new SqliteParameter("@Id", DbType.String)
                        { Value = cacheInfoData.CacheFileId.ToString() });
                        await scdCommand.ExecuteNonQueryAsync();
                    }

                    accessNumber = cacheInfoData.Accesses;
                    cacheFileId = cacheInfoData.CacheFileId;
                }


                using (var sqliteCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText =
                        @"INSERT INTO [CacheFiles] ([Id], [CacheInfoId], [Filter], [Language], [Annotations], [DataType], [CreationDate], [FileSize], [Accesses], [DataCache]) VALUES (@Id, @CacheInfoId, @Filter, @Language, @Annotations, @DataType, @CreationDate, @FileSize, @Accesses, @DataCache)"
                })
                {
                    var cacheValueStr = saveFileCache(cacheValue);
                    sqliteCommand.Parameters.Add(new SqliteParameter("@Id", DbType.String)
                    { Value = cacheFileId.ToString() });
                    sqliteCommand.Parameters.Add(new SqliteParameter("@CacheInfoId", DbType.String)
                    { Value = cacheInfoId });
                    sqliteCommand.Parameters.Add(new SqliteParameter("@Filter", DbType.String) { Value = filterStr });
                    sqliteCommand.Parameters.Add(new SqliteParameter("@Language", DbType.String)
                    { Value = _requestContext.UserLang });
                    sqliteCommand.Parameters.Add(new SqliteParameter("@Annotations", DbType.String)
                    { Value = createAnnotationString(dataset) });
                    sqliteCommand.Parameters.Add(new SqliteParameter("@DataType", DbType.Int32) { Value = dataType });
                    sqliteCommand.Parameters.Add(new SqliteParameter("@CreationDate", DbType.String)
                    { Value = DateTime.UtcNow });
                    sqliteCommand.Parameters.Add(new SqliteParameter("@FileSize", DbType.Int32)
                    { Value = cacheValueStr.Item1 });
                    sqliteCommand.Parameters.Add(new SqliteParameter("@Accesses", DbType.Int64) { Value = accessNumber });
                    sqliteCommand.Parameters.Add(new SqliteParameter("@DataCache", DbType.String)
                    { Value = cacheValueStr.Item2 });
                    await sqliteCommand.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<GetDataFromDataflowResponse> GetJsonStatForDataflowDataFromValidKey(
            DataFromDataflowRequest dataFromDataflowRequest, Dataset dataset)
        {
            try
            {
                if (!checkCacheEnableForCurrentData(dataFromDataflowRequest.DataflowId,
                    _requestContext?.IgnoreCache, _dataCacheConfig?.IsEnable, _dataCacheConfig?.ExclusionList))
                {
                    return null;
                }

                InizializeDbIfNotExist();

                _logger.LogDebug(
                    $"DataflowData find cache key: {_requestContext.NodeId}\t{_requestContext.UserLang}\t{dataFromDataflowRequest.DataflowId}");

                var cacheInfo = await GetOnlyCachedKeyInfoDataflowDataIfIsValid(dataFromDataflowRequest.DataflowId,
                    dataFromDataflowRequest.DataCriterias, dataset);

                if (cacheInfo == null)
                {
                    _logger.LogDebug(
                        $"Not found valid cache key: {_requestContext.NodeId}\t{_requestContext.UserLang}\t{dataFromDataflowRequest.DataflowId}");
                    return null;
                }

                using (var conn = new SqliteConnection(GetConnectionStringBuilder().ConnectionString))
                {
                    conn.Open();

                    GetDataFromDataflowResponse objResult = null;

                    using (var scdCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"SELECT DataCache 
                                    FROM [CacheFiles] 
                                    WHERE [Id]=@Id"
                    })
                    {
                        scdCommand.Parameters.Add(new SqliteParameter("@Id", DbType.String)
                        { Value = cacheInfo.CacheFileId.ToString() });
                        var objValueObj = await scdCommand.ExecuteScalarAsync();
                        if (objValueObj != null && objValueObj != DBNull.Value)
                        {
                            var resultStr = (string)objValueObj;
                            if (string.IsNullOrWhiteSpace(resultStr))
                            {
                                _logger.LogDebug("Null value in Cache Id");
                                return null;
                            }

                            objResult = getFileCache(resultStr);
                        }
                        else
                        {
                            _logger.LogDebug("Cache Id not found");
                            return null;
                        }
                    }

                    using (var scdCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"UPDATE [CacheFiles] SET Accesses=Accesses+1 WHERE Id=@Id"
                    })
                    {
                        scdCommand.Parameters.Add(new SqliteParameter("@Id", DbType.String)
                        { Value = cacheInfo.CacheFileId.ToString() });
                        await scdCommand.ExecuteNonQueryAsync();
                    }

                    return objResult;
                }
            }
            catch (SqliteException ex)
            {
                if (ex.SqliteErrorCode == 11)
                {
                    DropDbIfExist(_dataCacheConfig);
                }

                return null;
            }
        }

        public async Task<DataflowDataCacheFile> GetOnlyCachedKeyInfoDataflowData(string dataflowId,
            List<FilterCriteria> dataCriterias, bool includeExpired, Dataset dataset)
        {
            try
            {
                _logger.LogDebug(
                    $"GetOnlyCachedInfoDataflowData clear cache data: {_requestContext.NodeId}\t{_requestContext.UserLang}\t{dataflowId}");

                if (!checkCacheEnableForCurrentData(dataflowId, _requestContext?.IgnoreCache,
                    _dataCacheConfig?.IsEnable, _dataCacheConfig?.ExclusionList))
                {
                    return null;
                }

                InizializeDbIfNotExist();

                _logger.LogDebug(
                    $"DataflowData OnlyData find cache key: {_requestContext.NodeId}\t{_requestContext.UserLang}\t{dataflowId}");

                using (var conn = new SqliteConnection(GetConnectionStringBuilder().ConnectionString))
                {
                    conn.Open();
                    using (var scdCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText =
                            @"SELECT cf.Id, cf.CacheInfoId, cf.Filter, cf.Language, cf.Annotations, cf.DataType, cf.FileSize, cf.Accesses, cf.CreationDate, ci.TTL
                                            FROM [CacheFiles] cf INNER JOIN [CacheInfo] ci ON cf.CacheInfoId=ci.Id 
                                            WHERE ci.[DataflowId]=@DataflowId AND ci.[NodeId]=@NodeId AND Language=@Language 
                                                    AND Filter=@Filter AND Annotations=@Annotations"
                    })
                    {
                        scdCommand.Parameters.Add(
                            new SqliteParameter("@DataflowId", DbType.String) { Value = dataflowId });
                        scdCommand.Parameters.Add(new SqliteParameter("@NodeId", DbType.String)
                        { Value = _requestContext.NodeId });
                        scdCommand.Parameters.Add(new SqliteParameter("@Language", DbType.String)
                        { Value = _requestContext.UserLang });
                        scdCommand.Parameters.Add(new SqliteParameter("@Annotations", DbType.String)
                        { Value = createAnnotationString(dataset) });
                        scdCommand.Parameters.Add(new SqliteParameter("@Filter", DbType.String)
                        {
                            Value = dataCriterias != null ? JsonConvert.SerializeObject(dataCriterias) : "[]"
                        });
                        var reader = await scdCommand.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            var creationDate = Convert.ToDateTime(reader["CreationDate"]);
                            var expireDelta = DateTime.UtcNow.AddSeconds(-(long)reader["TTL"]);
                            if (!includeExpired && expireDelta > creationDate)
                            {
                                _logger.LogDebug("Cache key expired");
                                continue;
                            }

                            _logger.LogDebug("Cache key found");
                            var result = new DataflowDataCacheFile
                            {
                                CacheFileId = Guid.Parse((string)reader["Id"]),
                                CacheInfoId = Guid.Parse((string)reader["CacheInfoId"]),
                                Filter = JsonConvert.DeserializeObject<List<FilterCriteria>>((string)reader["Filter"]),
                                Language = (string)reader["Language"],
                                Annotations = (string)reader["Annotations"],
                                DataType = (DataType)(long)reader["DataType"],
                                CreationDate = creationDate,
                                FileSize = (int)(long)reader["FileSize"],
                                Accesses = (int)(long)reader["Accesses"]
                            };

                            return result;
                        }

                        _logger.LogDebug("Cache key not found");
                        return null;
                    }
                }
            }
            catch (SqliteException ex)
            {
                if (ex.SqliteErrorCode == 11)
                {
                    DropDbIfExist(_dataCacheConfig);
                }

                return null;
            }
        }

        public async Task<DataflowDataCacheFile> GetOnlyCachedKeyInfoDataflowDataIfIsValid(string dataflowId,
            List<FilterCriteria> dataCriterias, Dataset dataset)
        {
            try
            {
                var cacheInfo =
                    await GetOnlyCachedKeyInfoDataflowData(dataflowId, dataCriterias, false, dataset);

                if (cacheInfo == null)
                {
                    return null;
                }

                var cacheIsValid = dataCacheIsStillValid(dataset, cacheInfo);

                return cacheIsValid ? cacheInfo : null;
            }
            catch (SqliteException ex)
            {
                if (ex.SqliteErrorCode == 11)
                {
                    DropDbIfExist(_dataCacheConfig);
                }

                return null;
            }
        }

        private void InizializeDbIfNotExist(int? nodeId = null)
        {
            var stringBuilder = GetConnectionStringBuilder(nodeId);
            var result = File.Exists(stringBuilder.DataSource);
            if (!result)
            {
                using (var conn = new SqliteConnection(stringBuilder.ConnectionString))
                {
                    conn.Open();
                    var exist = false;
                    using (var scdCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'CacheInfo';"
                    })
                    {
                        exist = (long)scdCommand.ExecuteScalar() > 0;
                    }

                    if (!exist)
                    {
                        using (var scdCommandCreate = new SqliteCommand
                        {
                            Connection = conn,
                            CommandText = @"CREATE TABLE CacheInfo (
                            Id	TEXT,
                            NodeId    INTEGER,
                            DataflowId    TEXT,
                            TTL   INTEGER,
                            PRIMARY KEY(Id));"
                        })
                        {
                            scdCommandCreate.ExecuteNonQuery();
                        }
                    }

                    exist = false;
                    using (var scdCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText =
                            @"SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'CacheFiles';"
                    })
                    {
                        exist = (long)scdCommand.ExecuteScalar() > 0;
                    }

                    if (!exist)
                    {
                        using (var scdCommandCreate = new SqliteCommand
                        {
                            Connection = conn,
                            CommandText = @"CREATE TABLE CacheFiles (
                            Id    TEXT,
                            CacheInfoId   TEXT,
                            Filter    TEXT,
                            Language    TEXT,
                            Annotations    TEXT,
                            DataType   INTEGER,
                            Path  TEXT,
                            CreationDate  INTEGER,
                            FileSize  INTEGER,
                            Accesses  INTEGER,
                            DataCache TEXT,
                            PRIMARY KEY(Id));"
                        })
                        {
                            scdCommandCreate.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public async Task ClearCacheDataflowDataAsync(int nodeId)
        {
            if (_dataCacheConfig == null || !_dataCacheConfig.IsEnable)
            {
                return;
            }

            InizializeDbIfNotExist(nodeId);

            using (var conn = new SqliteConnection(GetConnectionStringBuilder(nodeId).ConnectionString))
            {
                conn.Open();
                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn
                })
                {
                    scdCommand.CommandText = $@"UPDATE [CacheFiles] SET DataCache='', FileSize=0  WHERE CacheInfoId IN (SELECT Id FROM CacheInfo WHERE NodeId=@NodeId)";
                    scdCommand.Parameters.Add(new SqliteParameter($"@NodeId", DbType.Int64) { Value = nodeId });
                    await scdCommand.ExecuteNonQueryAsync();
                }
            }

            clearFilesCache(nodeId);
        }

        private void clearFilesCache(int nodeId)
        {
            if (!_dataCacheConfig.SaveDataOnFile)
            {
                return;
            }

            var path = getPathCacheFile(nodeId);

            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during clear all files cache");
            }
        }

        public async Task<List<DataflowDataCacheInfo>> GetInfoFromNodeId(int nodeId)
        {
            _logger.LogDebug($"GetCacheInfoFromNodeId START with NodeId {nodeId}");

            InizializeDbIfNotExist();

            var cacheInfos = new List<DataflowDataCacheInfo>();

            using (var conn = new SqliteConnection(GetConnectionStringBuilder().ConnectionString))
            {
                conn.Open();

                var ttl = _dataCacheConfig.Expiration;
                var node = await _nodeRepository.GetByIdAsync(_requestContext.NodeId);
                if (node?.TtlDataflow != null)
                {
                    ttl = node.TtlDataflow.Value;
                }

                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText =
                        @"SELECT ci.*, SUM(FileSize) AS CacheSize, SUM(IsActive) CachedDataflow, SUM(Accesses) CachedDataAccess
                                    FROM [CacheInfo] ci
                                    LEFT JOIN (SELECT CacheInfoId, FileSize, Accesses, CASE FileSize WHEN 0 THEN 0 ELSE 1 END AS IsActive  FROM [CacheFiles]) cf
                                    ON ci.Id=cf.CacheInfoId
                                    WHERE ci.NodeId=@NodeId
                                    GROUP BY ci.Id, ci.NodeId, ci.DataflowId, ci.TTL"
                })
                {
                    scdCommand.Parameters.Add(new SqliteParameter("@NodeId", DbType.String)
                    { Value = _requestContext.NodeId });
                    var reader = await scdCommand.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        var entry = new DataflowDataCacheInfo
                        {
                            Id = Guid.Parse((string)reader["Id"]),
                            NodeId = (int)(long)reader["NodeId"],
                            DataflowId = (string)reader["DataflowId"],
                            TTL = (int)(long)reader["TTL"],
                            CacheSize = reader["CacheSize"] == DBNull.Value ? 0 : (long)reader["CacheSize"],
                            CachedDataflow = reader["CachedDataflow"] == DBNull.Value
                            ? 0
                            : (long)reader["CachedDataflow"],
                            CachedDataAccess = reader["CachedDataAccess"] == DBNull.Value
                            ? 0
                            : (long)reader["CachedDataAccess"]
                        };
                        cacheInfos.Add(entry);
                    }
                }
            }

            _logger.LogDebug("GetCacheInfoFromNodeId END");
            return cacheInfos;
        }

        public async Task<DataflowDataCacheInfo> CreateDataflowDataCacheInfo(
            DataflowDataCacheInfo dataflowDataCacheInfo)
        {
            _logger.LogDebug($"CreateDataflowDataCacheInfo START dataflowId:{dataflowDataCacheInfo?.DataflowId}");

            InizializeDbIfNotExist();

            var existingCacheInfoEntry =
                await GetCacheInfoByNodeIdAndDataflowId(_requestContext.NodeId, dataflowDataCacheInfo.DataflowId);

            // fix nodeId if needed
            if (dataflowDataCacheInfo?.NodeId != _requestContext.NodeId)
            {
                dataflowDataCacheInfo.NodeId = _requestContext.NodeId;
            }

            // an entry already exists. Update needed
            if (existingCacheInfoEntry != null)
            {
                existingCacheInfoEntry.TTL = Math.Max(dataflowDataCacheInfo.TTL, 0);
                var updateOK = await UpdateDataflowTTLFromNodeId(existingCacheInfoEntry.Id, existingCacheInfoEntry.TTL);
                return updateOK ? existingCacheInfoEntry : null;
            }

            var commandExecutionOK = false;
            using (var conn = new SqliteConnection(GetConnectionStringBuilder().ConnectionString))
            {
                conn.Open();
                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText =
                        @"INSERT INTO [CacheInfo] (Id, NodeId, DataflowId, TTL ) VALUES (@Id, @NodeId, @DataflowId, @TTL)"
                })
                {
                    var newGuid = Guid.NewGuid();
                    var newTTL = Math.Max(dataflowDataCacheInfo.TTL, 0);
                    scdCommand.Parameters.Add(new SqliteParameter("@Id", DbType.String) { Value = newGuid.ToString() });
                    scdCommand.Parameters.Add(new SqliteParameter("@NodeId", DbType.Int32)
                    { Value = _requestContext.NodeId });
                    scdCommand.Parameters.Add(new SqliteParameter("@DataflowId", DbType.String)
                    { Value = dataflowDataCacheInfo.DataflowId });
                    scdCommand.Parameters.Add(new SqliteParameter("@TTL", DbType.Int32) { Value = newTTL });
                    commandExecutionOK = await scdCommand.ExecuteNonQueryAsync() > 0;

                    dataflowDataCacheInfo.TTL = newTTL;
                    dataflowDataCacheInfo.Id = newGuid;
                }
            }

            _logger.LogDebug("CreateDataflowDataCacheInfo END");
            return commandExecutionOK ? dataflowDataCacheInfo : null;
        }

        public async Task<bool> UpdateDataflowTTLFromNodeId(Guid id, int ttl)
        {
            _logger.LogDebug($"UpdateDataflowTTLFromNodeId START with Id:{id}\tTTL:{ttl}");

            InizializeDbIfNotExist();

            var cacheInfos = new List<DataflowDataCacheInfo>();

            var result = false;
            using (var conn = new SqliteConnection(GetConnectionStringBuilder().ConnectionString))
            {
                conn.Open();

                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText = @"UPDATE [CacheInfo] 
                                    SET TTL=@TTL
                                    WHERE [Id]=@Id"
                })
                {
                    scdCommand.Parameters.Add(new SqliteParameter("@Id", DbType.String) { Value = id.ToString() });
                    scdCommand.Parameters.Add(new SqliteParameter("@TTL", DbType.Int32) { Value = ttl });
                    result = await scdCommand.ExecuteNonQueryAsync() > 0;
                }
            }

            _logger.LogDebug("UpdateDataflowTTLFromNodeId END");
            return result;
        }

        public async Task ClearSingleItemCache(Guid cacheInfoId, int nodeId)
        {
            _logger.LogDebug("start to ClearSingleItemCache");

            if (_dataCacheConfig == null || !_dataCacheConfig.IsEnable)
            {
                _logger.LogDebug("exit from ClearSingleItemCache");
                return;
            }

            InizializeDbIfNotExist();

            using (var conn = new SqliteConnection(GetConnectionStringBuilder().ConnectionString))
            {
                conn.Open();

                var found = false;
                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn
                })
                {
                    scdCommand.CommandText =
                        @"SELECT COUNT(*) FROM [CacheInfo] ci INNER JOIN [CacheFiles] cf ON cf.CacheInfoId=ci.Id WHERE CacheInfoId=@CacheInfoId AND NodeId=@NodeId";
                    scdCommand.Parameters.Add(new SqliteParameter("@CacheInfoId", DbType.Int64)
                    { Value = cacheInfoId.ToString() });
                    scdCommand.Parameters.Add(new SqliteParameter("@NodeId", DbType.Int64) { Value = nodeId });
                    found = (long)await scdCommand.ExecuteScalarAsync() > 0;
                }

                if (!found)
                {
                    return;
                }

                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn
                })
                {
                    scdCommand.CommandText =
                        @"UPDATE [CacheFiles] SET DataCache='', FileSize=0 WHERE CacheInfoId=@CacheInfoId";
                    scdCommand.Parameters.Add(new SqliteParameter("@CacheInfoId", DbType.Int64)
                    { Value = cacheInfoId.ToString() });
                    await scdCommand.ExecuteNonQueryAsync();
                }

                _logger.LogDebug("vacuum sqlite");
                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText = @"vacuum"
                })
                {
                    await scdCommand.ExecuteNonQueryAsync();
                }
            }


            _logger.LogDebug("end to ClearSingleItemCache");
        }

        public async Task<GetDataFromDataflowResponse> GetJsonStatForDataflowDataFromValidKeyCompatible(
            DataFromDataflowRequest dataFromDataflowRequest, Dataset dataset)
        {
            try
            {
                if (!checkCacheEnableForCurrentData(dataFromDataflowRequest.DataflowId,
                    _requestContext?.IgnoreCache, _dataCacheConfig?.IsEnable, _dataCacheConfig?.ExclusionList))
                {
                    return null;
                }

                InizializeDbIfNotExist();

                _logger.LogDebug(
                    $"DataflowData start find cache compatible key: {_requestContext.NodeId}\t{_requestContext.UserLang}\t{dataFromDataflowRequest.DataflowId}");

                var cacheKeys =
                    await getKeysCompatibleCandidate(dataFromDataflowRequest.DataflowId, dataFromDataflowRequest);

                var cacheInfo = getFirstFilterCompatibleAndValid(cacheKeys, dataFromDataflowRequest.DataCriterias, dataset);

                if (cacheInfo == null)
                {
                    _logger.LogDebug(
                        $"Not found compatible cache key: {_requestContext.NodeId}\t{_requestContext.UserLang}\t{dataFromDataflowRequest.DataflowId}");
                    return null;
                }

                _logger.LogDebug($"found compatible cache key fileId: {cacheInfo.CacheFileId}");
                using (var conn = new SqliteConnection(GetConnectionStringBuilder().ConnectionString))
                {
                    conn.Open();

                    GetDataFromDataflowResponse objResult = null;

                    using (var scdCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"SELECT DataCache 
                                    FROM [CacheFiles] 
                                    WHERE [Id]=@Id"
                    })
                    {
                        scdCommand.Parameters.Add(new SqliteParameter("@Id", DbType.String)
                        { Value = cacheInfo.CacheFileId.ToString() });
                        var objValueObj = await scdCommand.ExecuteScalarAsync();
                        if (objValueObj != null && objValueObj != DBNull.Value)
                        {
                            var resultStr = (string)objValueObj;
                            if (string.IsNullOrWhiteSpace(resultStr))
                            {
                                _logger.LogDebug("Null value in Cache Id");
                                return null;
                            }

                            objResult = getFileCache(resultStr);
                        }
                        else
                        {
                            _logger.LogDebug("Cache Id not found");
                            return null;
                        }
                    }

                    using (var scdCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"UPDATE [CacheFiles] SET Accesses=Accesses+1 WHERE Id=@Id"
                    })
                    {
                        scdCommand.Parameters.Add(new SqliteParameter("@Id", DbType.String)
                        { Value = cacheInfo.CacheFileId.ToString() });
                        await scdCommand.ExecuteNonQueryAsync();
                    }

                    return objResult;
                }
            }
            catch (SqliteException ex)
            {
                if (ex.SqliteErrorCode == 11)
                {
                    DropDbIfExist(_dataCacheConfig);
                }

                return null;
            }
        }

        public async Task<List<DataflowDataCacheExport>> ExportAccessNumber(int nodeId)
        {
            var exports = new List<DataflowDataCacheExport>();

            using (var conn = new SqliteConnection(GetConnectionStringBuilder().ConnectionString))
            {
                conn.Open();

                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText = @"SELECT ci.DataflowId, ci.NodeId, SUM(cf.Accesses) AS Accesses
                                    FROM CacheFiles cf
                                    INNER JOIN CacheInfo ci
                                    ON ci.Id=cf.CacheInfoId
                                    WHERE NodeId=@NodeId
                                    GROUP BY ci.DataflowId, ci.NodeId"
                })
                {
                    scdCommand.Parameters.Add(new SqliteParameter("@NodeId", DbType.Int32) { Value = nodeId });
                    var reader = await scdCommand.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        exports.Add(new DataflowDataCacheExport
                        {
                            Id = (string)reader["DataflowId"],
                            Access = (long)reader["Accesses"]
                        });
                    }
                }
            }

            return exports;
        }

        public async Task InvalidateKeyForCurrentNodeAndLanguagesHAVEBUG(int nodeId, string dataflowId,
            List<FilterCriteria> dataCriterias)
        {
            try
            {
                _logger.LogDebug($"InvalidateKey Cache: {_requestContext.NodeId}\t{dataflowId}");

                InizializeDbIfNotExist();

                using (var conn = new SqliteConnection(GetConnectionStringBuilder().ConnectionString))
                {
                    conn.Open();

                    var keysFileId = new List<Guid>();
                    using (var scdCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"SELECT cf.Id
                                            FROM [CacheFiles] cf INNER JOIN [CacheInfo] ci ON cf.CacheInfoId=ci.Id 
                                            WHERE ci.[DataflowId]=@DataflowId AND ci.[NodeId]=@NodeId AND Filter=@Filter"
                    })
                    {
                        scdCommand.Parameters.Add(
                            new SqliteParameter("@DataflowId", DbType.String) { Value = dataflowId });
                        scdCommand.Parameters.Add(new SqliteParameter("@NodeId", DbType.String)
                        { Value = _requestContext.NodeId });
                        scdCommand.Parameters.Add(new SqliteParameter("@Filter", DbType.String)
                        {
                            Value = dataCriterias != null ? JsonConvert.SerializeObject(dataCriterias) : "[]"
                        });
                        var reader = await scdCommand.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            keysFileId.Add(Guid.Parse((string)reader["Id"]));
                        }
                    }

                    _logger.LogDebug("Start to empty all key");
                    foreach (var itemKey in keysFileId)
                    {
                        using (var scdCommand = new SqliteCommand
                        {
                            Connection = conn,
                            CommandText = @"UPDATE [CacheFiles]
                                            SET FileSize=0, DataCache=null
                                            WHERE Id=@Id"
                        })
                        {
                            scdCommand.Parameters.Add(new SqliteParameter("@Id", DbType.String)
                            { Value = itemKey.ToString() });
                            await scdCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (SqliteException ex)
            {
                if (ex.SqliteErrorCode == 11)
                {
                    DropDbIfExist(_dataCacheConfig);
                }
            }
        }

        public async Task<List<DataflowDataCacheFile>> GetValidCacheFileIdForDataflowId(string dataflowId)
        {
            return await getAllUnExpiredCachedKeyInfoAssociatedAtSpecificDataflowWithAllData(dataflowId);
        }

        public bool DropDatabase(int nodeId)
        {
            var connString = GetConnectionStringBuilder();

            var worked = false;
            var tries = 1;
            while (tries < 4 && !worked)
            {
                try
                {
                    Thread.Sleep(tries * 200);
                    File.Delete(connString.DataSource);
                    worked = true;
                }
                catch (IOException) // delete only throws this on locking
                {
                    tries++;
                }
            }

            if (!worked)
            {
                _logger.LogInformation($"Unable to delete database file {connString.DataSource}");
                return false;
            }

            return true;
        }

        public async Task InvalidateAllKeysForCurrentNodeAndLanguages(string dataflowId)
        {
            try
            {
                _logger.LogDebug($"InvalidateKey Cache: {_requestContext.NodeId}\t{dataflowId}");

                InizializeDbIfNotExist();

                using (var conn = new SqliteConnection(GetConnectionStringBuilder().ConnectionString))
                {
                    conn.Open();

                    var keysFileId = new List<Guid>();
                    using (var scdCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"UPDATE [CacheFiles]
SET FileSize=0, DataCache=null, CreationDate='1980-09-14 15:09:10.0921198'
WHERE Id IN (
SELECT cf.Id
FROM [CacheFiles] cf INNER JOIN [CacheInfo] ci ON cf.CacheInfoId=ci.Id 
WHERE ci.[DataflowId]=@DataflowId AND ci.[NodeId]=@NodeId
) AND Language=@Language"
                    })
                    {
                        scdCommand.Parameters.Add(
                            new SqliteParameter("@DataflowId", DbType.String) { Value = dataflowId });
                        scdCommand.Parameters.Add(new SqliteParameter("@NodeId", DbType.Int32)
                        { Value = _requestContext.NodeId });
                        scdCommand.Parameters.Add(new SqliteParameter("@Language", DbType.String)
                        { Value = _requestContext.UserLang });

                        await scdCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqliteException ex)
            {
                if (ex.SqliteErrorCode == 11)
                {
                    DropDbIfExist(_dataCacheConfig);
                }
            }
        }

        private string getCacheFileBasePath()
        {
            return Path.Combine(_hostEnvironment.ContentRootPath, _dataCacheConfig.SavedDataFilePath);
        }

        private string getPathCacheFile(int nodeId)
        {
            return $"{getCacheFileBasePath()}/Node{nodeId}";
        }

        private string getPathCacheFileWithLang(int nodeId, string lang)
        {
            return $"{getCacheFileBasePath()}/Node{nodeId}/Lang{lang}";
        }

        private Tuple<long, string> saveFileCache(GetDataFromDataflowResponse getDataFromDataflowResponse)
        {
            if (!_dataCacheConfig.SaveDataOnFile)
            {
                var strResult = JsonConvert.SerializeObject(getDataFromDataflowResponse);
                return new Tuple<long, string>(Encoding.Unicode.GetByteCount(strResult) / 1024, strResult);
            }

            var responseWatch = Stopwatch.StartNew();
            string filePath = null;
            string fileName = null;
            long size = 0;
            try
            {
                var path = getPathCacheFileWithLang(_requestContext.NodeId, _requestContext.UserLang);


                if (!Directory.Exists(_dataCacheConfig.SavedDataFilePath))
                {
                    Directory.CreateDirectory(_dataCacheConfig.SavedDataFilePath);
                }

                if (!Directory.Exists($"{_dataCacheConfig.SavedDataFilePath}/Node{_requestContext.NodeId}"))
                {
                    Directory.CreateDirectory($"{_dataCacheConfig.SavedDataFilePath}/Node{_requestContext.NodeId}");
                }

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                fileName = $"{Path.GetRandomFileName()}.dfdata";
                filePath = $"{path}/{fileName}";
                while (File.Exists(filePath))
                {
                    fileName = Path.GetRandomFileName();
                    filePath = $"{_dataCacheConfig.SavedDataFilePath}{fileName}";
                }

                using (var sw = File.CreateText(filePath))
                {
                    sw.WriteLine(JsonConvert.SerializeObject(getDataFromDataflowResponse));
                }

                var fileToCompress = new FileInfo(filePath);
                using (var originalFileStream = fileToCompress.OpenRead())
                {
                    using (var compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                    {
                        using (var compressionStream = new GZipStream(compressedFileStream,
                            CompressionMode.Compress))
                        {
                            originalFileStream.CopyTo(compressionStream);
                            size = compressedFileStream.Length / 1024;
                            if (size == 0)
                            {
                                size = 1;
                            }

                            fileName += ".gz";
                        }
                    }
                }
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(filePath) &&
                    File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            _logger.LogDebug($"saveFileCache in {responseWatch.ElapsedMilliseconds}ms");
            _logger.LogDebug($"saved cache file {filePath}\tsize:{size}");
            return new Tuple<long, string>(size, fileName);
        }

        private GetDataFromDataflowResponse getFileCache(string cachedValueDb)
        {
            if (!_dataCacheConfig.SaveDataOnFile)
            {
                return JsonConvert.DeserializeObject<GetDataFromDataflowResponse>(cachedValueDb);
            }

            var path = $"{getCacheFileBasePath()}/Node{_requestContext.NodeId}/Lang{_requestContext.UserLang}";
            var filePath = $"{path}/{cachedValueDb}";
            _logger.LogDebug($"read cache file {filePath}");
            var responseWatch = Stopwatch.StartNew();
            using (var originalFileStream = File.OpenRead(filePath))
            {
                using (var decompressedMemoryStream = new MemoryStream())
                {
                    using (var decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedMemoryStream);
                        var result =
                            JsonConvert.DeserializeObject<GetDataFromDataflowResponse>(
                                Encoding.UTF8.GetString(decompressedMemoryStream.ToArray()));
                        _logger.LogDebug($"getFileCache in {responseWatch.ElapsedMilliseconds}ms");
                        return result;
                    }
                }
            }
        }

        private bool dataCacheIsStillValid(Dataset dataset, DataflowDataCacheFile cacheInfo)
        {
            var strDateValue = dataset.LastUpdate;

            if (!string.IsNullOrWhiteSpace(strDateValue))
            {
                try
                {
                    var lastUpdate = Convert.ToDateTime(strDateValue);
                    if (cacheInfo.CreationDate < lastUpdate.ToUniversalTime())
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    _logger.LogDebug($"Error when parsing datetime {strDateValue}");
                }
            }

            var currentAnnotations = createAnnotationString(dataset);
            if (!currentAnnotations.Equals(cacheInfo.Annotations))
            {
                return false;
            }

            return true;
        }

        private bool checkCacheEnableForCurrentData(string dataflowId, bool? ignoreCache,
            bool? isEnable, List<string> exclusionList)
        {
            if (string.IsNullOrWhiteSpace(dataflowId))
            {
                _logger.LogDebug("DataFromDataflowRequest invalid data");
                return false;
            }

            if (ignoreCache.HasValue && ignoreCache.Value)
            {
                _logger.LogDebug("Request with disabled cache");
                return false;
            }

            if (!isEnable.HasValue || !isEnable.Value)
            {
                _logger.LogDebug("DataflowData cache is disable");
                return false;
            }

            if (exclusionList != null &&
                exclusionList.Contains(dataflowId))
            {
                _logger.LogDebug($"DataflowData {dataflowId} is in ExclusionList");
                return false;
            }

            return true;
        }

        public void DropDbIfExist(DataflowDataCacheConfig dataCacheConfig)
        {
            var stringBuilder = GetConnectionStringBuilder();
            stringBuilder.ConnectionString = dataCacheConfig.ConnectionString;
            var result = File.Exists(stringBuilder.DataSource);
            if (result)
            {
                File.Delete(stringBuilder.DataSource);
            }
        }

        private async Task clearCacheAsync()
        {
            _logger.LogDebug("start to clearCacheAsync");
            using (var conn = new SqliteConnection(GetConnectionStringBuilder().ConnectionString))
            {
                conn.Open();

                var ttlDefault = _dataCacheConfig.Expiration;
                var nodes = await _nodeRepository.ListAllAsync();
                var ttlNodes = new Dictionary<int, int>();
                foreach (var node in nodes)
                {
                    var nodeData = await _nodeRepository.GetByIdAsync(node.NodeId);
                    if (node?.TtlDataflow != null && node.TtlDataflow.HasValue)
                    {
                        ttlNodes.Add(nodeData.NodeId, node.TtlDataflow.Value);
                    }
                    else
                    {
                        ttlNodes.Add(nodeData.NodeId, ttlDefault);
                    }
                }


                //Remove expired
                _logger.LogDebug("take id expired");
                var ids = new List<string>();
                foreach (var item in ttlNodes)
                {
                    using (var scdCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"SELECT cf.Id
                                    FROM [CacheFiles] cf INNER JOIN [CacheInfo] ci ON cf.CacheInfoId=ci.Id 
                                    WHERE ci.[NodeId]=@NodeId AND @TTLDate>CreationDate"
                    })
                    {
                        scdCommand.Parameters.Add(new SqliteParameter("@NodeId", DbType.Int32) { Value = item.Key });
                        scdCommand.Parameters.Add(new SqliteParameter("@TTLDate", DbType.DateTime)
                        { Value = DateTime.UtcNow.AddSeconds(-item.Value) });
                        var reader = await scdCommand.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            ids.Add((string)reader["Id"]);
                        }
                    }
                }

                _logger.LogDebug("Remove expired");
                foreach (var item in ids)
                {
                    using (var scdCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"UPDATE [CacheFiles]
                                        SET DataCache='', FileSize=0 
                                        WHERE Id=@Id"
                    })
                    {
                        scdCommand.Parameters.Add(new SqliteParameter("@Id", DbType.String) { Value = item });
                        await scdCommand.ExecuteNonQueryAsync();
                    }
                }

                _logger.LogDebug("check size sqlite");
                var dbSizeKb = 0L;
                var dbMaxSizeKb = _dataCacheConfig.MaxSize * 1024L;
                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText = @"SELECT SUM(FileSize)
                                    FROM [CacheFiles] cf"
                })
                {
                    var dbSizeKbObj = await scdCommand.ExecuteScalarAsync();
                    if (dbSizeKbObj != null && dbSizeKbObj != DBNull.Value)
                    {
                        dbSizeKb = (long)dbSizeKbObj;
                    }
                }

                if (dbSizeKb <= dbMaxSizeKb)
                {
                    if (ids.Count > 0)
                    {
                        _logger.LogDebug("vacuum sqlite");
                        using (var scdCommand = new SqliteCommand
                        {
                            Connection = conn,
                            CommandText = @"vacuum"
                        })
                        {
                            await scdCommand.ExecuteNonQueryAsync();
                        }
                    }

                    _logger.LogDebug("end to clearCacheAsync with NO Resize");
                    return;
                }

                _logger.LogDebug("read cleaner data");
                var cleaners = new List<string>();
                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText = @"SELECT cf.Id, FileSize
                                    FROM [CacheFiles] cf INNER JOIN [CacheInfo] ci ON cf.CacheInfoId=ci.Id 
                                    ORDER BY Accesses, CreationDate DESC"
                })
                {
                    var reader = await scdCommand.ExecuteReaderAsync();
                    while (reader.Read() && dbSizeKb > dbMaxSizeKb)
                    {
                        dbSizeKb -= (long)reader["FileSize"];
                        cleaners.Add((string)reader["Id"]);
                    }
                }

                _logger.LogDebug("execute cleaner data");
                foreach (var item in cleaners)
                {
                    using (var scdCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"UPDATE [CacheFiles]
                                        SET DataCache='', FileSize=0 
                                        WHERE Id=@Id"
                    })
                    {
                        scdCommand.Parameters.Add(new SqliteParameter("@Id", DbType.String) { Value = item });
                        await scdCommand.ExecuteNonQueryAsync();
                    }
                }

                _logger.LogDebug("vacuum sqlite");
                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText = @"vacuum"
                })
                {
                    await scdCommand.ExecuteNonQueryAsync();
                }

                _logger.LogDebug("end to clearCacheAsync with Resize");
            }
        }

        public async Task<DataflowDataCacheInfo> GetCacheInfoByNodeIdAndDataflowId(int nodeId, string dataflowId)
        {
            _logger.LogDebug($"GetCacheInfoByNodeIdAndDataflowId START with NodeId {nodeId}");

            InizializeDbIfNotExist();

            using (var conn = new SqliteConnection(GetConnectionStringBuilder().ConnectionString))
            {
                conn.Open();

                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText =
                        @"SELECT ci.*, SUM(FileSize) AS CacheSize, SUM(IsActive) CachedDataflow, SUM(Accesses) CachedDataAccess
                                    FROM [CacheInfo] ci
                                    LEFT JOIN (SELECT CacheInfoId, FileSize, Accesses, CASE FileSize WHEN 0 THEN 0 ELSE 1 END AS IsActive  FROM [CacheFiles]) cf
                                    ON ci.Id=cf.CacheInfoId
                                    WHERE ci.NodeId=@NodeId AND ci.DataflowId = @DataflowId
                                    GROUP BY ci.Id, ci.NodeId, ci.DataflowId, ci.TTL"
                })
                {
                    scdCommand.Parameters.Add(new SqliteParameter("@NodeId", DbType.String) { Value = nodeId });
                    scdCommand.Parameters.Add(new SqliteParameter("@DataflowId", DbType.String) { Value = dataflowId });
                    var reader = await scdCommand.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        var result = new DataflowDataCacheInfo
                        {
                            Id = Guid.Parse((string)reader["Id"]),
                            NodeId = (int)(long)reader["NodeId"],
                            DataflowId = (string)reader["DataflowId"],
                            TTL = (int)(long)reader["TTL"],
                            CacheSize = reader["CacheSize"] == DBNull.Value ? 0 : (long)reader["CacheSize"],
                            CachedDataflow = reader["CachedDataflow"] == DBNull.Value
                            ? 0
                            : (long)reader["CachedDataflow"],
                            CachedDataAccess = reader["CachedDataAccess"] == DBNull.Value
                            ? 0
                            : (long)reader["CachedDataAccess"]
                        };
                        return result;
                    }
                }
            }

            _logger.LogDebug("GetCacheInfoByNodeIdAndDataflowId END");
            return null;
        }

        private string createAnnotationString(Dataset dataset)
        {
            var notDisplayStr = dataset.NotDisplay != null ? JsonConvert.SerializeObject(dataset.NotDisplay) : "[]";

            return $"MaxObs_{dataset.MaxObservation ?? -1}:NotDisplay_{notDisplayStr}";
        }

        private async Task<List<DataflowDataCacheFile>>
            getAllUnExpiredCachedKeyInfoAssociatedAtSpecificDataflowWithAllData(string dataflowId)
        {
            _logger.LogDebug("START getAllUnExpiredCachedKeyInfoAssociatedAtSpecificDataflowWithAllData");
            var results = new List<DataflowDataCacheFile>();

            using (var conn = new SqliteConnection(GetConnectionStringBuilder().ConnectionString))
            {
                conn.Open();
                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText =
                        @"SELECT cf.CreationDate, cf.Id, cf.CacheInfoId, cf.Language, cf.Annotations, cf.DataType, cf.Filter, cf.FileSize, cf.Accesses, ci.TTL
                                    FROM [CacheFiles] cf INNER JOIN [CacheInfo] ci ON cf.CacheInfoId=ci.Id 
                                    WHERE cf.FileSize>0 AND ci.[DataflowId]=@DataflowId AND ci.[NodeId]=@NodeId AND Language=@Language AND 
                                    (DataType=@DataType OR DataType=@DataType2)
                                    ORDER BY cf.FileSize ASC"
                })
                {
                    scdCommand.Parameters.Add(new SqliteParameter("@DataflowId", DbType.String) { Value = dataflowId });
                    scdCommand.Parameters.Add(new SqliteParameter("@NodeId", DbType.String)
                    { Value = _requestContext.NodeId });
                    scdCommand.Parameters.Add(new SqliteParameter("@Language", DbType.String)
                    { Value = _requestContext.UserLang });
                    scdCommand.Parameters.Add(
                        new SqliteParameter("@DataType", DbType.String) { Value = DataType.AllData });
                    scdCommand.Parameters.Add(
                        new SqliteParameter("@DataType2", DbType.String) { Value = DataType.UnKnow });

                    var reader = await scdCommand.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        var creationDate = Convert.ToDateTime(reader["CreationDate"]);
                        var expireDelta = DateTime.UtcNow.AddSeconds(-(long)reader["TTL"]);
                        if (expireDelta > creationDate)
                        {
                            _logger.LogDebug("Cache key expired");
                            continue;
                        }

                        _logger.LogDebug("Cache key found");
                        results.Add(new DataflowDataCacheFile
                        {
                            CacheFileId = Guid.Parse((string)reader["Id"]),
                            CacheInfoId = Guid.Parse((string)reader["CacheInfoId"]),
                            Filter = JsonConvert.DeserializeObject<List<FilterCriteria>>((string)reader["Filter"]),
                            Language = (string)reader["Language"],
                            Annotations = (string)reader["Annotations"],
                            DataType = (DataType)(long)reader["DataType"],
                            CreationDate = creationDate,
                            FileSize = (int)(long)reader["FileSize"],
                            Accesses = (int)(long)reader["Accesses"]
                        });
                    }

                    _logger.LogDebug("END getAllUnExpiredCachedKeyInfoAssociatedAtSpecificDataflowWithAllData");
                    return results;
                }
            }
        }

        private async Task<List<DataflowDataCacheFile>> getKeysCompatibleCandidate(string dataflowId,
            DataFromDataflowRequest dataFromDataflowRequest)
        {
            _logger.LogDebug("START getAllUnExpiredCachedKeyInfoAssociatedAtSpecificDataflowWithAllData");
            var results = new List<DataflowDataCacheFile>();

            using (var conn = new SqliteConnection(GetConnectionStringBuilder().ConnectionString))
            {
                conn.Open();

                using (var scdCommand = new SqliteCommand())
                {
                    scdCommand.Connection = conn;

                    if (dataFromDataflowRequest.DataCriterias != null &&
                        dataFromDataflowRequest.DataCriterias.Count == 1 &&
                        dataFromDataflowRequest.DataCriterias.First().FilterValues != null &&
                        dataFromDataflowRequest.DataCriterias.First().FilterValues.Count == 1)
                    {
                        getKeyCompatibleCandidateOptimizeAlgorithmForOneCriteriaOneValue(scdCommand, dataflowId,
                            dataFromDataflowRequest);
                    }
                    else
                    {
                        getKeyCompatibleCandidateGenericAlgorithm(scdCommand, dataflowId);
                    }

                    var reader = await scdCommand.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        var creationDate = Convert.ToDateTime(reader["CreationDate"]);
                        var expireDelta = DateTime.UtcNow.AddSeconds(-(long)reader["TTL"]);
                        if (expireDelta > creationDate)
                        {
                            continue;
                        }

                        _logger.LogDebug("Cache key found");
                        results.Add(new DataflowDataCacheFile
                        {
                            CacheFileId = Guid.Parse((string)reader["Id"]),
                            CacheInfoId = Guid.Parse((string)reader["CacheInfoId"]),
                            Filter = JsonConvert.DeserializeObject<List<FilterCriteria>>((string)reader["Filter"]),
                            Language = (string)reader["Language"],
                            Annotations = (string)reader["Annotations"],
                            DataType = (DataType)(long)reader["DataType"],
                            CreationDate = creationDate,
                            FileSize = (int)(long)reader["FileSize"],
                            Accesses = (int)(long)reader["Accesses"]
                        });
                    }
                }
            }

            _logger.LogDebug($"found {results.Count} results");
            return results;
        }

        private void getKeyCompatibleCandidateOptimizeAlgorithmForOneCriteriaOneValue(SqliteCommand scdCommand,
            string dataflowId, DataFromDataflowRequest dataFromDataflowRequest)
        {
            _logger.LogDebug("start getKeyCompatibleCandidateOptimizeAlgorithmForOneCriteriaOneValue");
            scdCommand.CommandText =
                @"SELECT cf.CreationDate, cf.Id, cf.CacheInfoId, cf.Language, cf.Annotations, cf.DataType, cf.Filter, cf.FileSize, cf.Accesses, ci.TTL
                                    FROM [CacheFiles] cf INNER JOIN [CacheInfo] ci ON cf.CacheInfoId=ci.Id 
                                    WHERE cf.FileSize>0 AND ci.[DataflowId]=@DataflowId AND ci.[NodeId]=@NodeId AND Language=@Language AND 
                                    (DataType=@DataType OR DataType=@DataType2) AND UPPER(Filter) LIKE @Filter
                                    ORDER BY cf.FileSize ASC";
            scdCommand.Parameters.Add(new SqliteParameter("@DataflowId", DbType.String) { Value = dataflowId });
            scdCommand.Parameters.Add(new SqliteParameter("@NodeId", DbType.String) { Value = _requestContext.NodeId });
            scdCommand.Parameters.Add(
                new SqliteParameter("@Language", DbType.String) { Value = _requestContext.UserLang });
            scdCommand.Parameters.Add(new SqliteParameter("@DataType", DbType.String) { Value = DataType.AllData });
            scdCommand.Parameters.Add(new SqliteParameter("@DataType2", DbType.String) { Value = DataType.UnKnow });
            var filtervalues =
                $"%\"{dataFromDataflowRequest.DataCriterias.First().FilterValues.First().ToUpperInvariant()}\"%";
            scdCommand.Parameters.Add(new SqliteParameter("@Filter", DbType.String) { Value = filtervalues });

            _logger.LogDebug($"query {scdCommand.CommandText}\nfiltervalues:{filtervalues}");
        }

        private void getKeyCompatibleCandidateGenericAlgorithm(SqliteCommand scdCommand, string dataflowId)
        {
            _logger.LogDebug("start getKeyCompatibleCandidateGenericAlgorithm");

            scdCommand.CommandText =
                @"SELECT cf.CreationDate, cf.Id, cf.CacheInfoId, cf.Language, cf.Annotations, cf.DataType, cf.Filter, cf.FileSize, cf.Accesses, ci.TTL
                                    FROM [CacheFiles] cf INNER JOIN [CacheInfo] ci ON cf.CacheInfoId=ci.Id 
                                    WHERE cf.FileSize>0 AND ci.[DataflowId]=@DataflowId AND ci.[NodeId]=@NodeId AND Language=@Language AND 
                                    (DataType=@DataType OR DataType=@DataType2)
                                    ORDER BY cf.FileSize ASC";
            scdCommand.Parameters.Add(new SqliteParameter("@DataflowId", DbType.String) { Value = dataflowId });
            scdCommand.Parameters.Add(new SqliteParameter("@NodeId", DbType.String) { Value = _requestContext.NodeId });
            scdCommand.Parameters.Add(
                new SqliteParameter("@Language", DbType.String) { Value = _requestContext.UserLang });
            scdCommand.Parameters.Add(new SqliteParameter("@DataType", DbType.String) { Value = DataType.AllData });
            scdCommand.Parameters.Add(new SqliteParameter("@DataType2", DbType.String) { Value = DataType.UnKnow });
        }

        private DataflowDataCacheFile getFirstFilterCompatibleAndValid(
            List<DataflowDataCacheFile> dataflowDataCacheFiles, List<FilterCriteria> dataCriterias,
            Dataset dataset)
        {
            if (dataflowDataCacheFiles == null)
            {
                return null;
            }

            foreach (var keyItem in dataflowDataCacheFiles)
            {
                var hasTimePeriodFilter = dataCriterias == null || dataCriterias.Any(i => i.Type == FilterType.TimePeriod);
                if (hasTimePeriodFilter)
                {
                    //JsonStat filter only for FREQ = A? In this case result false when select Freq != A
                }

                var isValid = dataCacheIsStillValid(dataset, keyItem);
                if (!isValid)
                {
                    continue;
                }

                if (keyItem.Filter == null || keyItem.Filter.Count == 0)
                {
                    return keyItem;
                }

                if (filterIsCompatible(keyItem.Filter, dataCriterias))
                {
                    return keyItem;
                }
            }

            return null;
        }

        private bool filterIsCompatible(List<FilterCriteria> cachedFilter, List<FilterCriteria> currentFilter)
        {
            //currentFilter must be more restrictive than cacheFilter for return True
            if (cachedFilter == null || cachedFilter.Count <= 0)
            {
                return true;
            }

            foreach (var itemColumnCached in cachedFilter)
            {
                var columnCurrent = currentFilter.FirstOrDefault(i =>
                    i.Id.Equals(itemColumnCached.Id, StringComparison.InvariantCultureIgnoreCase) &&
                    i.Type == itemColumnCached.Type);
                if (columnCurrent == null)
                {
                    //cachedFilter must have all column in currentFilter
                    return false;
                }

                if ((itemColumnCached.Type == FilterType.CodeValues || itemColumnCached.Type == FilterType.StringValues) &&
                    itemColumnCached.FilterValues != null &&
                    itemColumnCached.FilterValues.Count > 0)
                {
                    foreach (var currentItemValue in columnCurrent.FilterValues)
                    {
                        //currentFilter must have all items in cachedFilter
                        if (!itemColumnCached.FilterValues.Any(i =>
                            i.Equals(currentItemValue, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            return false;
                        }
                    }
                }
                else if (itemColumnCached.Type == FilterType.TimePeriod)
                {
                    if (itemColumnCached.Period < columnCurrent.Period)
                    {
                        return false;
                    }
                }
                else if (itemColumnCached.Type == FilterType.TimeRange)
                {
                    if ((itemColumnCached.From == null && columnCurrent.From != null) ||
                        (columnCurrent.From == null && itemColumnCached.From != null))
                    {
                        return false;
                    }

                    if ((itemColumnCached.To == null && columnCurrent.To != null) ||
                        (columnCurrent.To == null && itemColumnCached.To != null))
                    {
                        return false;
                    }

                    if (itemColumnCached.From != null && columnCurrent.From != null &&
                        itemColumnCached.From > columnCurrent.From)
                    {
                        return false;
                    }

                    if (itemColumnCached.To != null && columnCurrent.To != null &&
                        itemColumnCached.To < columnCurrent.To)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public SqliteConnectionStringBuilder GetConnectionStringBuilder(int? nodeId = null)
        {
            var nodeIdValue = nodeId ?? _requestContext.NodeId;

            var stringBuilder = new SqliteConnectionStringBuilder
            {
                ConnectionString = _dataCacheConfig.ConnectionString
            };

            var extFile = new FileInfo(stringBuilder.DataSource).Extension;
            stringBuilder.DataSource = Path.Combine(_hostEnvironment.ContentRootPath, stringBuilder.DataSource);

            if (string.IsNullOrWhiteSpace(extFile))
            {
                stringBuilder.ConnectionString += $"_Node_{nodeIdValue}";
            }
            else
            {
                stringBuilder.ConnectionString =
                    stringBuilder.ConnectionString.Replace(extFile, $"_Node_{_requestContext.NodeId}{extFile}");
            }

            return stringBuilder;
        }

        public async Task ClearNodeCacheAsync(int nodeId)
        {
            await ClearCacheDataflowDataAsync(nodeId);
        }

        public async Task ClearSingleDataflowCache(string dataflowId, int nodeId)
        {
            if (_dataCacheConfig == null || !_dataCacheConfig.IsEnable)
            {
                return;
            }

            InizializeDbIfNotExist(nodeId);

            using (var conn = new SqliteConnection(GetConnectionStringBuilder(nodeId).ConnectionString))
            {
                conn.Open();

                //TODO take and remove DataCache path file

                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn
                })
                {
                    scdCommand.CommandText = $@"UPDATE [CacheFiles] SET DataCache='', FileSize=0  WHERE CacheInfoId IN (SELECT Id FROM CacheInfo WHERE NodeId=@NodeId AND DataflowId=@DataflowId)";
                    scdCommand.Parameters.Add(new SqliteParameter($"@NodeId", DbType.Int64) { Value = nodeId });
                    scdCommand.Parameters.Add(new SqliteParameter($"@DataflowId", DbType.String) { Value = dataflowId });
                    await scdCommand.ExecuteNonQueryAsync();
                }
                _logger.LogDebug($"Clear cache for dataflow {dataflowId} in node {nodeId}");
            }
        }
    }
}
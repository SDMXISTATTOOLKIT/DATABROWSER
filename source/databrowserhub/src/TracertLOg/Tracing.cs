using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace TracertLOg
{
    public class Tracing
    {
        public enum OperationTypeEnum
        {
            ControllerStartRequest,
            NsiRequestStart,
            NsiResponseMessage,
            NsiResponseHeader,
            NsiResponseDownloaded,
            NsiDataMessageProcessed,
            NsiRequestEnd,
            SdmxToJsonStat,
            SdmxXmlToSdmxObjectReader,
            SerializeModelView,
            ControllerEndRequest
        }

        public static DateTime GetServerLogTime()
        {
            return DateTime.Now;
        }

        public static async Task WriteTraceAsync(string requestUrl, string requestBody, string requestOperation,
            string operationName, DateTime logDateTime, string operationId, string userGuid, long responseTime)
        {
            try
            {
                InizializeDbIfNotExist();

                using (var connection = new SqliteConnection("Data Source=DB\\TracingQuery.sqlite"))
                {
                    connection.Open();
                    using (var command = new SqliteCommand())
                    {
                        command.Connection = connection;
                        command.CommandText =
                            @"INSERT INTO Tracing (OperationDateTime, OperationGuid, OperationUserGuid, OperationName, RequestUrl, RequestBody, RequestOperation, ResponseTime)
                                            VALUES (@OperationDateTime, @OperationGuid, @OperationUserGuid, @OperationName, @RequestUrl, @RequestBody, @RequestOperation, @ResponseTime)";
                        var param = command.CreateParameter();
                        param.DbType = DbType.DateTime;
                        param.Direction = ParameterDirection.Input;
                        param.ParameterName = "@OperationDateTime";
                        param.Value = logDateTime == default ? DateTime.Now : logDateTime;
                        command.Parameters.Add(param);

                        param = command.CreateParameter();
                        param.DbType = DbType.String;
                        param.Direction = ParameterDirection.Input;
                        param.ParameterName = "@OperationGuid";
                        param.Value = operationId;
                        command.Parameters.Add(param);

                        param = command.CreateParameter();
                        param.DbType = DbType.String;
                        param.Direction = ParameterDirection.Input;
                        param.ParameterName = "@OperationUserGuid";
                        param.Value = userGuid;
                        command.Parameters.Add(param);

                        param = command.CreateParameter();
                        param.DbType = DbType.String;
                        param.Direction = ParameterDirection.Input;
                        param.ParameterName = "@OperationName";
                        param.Value = operationName;
                        command.Parameters.Add(param);

                        param = command.CreateParameter();
                        param.DbType = DbType.String;
                        param.Direction = ParameterDirection.Input;
                        param.ParameterName = "@RequestUrl";
                        param.Value = requestUrl;
                        command.Parameters.Add(param);

                        param = command.CreateParameter();
                        param.DbType = DbType.String;
                        param.Direction = ParameterDirection.Input;
                        param.ParameterName = "@RequestBody";
                        param.Value = requestBody;
                        command.Parameters.Add(param);

                        param = command.CreateParameter();
                        param.DbType = DbType.String;
                        param.Direction = ParameterDirection.Input;
                        param.ParameterName = "@RequestOperation";
                        param.Value = requestOperation;
                        command.Parameters.Add(param);

                        param = command.CreateParameter();
                        param.DbType = DbType.Int32;
                        param.Direction = ParameterDirection.Input;
                        param.ParameterName = "@ResponseTime";
                        param.Value = responseTime;
                        command.Parameters.Add(param);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
            }
        }


        public static async Task<List<ResponseTracing>> ReadTracingAsync(string operationId = null,
            bool simpleMode = false,
            string filterBy = null,
            int lastNsiOperation = -1)
        {
            var response = new List<ResponseTracing>();

            try
            {
                InizializeDbIfNotExist();
                using (var connection = new SqliteConnection("Data Source=DB\\TracingQuery.sqlite"))
                {
                    connection.Open();

                    using (var command = new SqliteCommand())
                    {
                        var simpleQuery = "";
                        if (simpleMode)
                            simpleQuery = $"AND OperationName LIKE '{OperationTypeEnum.NsiResponseHeader}%'";

                        var limitStr = "";
                        if (lastNsiOperation >= 0)
                        {
                            limitStr = $" LIMIT {lastNsiOperation}";
                        }
                        else if (!string.IsNullOrEmpty(filterBy))
                        {
                            simpleQuery = @"AND OperationName LIKE @OperationName";
                            var paramFilter = command.CreateParameter();
                            paramFilter.DbType = DbType.String;
                            paramFilter.Direction = ParameterDirection.Input;
                            paramFilter.ParameterName = "@OperationName";
                            paramFilter.Value = filterBy;
                            command.Parameters.Add(paramFilter);
                        }

                        command.Connection = connection;
                        command.CommandText =
                            $"SELECT * FROM Tracing WHERE OperationGuid LIKE @OperationGuid {simpleQuery} ORDER BY Id DESC{limitStr}";

                        var param = command.CreateParameter();
                        param.DbType = DbType.String;
                        param.Direction = ParameterDirection.Input;
                        param.ParameterName = "@OperationGuid";
                        param.Value = operationId ?? "%";
                        command.Parameters.Add(param);

                        var reader = await command.ExecuteReaderAsync();

                        while (reader.Read())
                            response.Add(new ResponseTracing
                            {
                                RequestUrl = (string) reader["RequestUrl"],
                                RequestBody = (string) reader["RequestBody"],
                                RequestOperation = (string) reader["RequestOperation"],
                                OperationName = (string) reader["OperationName"],
                                LogDateTime = Convert.ToDateTime(reader["OperationDateTime"]),
                                OperationId = (string) reader["OperationGuid"],
                                UserGuid = (string) reader["OperationUserGuid"],
                                ResponseTime = (long) reader["ResponseTime"]
                            });
                    }
                }
            }
            catch (Exception)
            {
            }

            return response;
        }

        public static void InizializeDbIfNotExist()
        {
            using (var conn = new SqliteConnection("Data Source=DB\\TracingQuery.sqlite"))
            {
                conn.Open();
                var exist = false;
                using (var scdCommand = new SqliteCommand
                {
                    Connection = conn,
                    CommandText = @"SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'Tracing';"
                })
                {
                    exist = (long) scdCommand.ExecuteScalar() > 0;
                }

                if (!exist)
                    using (var scdCommandCreate = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText = @"CREATE TABLE Tracing (
                            Id    INTEGER PRIMARY KEY AUTOINCREMENT,
                            OperationDateTime TEXT,
                            OperationGuid TEXT,
                            OperationUserGuid TEXT,
                            OperationName TEXT,
                            RequestUrl    TEXT,
                            RequestBody   TEXT,
                            RequestOperation  TEXT,
                            ResponseTime  INTEGER)"
                    })
                    {
                        scdCommandCreate.ExecuteNonQuery();
                    }
            }
        }
    }
}
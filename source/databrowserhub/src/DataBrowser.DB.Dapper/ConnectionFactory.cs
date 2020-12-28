using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using DataBrowser.AC;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Database;

namespace DataBrowser.DB.Dapper
{
    public static class ConnectionFactory
    {
        public enum DataAccessProviderTypes
        {
            SqlServer,
            SqLite,
            MySql,
            PostgreSql
        }

        public static IDbConnection CreateDbConnection(DatabaseConfig databaseConfig)
        {
            IDbConnection connection;

            if (databaseConfig == null) return null;

            try
            {
                var factory = GetDbProviderFactory(databaseConfig.QueryOptimizerDbType);

                connection = factory.CreateConnection();

                if (connection != null) connection.ConnectionString = databaseConfig.QueryOptimizerConnectionString;
            }
            catch (Exception)
            {
                connection = null;
            }

            return connection;
        }

        public static IGeometryDbConnection CreateGeometryDbConnection(GeometryDatabaseConfig geometryDatabaseConfig)
        {
            IGeometryDbConnection connection = new GeometryDbConnection();

            if (geometryDatabaseConfig == null) return null;

            try
            {
                var factory = GetDbProviderFactory(geometryDatabaseConfig.DbType.ToLowerInvariant());

                connection.IDbConnection = factory.CreateConnection();

                if (connection != null)
                    connection.IDbConnection.ConnectionString = geometryDatabaseConfig.ConnectionString;
            }
            catch (Exception)
            {
                connection = null;
            }

            return connection;
        }


        public static DbProviderFactory GetDbProviderFactory(string providerName)
        {
            var providername = providerName.ToLower();

            if (providerName == "system.data.sqlclient") return GetDbProviderFactory(DataAccessProviderTypes.SqlServer);
            if (providerName == "system.data.sqlite" || providerName == "microsoft.data.sqlite" ||
                providerName == "sqlite") return GetDbProviderFactory(DataAccessProviderTypes.SqLite);
            if (providerName == "mysql.data.mysqlclient" || providername == "mysql.data")
                return GetDbProviderFactory(DataAccessProviderTypes.MySql);
            if (providerName == "npgsql") return GetDbProviderFactory(DataAccessProviderTypes.PostgreSql);
            throw new NotSupportedException(providerName);
        }

        public static DbProviderFactory GetDbProviderFactory(string dbProviderFactoryTypename, string assemblyName)
        {
            var instance = GetStaticProperty(dbProviderFactoryTypename, "Instance");
            if (instance == null)
            {
                var a = LoadAssembly(assemblyName);
                if (a != null)
                    instance = GetStaticProperty(dbProviderFactoryTypename, "Instance");
            }

            if (instance == null) throw new InvalidOperationException(dbProviderFactoryTypename);

            return instance as DbProviderFactory;
        }

        public static DbProviderFactory GetDbProviderFactory(DataAccessProviderTypes type)
        {
            if (type == DataAccessProviderTypes.SqlServer)
                return SqlClientFactory.Instance; // this library has a ref to SqlClient so this works
            if (type == DataAccessProviderTypes.SqLite)
                return GetDbProviderFactory("Microsoft.Data.Sqlite.SqliteFactory", "Microsoft.Data.Sqlite");
            if (type == DataAccessProviderTypes.MySql)
                return GetDbProviderFactory("MySql.Data.MySqlClient.MySqlClientFactory", "MySql.Data");
            if (type == DataAccessProviderTypes.PostgreSql)
                return GetDbProviderFactory("Npgsql.NpgsqlFactory", "Npgsql");
            throw new NotSupportedException(type.ToString());
        }


        /*
         **************************************************************
         *  Author: Rick Strahl 
         *          © West Wind Technologies, 2008 - 2009
         *          http://www.west-wind.com/
         * 
         * Created: 09/08/2008
         *
         * Permission is hereby granted, free of charge, to any person
         * obtaining a copy of this software and associated documentation
         * files (the "Software"), to deal in the Software without
         * restriction, including without limitation the rights to use,
         * copy, modify, merge, publish, distribute, sublicense, and/or sell
         * copies of the Software, and to permit persons to whom the
         * Software is furnished to do so, subject to the following
         * conditions:
         * 
         * The above copyright notice and this permission notice shall be
         * included in all copies or substantial portions of the Software.
         * 
         * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
         * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
         * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
         * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
         * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
         * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
         * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
         * OTHER DEALINGS IN THE SOFTWARE.
         **************************************************************  
        */


        /// <summary>
        ///     Retrieves a value from  a static property by specifying a type full name and property
        /// </summary>
        /// <param name="typeName">Full type name (namespace.class)</param>
        /// <param name="property">Property to get value from</param>
        /// <returns></returns>
        public static object GetStaticProperty(string typeName, string property)
        {
            var type = GetTypeFromName(typeName);
            if (type == null)
                return null;

            return GetStaticProperty(type, property);
        }

        /// <summary>
        ///     Returns a static property from a given type
        /// </summary>
        /// <param name="type">Type instance for the static property</param>
        /// <param name="property">Property name as a string</param>
        /// <returns></returns>
        public static object GetStaticProperty(Type type, string property)
        {
            object result = null;
            try
            {
                result = type.InvokeMember(property,
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty, null,
                    type, null);
            }
            catch
            {
                return null;
            }

            return result;
        }

        /// <summary>
        ///     Overload for backwards compatibility which only tries to load
        ///     assemblies that are already loaded in memory.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetTypeFromName(string typeName)
        {
            return GetTypeFromName(typeName, null);
        }

        /// <summary>
        ///     Helper routine that looks up a type name and tries to retrieve the
        ///     full type reference using GetType() and if not found looking
        ///     in the actively executing assemblies and optionally loading
        ///     the specified assembly name.
        /// </summary>
        /// <param name="typeName">type to load</param>
        /// <param name="assemblyName">
        ///     Optional assembly name to load from if type cannot be loaded initially.
        ///     Use for lazy loading of assemblies without taking a type dependency.
        /// </param>
        /// <returns>null</returns>
        public static Type GetTypeFromName(string typeName, string assemblyName)
        {
            var type = Type.GetType(typeName, false);
            if (type != null)
                return type;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            // try to find manually
            foreach (var asm in assemblies)
            {
                type = asm.GetType(typeName, false);

                if (type != null)
                    break;
            }

            if (type != null)
                return type;

            // see if we can load the assembly
            if (!string.IsNullOrEmpty(assemblyName))
            {
                var a = LoadAssembly(assemblyName);
                if (a != null)
                {
                    type = Type.GetType(typeName, false);
                    if (type != null)
                        return type;
                }
            }

            return null;
        }

        /// <summary>
        ///     Try to load an assembly into the application's app domain.
        ///     Loads by name first then checks for filename
        /// </summary>
        /// <param name="assemblyName">Assembly name or full path</param>
        /// <returns>null on failure</returns>
        public static Assembly LoadAssembly(string assemblyName)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch
            {
            }

            if (assembly != null)
                return assembly;

            if (File.Exists(assemblyName))
            {
                assembly = Assembly.LoadFrom(assemblyName);
                if (assembly != null)
                    return assembly;
            }

            return null;
        }
    }
}
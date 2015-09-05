// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Lappa_ORM;

namespace Framework.Database
{
    public class DB
    {
        public static Lappa_ORM.Database Auth = new Lappa_ORM.Database();

        public static string CreateConnectionString(string host, string user, string password, string database, int port, int minPoolSize, int maxPoolSize, ConnectionType connType)
        {
            if (connType == ConnectionType.MYSQL)
                return $"Server={host};User Id={user};Port={port};Password={password};Database={database};Allow Zero Datetime=True;Pooling=True;Min Pool Size={minPoolSize};Max Pool Size={maxPoolSize};CharSet=utf8";

            if (connType == ConnectionType.MSSQL)
                return $"Data Source={host}; Initial Catalog = {database}; User ID = {user}; Password = {password};Pooling=True;Min Pool Size={minPoolSize};Max Pool Size={maxPoolSize}";

            return null;
        }
    }
}

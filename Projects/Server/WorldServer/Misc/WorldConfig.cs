// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using Framework.Logging;
using Framework.Logging.IO;
using Framework.Misc;
using Lappa_ORM;

namespace WorldServer.Misc
{
    class WorldConfig
    {
        static Config config;

        #region Config Options
        public static LogType LogLevel;
        public static string LogDirectory;
        public static string LogConsoleFile;
        public static string LogPacketFile;

        public static ConnectionType AuthDBType;

        public static string AuthDBHost;
        public static int AuthDBPort;
        public static string AuthDBUser;
        public static string AuthDBPassword;
        public static string AuthDBDataBase;

        public static int AuthDBMinPoolSize;
        public static int AuthDBMaxPoolSize;

        public static string BindIP;

        public static uint RealmId;
        #endregion

        public static void Initialize(string file)
        {
            config = new Config(file);

            if (config != null)
            {
                LogLevel       = (LogType)config.Read("Log.Level", 0x7, true);
                LogDirectory   = config.Read("Log.Directory", "Logs/WorldServer");
                LogConsoleFile = config.Read("Log.Console.File", "");
                LogPacketFile  = config.Read("Log.Packet.File", "");

                FileWriter fl = null;

                if (LogConsoleFile != "")
                {
                    if (!Directory.Exists(LogDirectory))
                        Directory.CreateDirectory(LogDirectory);

                    fl = new FileWriter(LogDirectory, LogConsoleFile);
                }

                Log.Initialize(LogLevel, fl);

                if (LogPacketFile != "")
                    PacketLog.Initialize(LogDirectory, LogPacketFile);
            }

            ReadConfig();
        }

        static void ReadConfig()
        {
            AuthDBType = config.Read("AuthDB.Type", ConnectionType.MYSQL);
            AuthDBHost = config.Read("AuthDB.Host", "127.0.0.1");
            AuthDBPort = config.Read("AuthDB.Port", 3306);
            AuthDBUser = config.Read("AuthDB.User", "root");
            AuthDBPassword = config.Read("AuthDB.Password", "");
            AuthDBDataBase = config.Read("AuthDB.Database", "Auth");

            AuthDBMinPoolSize = config.Read("AuthDB.MinPoolSize", 5);
            AuthDBMaxPoolSize = config.Read("AuthDB.MaxPoolSize", 30);

            BindIP = config.Read("Bind.IP", "0.0.0.0");

            RealmId = config.Read("Realm.Id", 1u);
        }
    }
}

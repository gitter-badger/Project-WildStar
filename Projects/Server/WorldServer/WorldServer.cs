// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Framework.Database;
using Framework.Database.Auth;
using Framework.Logging;
using Framework.Misc;
using Lappa_ORM;
using WorldServer.ConsoleCommands;
using WorldServer.Misc;
using WorldServer.Network;
using WorldServer.Packets;

namespace WorldServer
{
    class WorldServer
    {
        static string serverName = nameof(WorldServer);

        static void Main()
        {
            // Disable Ctrl + C.
            Console.CancelKeyPress += (o, e) => e.Cancel = true;

            WorldConfig.Initialize($"./Configs/{serverName}.conf");

            Helper.PrintHeader(serverName);

            var connString = DB.CreateConnectionString(WorldConfig.AuthDBHost, WorldConfig.AuthDBUser, WorldConfig.AuthDBPassword,
                                                       WorldConfig.AuthDBDataBase, WorldConfig.AuthDBPort, WorldConfig.AuthDBMinPoolSize,
                                                       WorldConfig.AuthDBMaxPoolSize, WorldConfig.AuthDBType);

            Log.Database("Initialize database connections...");

            if (DB.Auth.Initialize(connString, WorldConfig.AuthDBType))
            {
                var realm = DB.Auth.Single<Realm>(r => r.Id == WorldConfig.RealmId);

                if (realm == null)
                {
                    Log.Error($"Realm 'Id: {WorldConfig.RealmId}': No database info");

                    Log.Wait();

                    return;
                }

                Log.Normal($"Realm 'Id: {realm.Id}, IP: {realm.IP}, Port: {realm.Port}' loaded.");

                // Set all accounts offline.
                DB.Auth.UpdateAll<Account>(a => a.Online.Set(false));

                using (var server = new Server(WorldConfig.BindIP, realm.Port))
                {
                    PacketManager.DefineMessageHandler();

                    ConsoleCommandManager.InitCommands();
                }
            }
            else
                Log.Error("Can't connect to all databases.");

            Log.Wait();
        }
    }
}

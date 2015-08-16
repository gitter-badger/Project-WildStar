// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using AuthServer.ConsoleCommands;
using AuthServer.Misc;
using AuthServer.Network;
using AuthServer.Packets;
using Framework.Database;
using Framework.Logging;
using Framework.Misc;

namespace AuthServer
{
    class AuthServer
    {
        static string serverName = nameof(AuthServer);

        static void Main()
        {
            // Disable Ctrl + C.
            Console.CancelKeyPress += (o, e) => e.Cancel = true;

            AuthConfig.Initialize($"./Configs/{serverName}.conf");

            Helper.PrintHeader(serverName);

            var connString = DB.CreateConnectionString(AuthConfig.AuthDBHost, AuthConfig.AuthDBUser, AuthConfig.AuthDBPassword,
                                                       AuthConfig.AuthDBDataBase, AuthConfig.AuthDBPort, AuthConfig.AuthDBMinPoolSize,
                                                       AuthConfig.AuthDBMaxPoolSize, AuthConfig.AuthDBType);

            Log.Database("Initialize database connections...");

            if (DB.Auth.Initialize(connString, AuthConfig.AuthDBType))
            {
                using (var server = new Server(AuthConfig.BindIP, AuthConfig.BindPort))
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

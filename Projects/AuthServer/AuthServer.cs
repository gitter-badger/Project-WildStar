// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using AuthServer.Configuration;
using AuthServer.ConsoleCommands;
using AuthServer.Network;
using AuthServer.Network.Packets;
using Framework.Database;

namespace AuthServer
{
    class AuthServer
    {
        static void Main(string[] args)
        {
            AuthConfig.Initialize($"./Configs/{nameof(AuthServer)}.conf");

            var connString = DB.CreateConnectionString(AuthConfig.AuthDBHost, AuthConfig.AuthDBUser, AuthConfig.AuthDBPassword,
                                                       AuthConfig.AuthDBDataBase, AuthConfig.AuthDBPort, AuthConfig.AuthDBMinPoolSize,
                                                       AuthConfig.AuthDBMaxPoolSize, AuthConfig.AuthDBType);

            Console.WriteLine("Initialize database connections...");

            if (DB.Auth.Initialize(connString, AuthConfig.AuthDBType))
            {
                Console.WriteLine("Done.");

                using (var server = new Server(AuthConfig.BindIP, AuthConfig.BindPort))
                {
                    PacketManager.DefineMessageHandler();

                    // Prevents auto close.
                    ConsoleCommandManager.InitCommands();
                }
            }
            else
                Console.WriteLine("Error.");
        }
    }
}

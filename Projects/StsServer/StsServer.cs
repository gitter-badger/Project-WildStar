// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using StsServer.Configuration;
using StsServer.ConsoleCommands;
using StsServer.Network;
using StsServer.Network.Packets;
using Framework.Cryptography;
using Framework.Database;

namespace StsServer
{
    class StsServer
    {
        static void Main(string[] args)
        {
            StsConfig.Initialize($"./Configs/{nameof(StsServer)}.conf");

            var connString = DB.CreateConnectionString(StsConfig.AuthDBHost, StsConfig.AuthDBUser, StsConfig.AuthDBPassword,
                                                       StsConfig.AuthDBDataBase, StsConfig.AuthDBPort, StsConfig.AuthDBMinPoolSize,
                                                       StsConfig.AuthDBMaxPoolSize, StsConfig.AuthDBType);

            Console.WriteLine("Initialize database connections...");

            if (DB.Auth.Initialize(connString, StsConfig.AuthDBType))
            {
                Console.WriteLine("Done.");

                using (var server = new Server(StsConfig.BindIP, StsConfig.BindPort))
                {
                    StsPacketManager.DefineMessageHandler();

                    // Prevents auto close.
                    ConsoleCommandManager.InitCommands();
                }
            }
            else
                Console.WriteLine("Error.");
        }
    }
}

// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Framework.Database;
using Framework.Logging;
using Framework.Misc;
using ProxyServer.Misc;
using ProxyServer.Network;
using ProxyServer.Packets;

namespace ProxyServer
{
    class ProxyServer
    {
        static string serverName = nameof(ProxyServer);

        static void Main()
        {
            // Disable Ctrl + C.
            Console.CancelKeyPress += (o, e) => e.Cancel = true;

            ProxyConfig.Initialize($"./Configs/{serverName}.conf");

            Helper.PrintHeader(serverName);

            var connString = DB.CreateConnectionString(ProxyConfig.AuthDBHost, ProxyConfig.AuthDBUser, ProxyConfig.AuthDBPassword,
                                                       ProxyConfig.AuthDBDataBase, ProxyConfig.AuthDBPort, ProxyConfig.AuthDBMinPoolSize,
                                                       ProxyConfig.AuthDBMaxPoolSize, ProxyConfig.AuthDBType);

            Log.Database("Initialize database connections...");

            if (DB.Auth.Initialize(connString, ProxyConfig.AuthDBType))
            {
                using (var server = new Server(ProxyConfig.BindIP, 23115))
                {
                    PacketManager.DefineMessageHandler();

                    Log.Normal("Done.");

                    while (true)
                        Thread.Sleep(5);
                }
            }
            else
                Log.Error("Can't connect to all databases.");

            Log.Wait();
        }
    }
}

// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Threading;
using AuthServer.Network;
using AuthServer.Network.Packets;
using Framework.Logging;

namespace AuthServer
{
    class AuthServer
    {
        static void Main(string[] args)
        {
            Directory.CreateDirectory("./Logs/AuthServer/");

            PacketLog.Initialize("./Logs/AuthServer/", "Packets.log");
            PacketManager.DefineMessageHandler();

            using (var server = new Server("0.0.0.0", 6600))
            {
                Console.WriteLine("Started...");

                while (true)
                    Thread.Sleep(1);
            }
        }
    }
}

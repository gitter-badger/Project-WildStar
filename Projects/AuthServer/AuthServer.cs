// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
            PacketLog.Initialize("Logs/AuthServer", "Packets.log");

            using (var server = new Server("0.0.0.0", 23115))
            {
                PacketManager.DefineMessageHandler();

                while (true)
                    Thread.Sleep(1);
            }
        }
    }
}

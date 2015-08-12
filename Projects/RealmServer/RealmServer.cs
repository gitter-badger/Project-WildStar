// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Framework.Logging;
using RealmServer.Network;
using RealmServer.Network.Packets;

namespace RealmServer
{
    class RealmServer
    {
        static void Main(string[] args)
        {
            PacketLog.Initialize("Logs/RealmServer", "Packets.log");

            using (var server = new Server("0.0.0.0", 24000))
            {
                PacketManager.DefineMessageHandler();

                while (true)
                    Thread.Sleep(1);
            }
        }
    }
}

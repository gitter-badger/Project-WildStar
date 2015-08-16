// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Framework.Packets
{
    public abstract class ClientPacket
    {
        public Packet Packet { protected get; set; }
        public bool IsReadComplete { get { return Packet.IsReadComplete; } }

        public abstract void Read();
    }
}

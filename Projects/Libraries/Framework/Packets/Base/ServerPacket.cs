// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Framework.Packets
{
    public abstract class ServerPacket
    {
        public Packet Packet { get; private set; }

        protected ServerPacket()
        {
            Packet = new Packet();
        }

        protected ServerPacket(object netMessage)
        {
            Packet = new Packet(netMessage);
        }

        public abstract void Write();
    }
}

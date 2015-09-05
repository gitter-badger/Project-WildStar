// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Framework.Packets.Client.Misc
{
    public class State1 : ClientPacket
    {
        public byte[] State { get; private set; }

        public override void Read()
        {
            // Just finish the reader.
            Packet.Read<ushort>(16);

            State = Packet.Data;
        }
    }
}

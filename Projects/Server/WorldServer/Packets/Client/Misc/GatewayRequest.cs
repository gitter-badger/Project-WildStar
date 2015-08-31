// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Packets;

namespace WorldServer.Packets.Client.Misc
{
    class GatewayRequest : ClientPacket
    {
        public byte[] GatewayTicket { get; set; }
        public string LoginName     { get; set; }

        public override void Read()
        {
            Packet.Read<uint>(32);

            GatewayTicket = Packet.Read(16);
            LoginName     = Packet.ReadWString();

            Packet.Read<uint>(32);
        }
    }
}

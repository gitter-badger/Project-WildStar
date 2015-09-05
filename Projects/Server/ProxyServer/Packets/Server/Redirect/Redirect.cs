// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Packets;
using ProxyServer.Constants.Net;

namespace ProxyServer.Packets.Server.Redirect
{
    class Redirect : ServerPacket
    {
        public uint IP              { get; set; }
        public ushort Port          { get; set; }
        public byte[] GatewayTicket { get; set; }
        public string RealmName     { get; set; }

        public Redirect() : base(ServerMessage.Redirect) { }

        public override void Write()
        {
            Packet.Write(IP, 32);
            Packet.Write(Port, 16);
            Packet.Write(GatewayTicket);
            Packet.Write(0, 32);
            Packet.WriteWString(RealmName);
            Packet.Write(0, 2);
            Packet.Write(0, 21);
        }
    }
}

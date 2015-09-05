// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Packets;

namespace ProxyServer.Packets.Client.Redirect
{
    class RedirectRequest : ClientPacket
    {
        public uint BuildNumber { get; set; }
        public string LoginName { get; set; }

        public override void Read()
        {
            BuildNumber = Packet.Read<uint>(32);

            Packet.Read<ulong>(64);

            LoginName = Packet.ReadString();

            // We don't need these fields for now...
            Packet.Read<uint>(32);
            Packet.Read<ushort>(16);
            Packet.Read<ushort>(16);
            Packet.Read<uint>(64);

            Packet.Read<uint>(32);
            Packet.Read<ushort>(16);
            Packet.Read<ushort>(16);
            Packet.Read<uint>(64);

            Packet.Read<uint>(32);
            Packet.Read<uint>(32);
            Packet.Read<uint>(32);
            Packet.Read<uint>(32);

            // CPU Info
            Packet.ReadWString();
            Packet.ReadWString();
            Packet.ReadWString();
            Packet.Read<uint>(32);
            Packet.Read<uint>(32);
            Packet.Read<uint>(32);
            Packet.Read<uint>(32);
            Packet.Read<uint>(32);

            Packet.Read<uint>(32);

            // Graphics Info
            Packet.ReadWString();
            Packet.Read<uint>(32);
            Packet.Read<uint>(32);
            Packet.Read<uint>(32);
            Packet.Read<uint>(32);
            Packet.Read<uint>(32);

            Packet.Read<uint>(32);
            Packet.Read<uint>(32);
            Packet.Read<uint>(32);
            Packet.Read<uint>(32);
        }
    }
}

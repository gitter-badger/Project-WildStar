// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Constants.Net;

namespace Framework.Packets.Server.Misc
{
    public class SHello : ServerPacket
    {
        public uint BuildNumber       { get; set; }
        public uint RealmId           { get; set; }
        public uint RealmGroupId      { get; set; }
        public ulong StartupTime      { get; set; }
        public ushort ListenPort      { get; set; }
        public ushort ConnectionType  { get; set; }
        public uint NetworkMessageCRC { get; set; }

        public SHello() : base(GlobalServerMessage.SHello) { }

        public override void Write()
        {
            Packet.Write(BuildNumber, 32);
            Packet.Write(RealmId, 32);
            Packet.Write(RealmGroupId, 32);
            Packet.Write(0, 32);
            Packet.Write(StartupTime, 64);
            Packet.Write(ListenPort, 16);
            Packet.Write(ConnectionType, 5);
            Packet.Write(NetworkMessageCRC, 32);
            Packet.Write(0, 32);
            Packet.Write(0, 64);
            Packet.Write(0, 32);
        }
    }
}

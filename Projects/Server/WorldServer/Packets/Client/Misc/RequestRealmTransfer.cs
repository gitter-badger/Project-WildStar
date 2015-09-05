// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Packets;

namespace WorldServer.Packets.Client.Misc
{
    class RequestRealmTransfer : ClientPacket
    {
        public uint RealmId { get; set; }

        public override void Read()
        {
            RealmId = Packet.Read<uint>(32);
        }
    }
}

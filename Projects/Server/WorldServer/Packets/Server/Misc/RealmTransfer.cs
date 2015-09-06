// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Packets;
using WorldServer.Constants.Net;

namespace WorldServer.Packets.Server.Misc
{
    class RealmTransfer : ServerPacket
    {
        public uint RealmTransferFlags     { get; set; }
        public bool SubscriptionExpired    { get; set; }
        public uint GameTimeHoursRemaining { get; set; }

        public RealmTransfer() : base(ServerMessage.RealmTransfer) { }

        public override void Write()
        {
            Packet.Write(RealmTransferFlags, 32);
            Packet.Write(SubscriptionExpired, 1);
            Packet.Write(GameTimeHoursRemaining, 32);
        }
    }
}

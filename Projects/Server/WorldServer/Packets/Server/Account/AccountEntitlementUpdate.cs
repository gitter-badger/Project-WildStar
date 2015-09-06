// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Framework.Constants.Account;
using Framework.Packets;
using WorldServer.Constants.Net;

namespace WorldServer.Packets.Server.Account
{
    class AccountEntitlementUpdate : ServerPacket
    {
        public Dictionary<Entitlement, uint> Entitlements { get; set; } = new Dictionary<Entitlement, uint>();

        public AccountEntitlementUpdate() : base(ServerMessage.AccountEntitlementUpdate) { }

        public override void Write()
        {
            Packet.Write(Entitlements.Count, 32);

            foreach (var e in Entitlements)
            {
                Packet.Write(e.Key, 32);
                Packet.Write(e.Value, 32);
            }
        }
    }
}

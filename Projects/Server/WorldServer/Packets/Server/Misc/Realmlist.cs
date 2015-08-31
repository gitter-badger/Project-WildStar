// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Framework.Packets;
using WorldServer.Constants.Net;
using WorldServer.Packets.Structures.Realm;

namespace WorldServer.Packets.Server.Misc
{
    class RealmList : ServerPacket
    {
        public List<RealmListEntry> Realms { get; set; } = new List<RealmListEntry>();
        public List<RealmMessageEntry> RealmMessages { get; set; } = new List<RealmMessageEntry>();

        public RealmList() : base(ServerMessage.Realmlist) { }

        public override void Write()
        {
            Packet.Write(0, 64);
            Packet.Write(Realms.Count, 32);

            Realms.ForEach(r => r.Write(Packet));

            Packet.Write(RealmMessages.Count, 32);

            RealmMessages.ForEach(rm => rm.Write(Packet));
        }
    }
}

// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Framework.Packets;

namespace WorldServer.Packets.Structures.Realm
{
    class RealmMessageEntry : IServerStruct
    {
        public uint RealmId          { get; set; }
        public List<string> Messages { get; set; } = new List<string>();

        public void Write(Packet packet)
        {
            packet.Write(RealmId, 32);
            packet.Write(Messages.Count, 8);

            Messages.ForEach(m => packet.WriteWString(m));
        }
    }
}

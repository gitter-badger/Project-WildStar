// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Constants.Realm;
using Framework.Packets;

namespace WorldServer.Packets.Structures.Realm
{
    class RealmListEntry : IServerStruct
    {
        public uint Id                    { get; set; }
        public string Name                { get; set; }
        public RealmType Type             { get; set; }
        public RealmStatus Status         { get; set; }
        public RealmPopulation Population { get; set; }


        public void Write(Packet packet)
        {
            packet.Write(Id, 32);
            packet.WriteWString(Name);
            packet.Write(0, 32);
            packet.Write(0, 32);

            packet.Write(Type, 2);
            packet.Write(Status, 3);
            packet.Write(Population, 3);

            packet.Write(0, 32);

            packet.Write(0, 64);
            packet.Write(0, 64);

            packet.Write(0, 14);
            packet.Write(0, 32);
            packet.WriteWString("");
            packet.Write(0, 64);

            packet.Write(0, 16);
            packet.Write(0, 16);
            packet.Write(0, 16);
            packet.Write(0, 16);
        }
    }
}

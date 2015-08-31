// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Packets;
using WorldServer.Constants.Net;

namespace WorldServer.Packets.Server.Character
{
    class CharacterList : ServerPacket
    {
        public CharacterList() : base(ServerMessage.CharacterList) { }

        // Just write an empty character list for now.
        public override void Write()
        {
            Packet.Write(0, 64);
            Packet.Write(0, 32);

            Packet.Write(0, 32);
            Packet.Write(0, 32);
            Packet.Write(0, 14);
            Packet.Write(0, 14);
            Packet.Write(0, 64);
            Packet.Write(0, 32);
            Packet.Write(0, 32);
            Packet.Write(0, 32);
            Packet.Write(0, 32);
        }
    }
}

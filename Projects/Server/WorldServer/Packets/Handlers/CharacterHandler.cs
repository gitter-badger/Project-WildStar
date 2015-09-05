// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using WorldServer.Attributes;
using WorldServer.Constants.Net;
using WorldServer.Network;
using WorldServer.Packets.Client.Character;

namespace WorldServer.Packets.Handlers
{
    class CharacterHandler
    {
        [NetMessage(ClientMessage.CharacterList)]
        public static async void HandleCharacterList(CharacterList characterList, Session session)
        {
            // Empty character list.
            await session.Send(new Server.Character.CharacterList());
        }
    }
}

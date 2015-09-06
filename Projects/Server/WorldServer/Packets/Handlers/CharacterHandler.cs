// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Constants.Account;
using WorldServer.Attributes;
using WorldServer.Constants.Net;
using WorldServer.Network;
using WorldServer.Packets.Client.Character;
using WorldServer.Packets.Server.Account;

namespace WorldServer.Packets.Handlers
{
    class CharacterHandler
    {
        [NetMessage(ClientMessage.CharacterList)]
        public static async void HandleCharacterList(CharacterList characterList, Session session)
        {
            var accountEntitlementUpdate = new AccountEntitlementUpdate();

            session.Account.Entitlements.ForEach(e => accountEntitlementUpdate.Entitlements.Add((Entitlement)e.Id, e.Value));

            await session.Send(accountEntitlementUpdate);

            // Empty character list for now.
            await session.Send(new Server.Character.CharacterList());
        }
    }
}

// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AuthServer.Attributes;
using AuthServer.Constants.Net;

namespace AuthServer.Network.Packets.Handlers
{
    class GameAccountHandler
    {
        [AuthMessage(AuthMessage.ListMyAccounts)]
        public static void HandleGameAccountListMyAccounts(AuthPacket packet, AuthSession session)
        {
            var userId = uint.Parse(packet["UserId"].ToString());

            if (userId == session.Account.Id)
            {
                // ...
            }
        }
    }
}

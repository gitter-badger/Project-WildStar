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
                var reply = new AuthPacket(AuthReason.OK, packet.Header.Sequence);
                var xmlData = new XmlData();

                // Only 1 GameAccount supported for now.
                xmlData.WriteElementRoot("Reply");
                xmlData.WriteCustom("<GameAccount>\n");
                xmlData.WriteElement("Alias", $"{session.Account.GameAccounts[0].Alias}");
                xmlData.WriteElement("Created", "");
                xmlData.WriteCustom("</GameAccount>\n");

                reply.WriteXmlData(xmlData);

                session.Send(reply);
            }
        }
    }
}

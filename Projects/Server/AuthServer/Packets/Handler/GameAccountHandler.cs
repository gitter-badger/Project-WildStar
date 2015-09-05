// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AuthServer.Attributes;
using AuthServer.Constants.Net;
using AuthServer.Network;

namespace AuthServer.Packets.Handler
{
    class GameAccountHandler
    {
        [AuthMessage(StsMessage.ListMyAccounts)]
        public static void HandleGameAccountListMyAccounts(Packet packet, Session session)
        {
            var userId = uint.Parse(packet["UserId"].ToString());

            if (userId == session.Account.Id)
            {
                var reply = new Packet(StsReason.OK, packet.Header.Sequence);

                // Only 1 GameAccount supported for now.
                reply.Xml.WriteElementRoot("Reply");
                reply.Xml.WriteCustom("<GameAccount>\n");
                reply.Xml.WriteElement("Alias", $"{session.Account.Alias}");
                reply.Xml.WriteElement("Created", "");
                reply.Xml.WriteCustom("</GameAccount>\n");

                session.Send(reply);
            }
        }
    }
}

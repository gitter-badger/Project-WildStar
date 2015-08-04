// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using StsServer.Attributes;
using StsServer.Constants.Net;

namespace StsServer.Network.Packets.Handlers
{
    class GameAccountHandler
    {
        [StsMessage(StsMessage.ListMyAccounts)]
        public static void HandleGameAccountListMyAccounts(StsPacket packet, StsSession session)
        {
            var userId = uint.Parse(packet["UserId"].ToString());

            if (userId == session.Account.Id)
            {
                var reply = new StsPacket(StsReason.OK, packet.Header.Sequence);
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

// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Database;
using Framework.Database.Auth;
using Framework.Logging;
using WorldServer.Attributes;
using WorldServer.Constants.Net;
using WorldServer.Network;
using WorldServer.Packets.Client.Misc;
using WorldServer.Packets.Server.Misc;
using WorldServer.Packets.Structures.Realm;

namespace WorldServer.Packets.Handlers
{
    class MiscHandler
    {
        [NetMessage(ClientMessage.GatewayRequest)]
        public static void HandleGatewayRequest(GatewayRequest gatewayRequest, Session session)
        {
            Log.Debug($"Got new connection for Account '{gatewayRequest.LoginName}'.");

            session.Account = DB.Auth.Single<Account>(a => a.Email == gatewayRequest.LoginName && a.GatewayTicket != "");

            if (session.Account != null)
                session.Crypt = new Framework.Cryptography.PacketCrypt(gatewayRequest.GatewayTicket);
            else
            {
                Log.Debug($"Connection for Account '{gatewayRequest.LoginName}' can't be validated.");

                session.Dispose();
            }
        }

        [NetMessage(ClientMessage.RealmListChanged)]
        public static async void HandleRealmListChanged(RealmListChanged realmListChanged, Session session)
        {
            var realms = DB.Auth.Select<Realm>();
            var realmList = new RealmList();

            realms.ForEach(r =>
            {
                realmList.Realms.Add(new RealmListEntry
                {
                    Id         = r.Id,
                    Name       = r.Name,
                    Type       = r.Type,
                    Status     = r.Status,
                    Population = r.Population
                });
            });

            await session.Send(realmList);
        }
    }
}

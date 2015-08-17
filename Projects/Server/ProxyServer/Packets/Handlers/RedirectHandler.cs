// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Net;
using Framework.Constants.Realm;
using Framework.Database;
using Framework.Database.Auth;
using Framework.Misc;
using Framework.Misc.Extensions;
using Lappa_ORM;
using ProxyServer.Attributes;
using ProxyServer.Constants.Net;
using ProxyServer.Network;
using ProxyServer.Packets.Client.Redirect;
using ProxyServer.Packets.Server.Redirect;

namespace ProxyServer.Packets.Handlers
{
    class RedirectHandler
    {
        [NetMessage(ClientMessage.RedirectRequest)]
        public static async void HandleRedirectRequest(RedirectRequest redirectRequest, Session session)
        {
            var account = DB.Auth.Single<Account>(a => a.Email == redirectRequest.LoginName);

            if (account != null && DB.Auth.Any<Framework.Database.Auth.Redirect>(r => r.AccountId == account.Id && r.IP == session.GetIPAddress()))
            {
                // Delete the redirect state from database.
                DB.Auth.Delete<Framework.Database.Auth.Redirect>(r => r.AccountId == account.Id && r.IP == session.GetIPAddress());

                // Get the default realm.
                var realm = DB.Auth.Single<Realm>(r => r.Index == 0);

                if (realm != null)
                {
                    // Generate gateway ticket
                    var gatewayTicket = Helper.GenerateRandomKey(16);

                    account.GatewayTicket = gatewayTicket.ToHexString();

                    if (DB.Auth.Update(account))
                    {
                        await session.Send(new RedirectResponse { Result = 0 });

                        var redirect = new Server.Redirect.Redirect
                        {
                            IP            = BitConverter.ToUInt32(IPAddress.Parse(realm.IP).GetAddressBytes().Reverse().ToArray(), 0),
                            Port          = realm.Port,
                            GatewayTicket = gatewayTicket,
                            RealmName     = realm.Name
                        };

                        await session.Send(redirect);

                        return;
                    }
                }
            }

            // Close the connection for now.
            session.Dispose();
        }
    }
}

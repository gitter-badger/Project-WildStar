// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using AuthServer.Attributes;
using AuthServer.Constants.Net;
using Framework.Database;
using Framework.Database.Auth;

namespace AuthServer.Network.Packets.Handler
{
    class AuthHandler
    {
        [AuthPacket(ClientMessage.State1)]
        public static void HandleState1(Packet packet, AuthSession session)
        {
            // Send same data back for now.
            session.SendRaw(packet.Data);
        }

        [AuthPacket(ClientMessage.State2)]
        public static void HandleState2(Packet packet, AuthSession session)
        {
            // Send same data back for now.
            session.SendRaw(packet.Data);
        }

        [AuthPacket(ClientMessage.AuthRequest)]
        public static void HandleAuthRequest(Packet packet, AuthSession session)
        {
            packet.Read<uint>(32);
            packet.Read<ulong>(64);

            var loginName = packet.ReadString();

            Console.WriteLine($"Account '{loginName}' tries to connect.");

            //var account = DB.Auth.Single<Account>(a => a.Email == loginName);

            //if (account != null && account.Online)
            {
                var authComplete = new Packet(ServerMessage.AuthComplete);

                authComplete.Write(0, 32);

                session.Send(authComplete);

                var connectToRealm = new Packet(ServerMessage.ConnectToRealm);

                // Data...

                session.Send(connectToRealm);
            }
        }
    }
}

// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using AuthServer.Attributes;
using AuthServer.Constants.Account;
using AuthServer.Constants.Net;
using AuthServer.Cryptography;
using AuthServer.Network;
using Framework.Database;
using Framework.Database.Auth;
using Framework.Misc.Extensions;
using Lappa_ORM;

namespace AuthServer.Packets.Handler
{
    class AuthHandler
    {
        [AuthMessage(StsMessage.LoginStart)]
        public static void HandleAuthLoginStart(Packet packet, Session session)
        {
            // Account.Email
            var loginName = packet["LoginName"].ToString();

            session.Account = DB.Auth.Single<Account>(a => a.Email == loginName);

            // Support for email only.
            if (loginName != null && session.Account != null)
            {
                session.SecureRemotePassword = new SRP6a(session.Account.Salt, loginName, session.Account.PasswordVerifier);
                session.SecureRemotePassword.CalculateB();

                var keyData = new BinaryWriter(new MemoryStream());

                keyData.Write(session.SecureRemotePassword.S.Length);
                keyData.Write(session.SecureRemotePassword.S);
                keyData.Write(session.SecureRemotePassword.B.Length);
                keyData.Write(session.SecureRemotePassword.B);

                var reply = new Packet(StsReason.OK, packet.Header.Sequence);

                reply.Xml.WriteElementRoot("Reply");
                reply.Xml.WriteElement("KeyData", Convert.ToBase64String(keyData.ToArray()));

                session.Send(reply);
            }
            else
            {
                // Let's use ErrBadPasswd instead of ErrAccountNotFound.
                var reply = new Packet(StsReason.Error, packet.Header.Sequence);

                reply.WriteError(AuthResult.ErrBadPasswd);

                session.Send(reply);
            }
        }

        [AuthMessage(StsMessage.KeyData)]
        public static void HandleAuthKeyData(Packet packet, Session session)
        {
            var keyData = new BinaryReader(new MemoryStream(Convert.FromBase64String(packet["KeyData"].ToString())));
            var a = keyData.ReadBytes(keyData.ReadInt32());
            var m = keyData.ReadBytes(keyData.ReadInt32());

            session.SecureRemotePassword.CalculateU(a);
            session.SecureRemotePassword.CalculateClientM(a);

            if (session.SecureRemotePassword.ClientM.Compare(m))
            {
                session.SecureRemotePassword.CalculateServerM(m, a);

                session.ClientCrypt = new RC4();
                session.ClientCrypt.PrepareKey(session.SecureRemotePassword.SessionKey);

                session.State = 1;

                var SKeyData = new BinaryWriter(new MemoryStream());

                SKeyData.Write(session.SecureRemotePassword.ServerM.Length);
                SKeyData.Write(session.SecureRemotePassword.ServerM);

                var reply = new Packet(StsReason.OK, packet.Header.Sequence);

                reply.Xml.WriteElementRoot("Reply");
                reply.Xml.WriteElement("KeyData", Convert.ToBase64String(SKeyData.ToArray()));

                session.Send(reply);
            }
            else
            {
                session.Account = null;

                var reply = new Packet(StsReason.Error, packet.Header.Sequence);

                reply.WriteError(AuthResult.ErrBadPasswd);

                session.Send(reply);
            }
        }

        [AuthMessage(StsMessage.LoginFinish)]
        public static void HandleAuthLoginFinish(Packet packet, Session session)
        {
            // Server packets are encrypted now.
            session.ServerCrypt = new RC4();
            session.ServerCrypt.PrepareKey(session.SecureRemotePassword.SessionKey);

            var reply = new Packet(StsReason.OK, packet.Header.Sequence);

            reply.Xml.WriteElementRoot("Reply");

            reply.Xml.WriteElement("LocationId", "");
            reply.Xml.WriteElement("UserId", session.Account.Id.ToString());
            reply.Xml.WriteElement("UserCenter", "");
            reply.Xml.WriteElement("UserName", session.Account.Email);
            reply.Xml.WriteElement("AccessMask", "");
            reply.Xml.WriteElement("Roles", "");
            reply.Xml.WriteElement("Status", "");

            session.Send(reply);
        }

        [AuthMessage(StsMessage.RequestGameToken)]
        public static void HandleAuthRequestGameToken(Packet packet, Session session)
        {
            DB.Auth.Delete<Redirect>(r => r.AccountId == session.Account.Id);

            var redirectData = new Redirect
            {
                AccountId = session.Account.Id,
                IP = session.GetIPAddress()
            };

            if (DB.Auth.Add(redirectData))
            {
                var reply = new Packet(StsReason.OK, packet.Header.Sequence);

                reply.Xml.WriteElementRoot("Reply");

                reply.Xml.WriteElement("Token", "");

                session.Send(reply);
            }
            else
                session.Dispose();
        }
    }
}

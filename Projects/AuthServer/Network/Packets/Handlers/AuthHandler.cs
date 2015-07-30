// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using AuthServer.Attributes;
using AuthServer.Constants.Net;
using Framework.Cryptography;
using Framework.Cryptography.BNet;
using Framework.Misc;

namespace AuthServer.Network.Packets.Handlers
{
    class AuthHandler
    {
        [AuthMessage(Constants.Net.AuthMessage.LoginStart)]
        public static void HandleAuthLoginStart(AuthPacket packet, AuthSession session)
        {
            session.SecureRemotePassword = new SRP6a(Helper.GenerateRandomKey(8).ToHexString());
            session.SecureRemotePassword.CalculateX("fabian@arctium.org", "test", true);

            var keyData = new BinaryWriter(new MemoryStream());

            keyData.Write(session.SecureRemotePassword.S.Length);
            keyData.Write(session.SecureRemotePassword.S);
            keyData.Write(session.SecureRemotePassword.B.Length);
            keyData.Write(session.SecureRemotePassword.B);

            var xmlData = new XmlData();

            xmlData.WriteElementRoot("Reply");
            xmlData.WriteElement("KeyData", Convert.ToBase64String(keyData.ToArray()));

            var sts = new StsPacket();
            var reply = new AuthPacket();

            reply.WriteXmlData(xmlData);
            sts.WriteData(reply.Data.Length, packet.Header.Sequence);

            session.Send(sts);
            session.Send(reply);
        }

        [AuthMessage(Constants.Net.AuthMessage.KeyData)]
        public static void HandleAuthKeyData(AuthPacket packet, AuthSession session)
        {
            var keyData = new BinaryReader(new MemoryStream(Convert.FromBase64String(packet.Values["KeyData"].ToString())));
            var a = keyData.ReadBytes(keyData.ReadInt32());
            var m = keyData.ReadBytes(keyData.ReadInt32());

            session.SecureRemotePassword.CalculateU(a);
            session.SecureRemotePassword.CalculateClientM(a);

            if (session.SecureRemotePassword.ClientM.Compare(m))
            {
                session.SecureRemotePassword.CalculateServerM(m, a);

                session.ClientCrypt = new SARC4();
                session.ClientCrypt.PrepareKey(session.SecureRemotePassword.SessionKey);

                session.State = 1;

                var SKeyData = new BinaryWriter(new MemoryStream());

                SKeyData.Write(session.SecureRemotePassword.ServerM.Length);
                SKeyData.Write(session.SecureRemotePassword.ServerM);

                var xmlData = new XmlData();

                xmlData.WriteElementRoot("Reply");
                xmlData.WriteElement("KeyData", Convert.ToBase64String(SKeyData.ToArray()));

                var sts = new StsPacket();
                var reply = new AuthPacket();

                reply.WriteXmlData(xmlData);
                sts.WriteData(reply.Data.Length, packet.Header.Sequence);

                session.Send(sts);
                session.Send(reply);

                session.ServerCrypt = new SARC4();
                session.ServerCrypt.PrepareKey(session.SecureRemotePassword.SessionKey);
            }
            else
            {
                var sts = new StsPacket(StsReason.ErrBadPasswd);
                var reply = new AuthPacket();

                reply.WriteString("<Error code=\"11\" server=\"0\" module=\"0\" line=\"0\"/>\n");

                sts.WriteData(reply.Data.Length, packet.Header.Sequence);

                session.Send(sts);
                session.Send(reply);
            }
        }
    }
}

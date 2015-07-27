// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AuthServer.Constants.Net;
using AuthServer.Network.Packets.Headers;

namespace AuthServer.Network.Packets
{
    class AuthPacket : PacketBase
    {
        BinaryReader readStream;
        //BinaryWriter writeStream;

        public AuthPacket(byte[] data)
        {
            readStream = new BinaryReader(new MemoryStream(data));

            Data = data;
            Values = new Dictionary<string, object>();
        }

        public override void ReadHeader(Tuple<string, string[], int> headerInfo)
        {
            if (headerInfo.Item2.Length >= 4)
            {
                var identifier = headerInfo.Item2[0].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                if (identifier.Length == 3)
                {
                    var msgString = identifier[1].Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    var sequence = Convert.ToByte(headerInfo.Item2[1].Remove(0, 2));
                    var length = Convert.ToUInt16(headerInfo.Item2[3].Remove(0, 2));

                    AuthMessage msg;

                    if (!Enum.TryParse(msgString, out msg))
                        msg = AuthMessage.Unknown;

                    Header = new AuthHeader
                    {
                        Message    = msg,
                        Length     = (ushort)headerInfo.Item3,
                        DataLength = length,
                        Sequence = sequence
                    };
                }
            }
        }

        public override void ReadData()
        {
            var xml = XDocument.Load(new MemoryStream(Data));
            var header = Header as StsHeader;
            var elementList = xml.Elements().ToList();

            if (elementList.Elements().Count() > 0)
                elementList = xml.Element(elementList[0].Name).Elements().ToList();

            for (var i = 0; i < elementList.Count; i++)
            {
                Values.Add(elementList[i].Name.LocalName, elementList[i].Value);
            }
        }
    }
}

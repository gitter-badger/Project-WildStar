// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using AuthServer.Constants.Net;
using AuthServer.Network.Packets.Headers;

namespace AuthServer.Network.Packets
{
    class StsPacket : PacketBase
    {
        BinaryReader readStream;
        BinaryWriter writeStream;

        public StsPacket(StsReason reason = StsReason.OK)
        {
            writeStream = new BinaryWriter(new MemoryStream());

            WriteStringLine($"STS/1.0 {(int)reason} {reason.ToString()}");
        }

        public StsPacket(byte[] data)
        {
            readStream = new BinaryReader(new MemoryStream(data));

            Data = data;
            Values = new Dictionary<string, object>();
        }

        public void WriteStringLine(string line)
        {
            writeStream.Write(Encoding.UTF8.GetBytes(line));
            writeStream.Write(new byte[] { 0x0D, 0x0A });
        }

        public void WriteData(int length, int sequence)
        {
            WriteStringLine($"l:{length}");
            WriteStringLine($"s:{sequence}R");

            writeStream.Write(new byte[] { 0x0D, 0x0A });

            Finish();
        }

        public void Finish()
        {
            Data = (writeStream.BaseStream as MemoryStream).ToArray();
        }

        public override void ReadHeader(Tuple<string, string[], int> headerInfo)
        {
            if (headerInfo.Item2.Length >= 2)
            {
                var identifier = headerInfo.Item2[0].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                if (identifier.Length == 3)
                {
                    var msgString = identifier[1].Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    var length = Convert.ToUInt16(headerInfo.Item2[1].Remove(0, 2));

                    StsMessage msg;

                    if (!Enum.TryParse(msgString, out msg))
                        msg = StsMessage.Unknown;

                    Header = new StsHeader
                    {
                        Message    = msg,
                        Length     = (ushort)headerInfo.Item3,
                        DataLength = length,
                        Sequence   = 0
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

// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using AuthServer.Constants.Account;
using AuthServer.Constants.Net;

namespace AuthServer.Packets
{
    class Packet
    {
        public object this[string elementName]
        {
            get
            {
                object ret;

                if (values.TryGetValue(elementName, out ret))
                    return ret;

                return null;
            }
        }

        public PacketHeader Header { get; set; }
        public XmlData Xml { get; }
        public byte[] Data { get; set; }

        BinaryReader readStream;
        BinaryWriter stsWriter, xmlWriter;
        Dictionary<string, object> values;

        public Packet(StsReason reason = StsReason.OK, int sequence = 0)
        {
            stsWriter = new BinaryWriter(new MemoryStream());
            xmlWriter = new BinaryWriter(new MemoryStream());

            WriteStringLine($"STS/1.0 {(int)reason} {reason.ToString()}");

            Header = new PacketHeader
            {
                Sequence = (byte)sequence
            };

            Xml = new XmlData();
        }

        public Packet(byte[] data)
        {
            readStream = new BinaryReader(new MemoryStream(data));

            Data = data;
            values = new Dictionary<string, object>();
        }

        public void WriteXmlData()
        {
            xmlWriter.Write(Encoding.UTF8.GetBytes(Xml.ToString()));
            xmlWriter.Write(new byte[] { 0x0A });
        }

        public void WriteString(string data)
        {
            xmlWriter.Write(Encoding.UTF8.GetBytes(data));
        }

        void WriteStringLine(string line)
        {
            stsWriter.Write(Encoding.UTF8.GetBytes(line));
            stsWriter.Write(new byte[] { 0x0D, 0x0A });
        }

        void WriteHeader(int length, int sequence)
        {
            WriteStringLine($"l:{length}");
            WriteStringLine($"s:{sequence}R");

            stsWriter.Write(new byte[] { 0x0D, 0x0A });
        }

        public void WriteError(AuthResult errorCode, int server = 0, int module = 0, int line = 0)
        {
            WriteString($"<Error code=\"{(int)errorCode}\" server=\"{server}\" module=\"{module}\" line=\"{line}\"/>\n");
        }

        public void Finish()
        {
            WriteHeader((int)xmlWriter.BaseStream.Length, Header.Sequence);

            stsWriter.Write((xmlWriter.BaseStream as MemoryStream).ToArray());

            Data = (stsWriter.BaseStream as MemoryStream).ToArray();
        }

        public void ReadHeader(Tuple<string, string[], int> headerInfo)
        {
            if (headerInfo.Item2.Length >= 2)
            {
                var identifier = headerInfo.Item2[0].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                if (identifier.Length == 3)
                {
                    var msgString = identifier[1].Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    byte sequence = 0;
                    ushort length = 0;

                    if (headerInfo.Item2.Length >= 4)
                    {
                        sequence = Convert.ToByte(headerInfo.Item2[1].Remove(0, 2));
                        length = Convert.ToUInt16(headerInfo.Item2[3].Remove(0, 2));
                    }
                    else if (headerInfo.Item2.Length >= 2)
                        length = Convert.ToUInt16(headerInfo.Item2[1].Remove(0, 2));

                    StsMessage msg;

                    if (!Enum.TryParse(msgString, out msg))
                        msg = StsMessage.Unknown;

                    Header = new PacketHeader
                    {
                        Message    = msg,
                        Length     = (ushort)headerInfo.Item3,
                        DataLength = length,
                        Sequence   = sequence
                    };
                }
            }
        }

        public void ReadData(byte[] xmlData)
        {
            var xml = XDocument.Load(new MemoryStream(xmlData));
            var elementList = xml.Elements().ToList();

            if (elementList.Elements().Count() > 0)
                elementList = xml.Element(elementList[0].Name).Elements().ToList();

            for (var i = 0; i < elementList.Count; i++)
                values.Add(elementList[i].Name.LocalName, elementList[i].Value);
        }
    }
}

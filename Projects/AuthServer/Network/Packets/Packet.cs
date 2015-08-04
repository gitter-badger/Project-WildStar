// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using AuthServer.Constants.Net;
using Framework.Misc;

namespace AuthServer.Network.Packets
{
    class Packet
    {
        public PacketHeader Header { get; set; }
        public byte[] Data { get; set; }

        BinaryReader readStream;
        BinaryWriter writeStream;

        int shiftedBits = 0;
        byte packedByte = 0;

        public Packet(ServerMessage message)
        {
            writeStream = new BinaryWriter(new MemoryStream());

            Header = new PacketHeader { Message = (ushort)message };
        }

        public Packet(byte[] data)
        {
            if (data.Length >= 4)
            {
                var offset = 0;

                readStream = new BinaryReader(new MemoryStream(data));

                Header = new PacketHeader
                {
                    Size    = Read<uint>(32),
                    Message = Read<ushort>(12)
                };

                Flush();

                if (Header.Message == (ushort)ClientMessage.MultiPacket)
                {
                    Header.Size = Read<uint>(24);

                    Flush();

                    offset = (int)readStream.BaseStream.Position;

                    Data = new byte[Header.Size - 4];
                    Buffer.BlockCopy(data, offset, Data, 0, (int)Header.Size - 4);
                }
                else
                {
                    Data = new byte[Header.Size];
                    Buffer.BlockCopy(data, offset, Data, 0, (int)Header.Size);
                }
            }
        }

        public void Finish()
        {
        }

        public void ReAssign()
        {
            readStream = new BinaryReader(new MemoryStream(Data));
        }

        public T Read<T>(byte bits = 0)
        {
            if (bits == 0)
                return readStream.Read<T>();

            return AddBits<T>(bits).ChangeType<T>();
        }

        T AddBits<T>(int bits)
        {
            ulong unpackedValue = 0;
            packedByte = Read<byte>();
            var bitsToRead = 0;
            var readBitCT = 0;
            var count = 0;

            while (readBitCT < bits)
            {

                bitsToRead = 8 - shiftedBits;

                if (bitsToRead > bits - readBitCT)
                    bitsToRead = bits - readBitCT;

                unpackedValue |= (uint)((((1 << bitsToRead) - 1) & (packedByte >> (byte)shiftedBits)) << count);

                readBitCT = bitsToRead + count;
                count += bitsToRead;

                shiftedBits = (bitsToRead + shiftedBits) & 7;

                if (shiftedBits == 0)
                    packedByte = Read<byte>();
            }

            readStream.BaseStream.Position -= 1;

            return (T)Convert.ChangeType(unpackedValue, typeof(T));
        }

        public byte[] Read(int count)
        {
            Flush();

            return readStream.ReadBytes(count);
        }

        public string ReadString()
        {
            var s = "";
            var length = Read<byte>() >> 1;

            for (int i = 0; i < length; i++)
                s += Convert.ToChar(Read<ushort>());

            return s;
        }

        public void Flush()
        {
            if (shiftedBits != 0)
            {
                shiftedBits = 0;
                packedByte = Read<byte>();
            }
        }
    }
}

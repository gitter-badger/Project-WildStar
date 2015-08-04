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

            Write(message, 12);

            FlushServer();
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

                FlushClient();

                if (Header.Message == (ushort)ClientMessage.MultiPacket)
                {
                    Header.Size = Read<uint>(24);

                    FlushClient();

                    offset = (int)readStream.BaseStream.Position + 1;

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

        public void FinishData()
        {
            //FlushServer();

            Data = (writeStream.BaseStream as MemoryStream).ToArray();
        }

        public void Finish()
        {
            writeStream = new BinaryWriter(new MemoryStream());

            Write(Data.Length + 10, 24);

            FlushServer();

            Write(ServerMessage.MultiPacket, 12);

            FlushServer();

            Write(Data.Length + 4, 24);

            FlushServer();

            writeStream.Write(Data);

            Data = (writeStream.BaseStream as MemoryStream).ToArray();
        }

        public void Write(object value, int bits)
        {
            var val = value.ChangeType<ulong>();
            var bitsToWrite = 0;
            var writtenBits = 0;
            var count = bits;

            while (count > 0)
            {
                bitsToWrite = 8 - shiftedBits;

                if (bitsToWrite > bits - writtenBits)
                    bitsToWrite = bits - writtenBits;

                packedByte |= (byte)(((ulong)((1 << bitsToWrite) - 1) & val) << (shiftedBits & 0x1F));

                writtenBits = bitsToWrite;
                count -= bitsToWrite;

                shiftedBits = (bitsToWrite + shiftedBits) & 7;

                if (shiftedBits == 0)
                {
                    writeStream.Write(packedByte);

                    packedByte = 0;
                }

                val >>= bitsToWrite;
            }
        }

        public void ReadMessage()
        {
            readStream = new BinaryReader(new MemoryStream(Data));

            Header.Message = Read<ushort>(12);

            FlushClient();
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
            FlushClient();

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

        public void FlushClient()
        {
            if (shiftedBits != 0)
            {
                shiftedBits = 0;
                packedByte = Read<byte>();
            }
        }

        public void FlushServer()
        {
            writeStream.Write(packedByte);

            shiftedBits = 0;
            packedByte = 0;
        }
    }
}

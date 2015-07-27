// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using AuthServer.Network.Packets;
using Framework.Logging;

namespace AuthServer.Network
{
    class AuthSession : IDisposable
    {
        Socket client;
        Stack<PacketBase> packetQueue;
        byte[] dataBuffer = new byte[0x400];

        public AuthSession(Socket clientSocket)
        {
            client = clientSocket;
            packetQueue = new Stack<PacketBase>();
        }

        public void Accept()
        {
            var socketEventargs = new SocketAsyncEventArgs();

            socketEventargs.SetBuffer(dataBuffer, 0, dataBuffer.Length);

            socketEventargs.Completed += OnConnection;
            socketEventargs.UserToken = client;
            socketEventargs.SocketFlags = SocketFlags.None;

            client.ReceiveAsync(socketEventargs);
        }

        public void OnConnection(object sender, SocketAsyncEventArgs e)
        {
            PacketLog.Write<PacketBase>(dataBuffer, e.BytesTransferred, client.RemoteEndPoint as IPEndPoint);

            if (e.BytesTransferred < 256 || e.BytesTransferred > 400)
            {
                Console.WriteLine("Wrong initialization packet.");

                Dispose();
            }
            else
            {
                // POST
                if (BitConverter.ToUInt32(dataBuffer, 0) == 0x54534F50)
                {
                    var packetData = new byte[e.BytesTransferred];

                    Buffer.BlockCopy(dataBuffer, 0, packetData, 0, e.BytesTransferred);

                    var packetInfo = GetMessageType(packetData);

                    if (packetInfo.Item1 != "Sts")
                    {
                        Console.WriteLine("Wrong packet type for first packet.");

                        Dispose();

                        return;
                    }

                    var packet = new StsPacket(packetData);

                    packet.ReadHeader(packetInfo);

                    packetData = new byte[packet.Header.DataLength];

                    Buffer.BlockCopy(packet.Data, packet.Header.Length, packetData, 0, packet.Header.DataLength);

                    packet.Data = packetData;

                    packet.ReadData();

                    PacketManager.InvokeStsHandler(packet, this);

                    e.Completed -= OnConnection;
                    e.Completed += Process;

                    client.ReceiveAsync(e);
                }
            }
        }

        public void Process(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                var socket = e.UserToken as Socket;
                var recievedBytes = e.BytesTransferred;

                if (recievedBytes > 0)
                {

                    if (packetQueue.Count > 0)
                    {
                        var lastPacket = packetQueue.Pop();

                        if (recievedBytes != lastPacket.Header.DataLength)
                        {
                            Console.WriteLine("Wrong data received.");

                            Dispose();

                            return;
                        }

                        var packetData = new byte[recievedBytes];

                        Buffer.BlockCopy(dataBuffer, 0, packetData, 0, recievedBytes);

                        lastPacket.Data = packetData;

                        lastPacket.ReadData();

                        ProcessPacket(lastPacket);
                    }

                    // POST
                    if (BitConverter.ToUInt32(dataBuffer, 0) == 0x54534F50)
                    {
                        {
                            var packetData = new byte[recievedBytes];

                            Buffer.BlockCopy(dataBuffer, 0, packetData, 0, recievedBytes);

                            PacketBase packet = null;

                            var packetInfo = GetMessageType(packetData);

                            if (packetInfo.Item1 == "Sts")
                            {
                                packet = new StsPacket(packetData);
                                packet.ReadHeader(packetInfo);
                            }
                            else if (packetInfo.Item1 == "Auth")
                            {
                                packet = new AuthPacket(packetData);
                                packet.ReadHeader(packetInfo);
                            }

                            if ((recievedBytes - packet.Header.Length) != packet.Header.DataLength)
                            {
                                packetQueue.Push(packet);
                            }
                            else
                            {
                                recievedBytes -= packet.Header.Length;

                                ProcessPacket(packet);
                            }
                        }
                    }


                    client.ReceiveAsync(e);
                }
                else
                    Dispose();
            }
            catch
            {
                Dispose();
            }
        }

        public void ProcessPacket(PacketBase packet)
        {
            PacketLog.Write<PacketBase>(packet.Data, packet.Data.Length, client.RemoteEndPoint as IPEndPoint);

            if (packet is StsPacket)
                PacketManager.InvokeStsHandler((StsPacket)packet, this);
            else if (packet is AuthPacket)
                PacketManager.InvokeAuthHandler((AuthPacket)packet, this);
        }

        public void Send(PacketBase packet)
        {
            try
            {
                var socketEventargs = new SocketAsyncEventArgs();

                socketEventargs.SetBuffer(packet.Data, 0, packet.Data.Length);

                socketEventargs.Completed += SendCompleted;
                socketEventargs.UserToken = packet;
                socketEventargs.RemoteEndPoint = client.RemoteEndPoint;
                socketEventargs.SocketFlags = SocketFlags.None;

                client.SendAsync(socketEventargs);

                PacketLog.Write<PacketBase>(packet.Data, packet.Data.Length, client.RemoteEndPoint as IPEndPoint);
            }
            catch
            {
                Dispose();
            }
        }

        void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
        }

        public Tuple<string[], int> ReadLines(BinaryReader readStream)
        {
            var position = 0;

            while (position == 0 && readStream.BaseStream.CanRead)
            {
                if (readStream.ReadByte() == 0x0D)
                {
                    if (readStream.ReadByte() == 0x0A)
                    {
                        if (readStream.ReadUInt16() == 0x0A0D)
                            position = (int)readStream.BaseStream.Position;
                    }
                }
            }

            readStream.BaseStream.Position = 0;

            var data = readStream.ReadBytes(position - 4);
            var lines = Encoding.ASCII.GetString(data).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            return Tuple.Create(lines, position);
        }

        public Tuple<string, string[], int> GetMessageType(byte[] data)
        {
            var headerParts = ReadLines(new BinaryReader(new MemoryStream(data)));

            if (headerParts.Item1.Length >= 2)
            {
                var identifier = headerParts.Item1[0].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                if (identifier.Length == 3)
                    return Tuple.Create(identifier[1].Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0], headerParts.Item1, headerParts.Item2);
            }

            return Tuple.Create("Unknown", new string[0], 0);
        }

        public string GetClientInfo()
        {
            var ipEndPoint = client.RemoteEndPoint as IPEndPoint;

            return ipEndPoint != null ? ipEndPoint.Address + ":" + ipEndPoint.Port : "";
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    client.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

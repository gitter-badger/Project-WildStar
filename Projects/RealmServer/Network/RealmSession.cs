// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Framework.Cryptography;
using Framework.Logging;
using RealmServer.Constants.Net;
using RealmServer.Network.Packets;

namespace RealmServer.Network
{
    class RealmSession : IDisposable
    {
        Socket client;
        ConcurrentQueue<Packet> packetQueue;
        PacketCrypt crypt = null;
        byte[] dataBuffer = new byte[0x2000];

        public RealmSession(Socket clientSocket)
        {
            client = clientSocket;
            packetQueue = new ConcurrentQueue<Packet>();
            crypt = new PacketCrypt();
        }

        public void Accept()
        {
            var socketEventArgs = new SocketAsyncEventArgs();

            socketEventArgs.SetBuffer(dataBuffer, 0, dataBuffer.Length);

            socketEventArgs.Completed += OnConnection;
            socketEventArgs.UserToken = client;
            socketEventArgs.SocketFlags = SocketFlags.None;

            client.ReceiveAsync(socketEventArgs);
        }

        public void OnConnection(object sender, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0)
            {
                var packetData = new byte[e.BytesTransferred];

                Buffer.BlockCopy(dataBuffer, 0, packetData, 0, e.BytesTransferred);

                ProcessPacket(new Packet(packetData));
            }

            PacketLog.Write<Packet>(dataBuffer, e.BytesTransferred, client.RemoteEndPoint as IPEndPoint);

            e.Completed -= OnConnection;
            e.Completed += Process;

            var sHello = new Packet(ServerMessage.SHello);

            sHello.Write(9165, 32);       // BuildNumber
            sHello.Write(1, 32);          // RealmId
            sHello.Write(0, 32);          // RealmGroupId
            sHello.Write(0, 32);
            sHello.Write(0, 64);
            sHello.Write(0, 16);
            sHello.Write(11, 5);          // ConnectionType
            sHello.Write(0xBDD06089, 32); // NetworkMessageCRC
            sHello.Write(0, 32);
            sHello.Write(0, 64);
            sHello.Write(0, 32);

            Send(sHello);

            client.ReceiveAsync(e);
        }

        public void Process(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                var socket = e.UserToken as Socket;
                var receivedBytes = e.BytesTransferred;

                if (receivedBytes > 0)
                {
                    while (receivedBytes > 0)
                    {
                        var packetData = new byte[receivedBytes];

                        Buffer.BlockCopy(dataBuffer, 0, packetData, 0, receivedBytes);

                        var pkt = new Packet(packetData);

                        receivedBytes -= (int)pkt.Header.Size;

                        if (pkt.Header.Message != (ushort)ClientMessage.State1 && pkt.Header.Message != (ushort)ClientMessage.State2)
                        {
                            crypt.Decrypt(pkt.Data, pkt.Data.Length);

                            pkt.ReadMessage();

                            // Remove the 'Composite' header.
                            receivedBytes -= 6;

                            Buffer.BlockCopy(dataBuffer, (int)pkt.Header.Size + 6, dataBuffer, 0, receivedBytes);
                        }
                        else
                            Buffer.BlockCopy(dataBuffer, (int)pkt.Header.Size, dataBuffer, 0, receivedBytes);

                        if (receivedBytes > 0)
                            packetQueue.Enqueue(pkt);

                        ProcessPacket(pkt);
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

        public void ProcessPacket(Packet packet)
        {
            if (packetQueue.Count > 0)
                packetQueue.TryDequeue(out packet);

            PacketLog.Write<Packet>(packet.Data, packet.Data.Length, client.RemoteEndPoint as IPEndPoint);

            PacketManager.Invoke(packet, this);
        }

        public void Send(Packet packet)
        {
            try
            {
                packet.FinishData();

                crypt.Encrypt(packet.Data, packet.Data.Length);

                packet.Finish();

                PacketLog.Write<Packet>(packet.Data, packet.Data.Length, client.RemoteEndPoint as IPEndPoint);

                var socketEventargs = new SocketAsyncEventArgs();

                socketEventargs.SetBuffer(packet.Data, 0, packet.Data.Length);

                socketEventargs.Completed += SendCompleted;
                socketEventargs.UserToken = packet;
                socketEventargs.RemoteEndPoint = client.RemoteEndPoint;
                socketEventargs.SocketFlags = SocketFlags.None;

                client.SendAsync(socketEventargs);
            }
            catch
            {
                Dispose();
            }
        }

        public void SendRaw(byte[] data)
        {
            try
            {
                PacketLog.Write<Packet>(data, data.Length, client.RemoteEndPoint as IPEndPoint);

                var socketEventargs = new SocketAsyncEventArgs();

                socketEventargs.SetBuffer(data, 0, data.Length);

                socketEventargs.Completed += SendCompleted;
                socketEventargs.UserToken = data;
                socketEventargs.RemoteEndPoint = client.RemoteEndPoint;
                socketEventargs.SocketFlags = SocketFlags.None;

                client.SendAsync(socketEventargs);
            }
            catch
            {
                Dispose();
            }
        }

        void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
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

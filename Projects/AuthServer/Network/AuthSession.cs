// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using AuthServer.Network.Packets;
using Framework.Cryptography;
using Framework.Logging;

namespace AuthServer.Network
{
    class AuthSession : IDisposable
    {
        Socket client;
        Queue<Packet> packetQueue;
        PacketCrypt crypt = null;
        byte[] dataBuffer = new byte[0x1000];

        public AuthSession(Socket clientSocket)
        {
            client = clientSocket;
            packetQueue = new Queue<Packet>();
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
            PacketLog.Write<Packet>(dataBuffer, e.BytesTransferred, client.RemoteEndPoint as IPEndPoint);

            e.Completed -= OnConnection;
            e.Completed += Process;

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
                    var packetData = new byte[receivedBytes];

                    Buffer.BlockCopy(dataBuffer, 0, packetData, 0, receivedBytes);

                    var pkt = new Packet(packetData);

                    crypt.Decrypt(pkt.Data, pkt.Data.Length);

                    ProcessPacket(pkt);

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
            PacketLog.Write<Packet>(packet.Data, packet.Data.Length, client.RemoteEndPoint as IPEndPoint);

            PacketManager.Invoke(packet, this);
        }

        public void Send(Packet packet)
        {
            try
            {
                packet.Finish();

                PacketLog.Write<Packet>(packet.Data, packet.Data.Length, client.RemoteEndPoint as IPEndPoint);

                crypt.Encrypt(packet.Data, packet.Data.Length);

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

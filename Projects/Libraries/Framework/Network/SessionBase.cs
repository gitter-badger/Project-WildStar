// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Framework.Constants.Net;
using Framework.Cryptography;
using Framework.Logging;
using Framework.Packets;

namespace Framework.Network
{
    public abstract class SessionBase : IDisposable
    {
        public PacketCrypt Crypt { get; set; }

        protected Socket client;
        protected ConcurrentQueue<Packet> packetQueue;
        protected byte[] dataBuffer = new byte[0x1000];

        public SessionBase(Socket clientSocket)
        {
            client = clientSocket;
            packetQueue = new ConcurrentQueue<Packet>();
            Crypt = new PacketCrypt();
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

        public abstract void OnConnection(object sender, SocketAsyncEventArgs e);

        public abstract void Process(object sender, SocketAsyncEventArgs e);

        public abstract Task ProcessPacket(Packet packet);

        public abstract Task Send(ServerPacket packet);

        public void SendRaw(byte[] data)
        {
            try
            {
                var socketEventargs = new SocketAsyncEventArgs();

                socketEventargs.SetBuffer(data, 0, data.Length);

                socketEventargs.Completed += SendCompleted;
                socketEventargs.UserToken = data;
                socketEventargs.RemoteEndPoint = client.RemoteEndPoint;
                socketEventargs.SocketFlags = SocketFlags.None;

                client.SendAsync(socketEventargs);

                PacketLog.Write<GlobalServerMessage>(0xFFFF, data, data.Length, client.RemoteEndPoint as IPEndPoint);
            }
            catch
            {
                Dispose();
            }
        }

        protected void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
        }

        public string GetClientInfo()
        {
            var ipEndPoint = client.RemoteEndPoint as IPEndPoint;

            return ipEndPoint != null ? ipEndPoint.Address + ":" + ipEndPoint.Port : "";
        }

        public string GetIPAddress()
        {
            var ipEndPoint = client.RemoteEndPoint as IPEndPoint;

            return ipEndPoint != null ? ipEndPoint.Address.ToString() : "";
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

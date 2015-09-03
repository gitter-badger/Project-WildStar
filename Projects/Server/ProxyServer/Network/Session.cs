// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Framework.Constants.Net;
using Framework.Logging;
using Framework.Network;
using Framework.Packets;
using Framework.Packets.Server.Misc;
using ProxyServer.Constants.Net;
using ProxyServer.Packets;

namespace ProxyServer.Network
{
    class Session : SessionBase
    {
        public Session(Socket clientSocket) : base(clientSocket) { }

        public override async void OnConnection(object sender, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0)
            {
                var packetData = new byte[e.BytesTransferred];

                Buffer.BlockCopy(dataBuffer, 0, packetData, 0, e.BytesTransferred);

                await ProcessPacket(new Packet(packetData, (ushort)ClientMessage.Composite));
            }

            e.Completed -= OnConnection;
            e.Completed += Process;

            client.ReceiveAsync(e);

            var sHello = new SHello
            {
                BuildNumber = 9317,
                ConnectionType = 3,
                NetworkMessageCRC = 0xF762368B
            };

            await Send(sHello);
        }

        public override async void Process(object sender, SocketAsyncEventArgs e)
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

                        var pkt = new Packet(packetData, (ushort)ClientMessage.Composite);

                        receivedBytes -= (int)pkt.Header.Size;

                        if (pkt.Header.Message != (ushort)GlobalClientMessage.State1 && pkt.Header.Message != (ushort)GlobalClientMessage.State2)
                        {
                            Crypt.Decrypt(pkt.Data, pkt.Data.Length);

                            pkt.ReadMessage();

                            // Remove the 'Composite' header.
                            receivedBytes -= 6;

                            Buffer.BlockCopy(dataBuffer, (int)pkt.Header.Size + 6, dataBuffer, 0, receivedBytes);
                        }
                        else
                            Buffer.BlockCopy(dataBuffer, (int)pkt.Header.Size, dataBuffer, 0, receivedBytes);

                        if (receivedBytes > 0)
                            packetQueue.Enqueue(pkt);

                        await ProcessPacket(pkt);
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

        public override async Task ProcessPacket(Packet packet)
        {
            if (packetQueue.Count > 0)
                packetQueue.TryDequeue(out packet);

            await PacketManager.Invoke(packet, this);
            await PacketLog.Write<ClientMessage>(packet.Data, packet.Data.Length, client.RemoteEndPoint as IPEndPoint);
        }

        public override async Task Send(ServerPacket packet)
        {
            try
            {
                packet.Write();
                packet.Packet.FinishData();

                Crypt.Encrypt(packet.Packet.Data, packet.Packet.Data.Length);

                packet.Packet.Finish((ushort)ServerMessage.Composite);

                var socketEventargs = new SocketAsyncEventArgs();

                socketEventargs.SetBuffer(packet.Packet.Data, 0, packet.Packet.Data.Length);

                socketEventargs.Completed += SendCompleted;
                socketEventargs.UserToken = packet;
                socketEventargs.RemoteEndPoint = client.RemoteEndPoint;
                socketEventargs.SocketFlags = SocketFlags.None;

                client.SendAsync(socketEventargs);

                await PacketLog.Write<ServerMessage>(packet.Packet.Data, packet.Packet.Data.Length, client.RemoteEndPoint as IPEndPoint);
            }
            catch
            {
                Dispose();
            }
        }
    }
}

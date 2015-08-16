// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Network;
using System.Threading.Tasks;
using Framework.Packets;
using ProxyServer.Constants.Net;
using Framework.Logging;
using System.Net.Sockets;
using System.Net;
using ProxyServer.Packets;

namespace ProxyServer.Network
{
    class Session : SessionBase
    {
        public Session(Socket clientSocket) : base(clientSocket) { }

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
                packet.Packet.FinishData();

                crypt.Encrypt(packet.Packet.Data, packet.Packet.Data.Length);

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

// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Network;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ProxyServer.Network
{
    class Server : ServerBase
    {
        public Server(string ip, int port) : base(ip, port) { }

        public override async Task DoWork(Socket client)
        {
            await Task.Factory.StartNew(new Session(client).Accept);
        }
    }
}

// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Network
{
    public class ServerBase : IDisposable
    {
        TcpListener listener;
        bool isRunning;

        public ServerBase(string ip, int port)
        {
            var bindIP = IPAddress.None;

            try
            {
                listener = new TcpListener(bindIP, port);
                listener.Start();

                if (isRunning = listener.Server.IsBound)
                    new Thread(AcceptConnection).Start(200);
            }
            catch
            {
            }
        }

        async void AcceptConnection(object delay)
        {
            while (isRunning)
            {
                Thread.Sleep((int)delay);

                if (listener.Pending())
                {
                    var clientSocket = await listener.AcceptSocketAsync();

                    if (clientSocket != null)
                        await DoWork(clientSocket);
                }
            }
        }

        public virtual Task DoWork(Socket client)
        {
            return null;
        }

        public void Dispose()
        {
            listener = null;
            isRunning = false;
        }
    }
}

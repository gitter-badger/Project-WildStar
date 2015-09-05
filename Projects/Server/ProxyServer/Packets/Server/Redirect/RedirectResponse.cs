// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Packets;
using ProxyServer.Constants.Net;

namespace ProxyServer.Packets.Server.Redirect
{
    class RedirectResponse : ServerPacket
    {
        public uint Result { get; set; }

        public RedirectResponse() : base (ServerMessage.RedirectResponse) { }

        public override void Write()
        {
            Packet.Write(Result, 32);
        }
    }
}

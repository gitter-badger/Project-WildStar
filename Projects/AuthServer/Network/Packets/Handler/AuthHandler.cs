// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AuthServer.Attributes;
using AuthServer.Constants.Net;

namespace AuthServer.Network.Packets.Handler
{
    class AuthHandler
    {
        [AuthPacket(ClientMessage.State1)]
        public static void HandleState1(Packet packet, AuthSession session)
        {

        }

        [AuthPacket(ClientMessage.State2)]
        public static void HandleState2(Packet packet, AuthSession session)
        {
            
        }
    }
}

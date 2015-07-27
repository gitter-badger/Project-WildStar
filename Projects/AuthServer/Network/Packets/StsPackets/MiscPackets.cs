// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AuthServer.Attributes;

namespace AuthServer.Network.Packets.StsPackets
{
    class MiscPackets
    {
        [StsMessage(Constants.Net.StsMessage.Connect)]
        public static void HandleStsConnect(StsPacket packet, AuthSession session)
        {
        }
    }
}

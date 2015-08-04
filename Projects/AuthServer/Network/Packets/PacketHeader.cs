// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AuthServer.Network.Packets
{
    class PacketHeader
    {
        public ushort Message { get; set; }
        public uint Size      { get; set; }
    }
}

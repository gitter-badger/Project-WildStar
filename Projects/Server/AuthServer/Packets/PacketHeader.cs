// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AuthServer.Constants.Net;

namespace AuthServer.Packets
{
    class PacketHeader
    {
        public StsMessage Message { get; set; }
        public ushort Length       { get; set; }
        public ushort DataLength   { get; set; }
        public byte Sequence       { get; set; }
    }
}

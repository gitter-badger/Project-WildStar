// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AuthServer.Network.Packets.Headers
{
    abstract class HeaderBase
    {
        public ushort Length     { get; set; }
        public ushort DataLength { get; set; }
        public byte Sequence     { get; set; }
    }
}

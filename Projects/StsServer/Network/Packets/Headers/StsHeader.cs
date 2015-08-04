// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using StsServer.Constants.Net;

namespace StsServer.Network.Packets.Headers
{
    class StsHeader
    {
        public StsMessage Message { get; set; }
        public ushort Length       { get; set; }
        public ushort DataLength   { get; set; }
        public byte Sequence       { get; set; }
    }
}

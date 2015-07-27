// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AuthServer.Constants.Net;

namespace AuthServer.Network.Packets.Headers
{
    class StsHeader : HeaderBase
    {
        public StsMessage Message { get; set; }
    }
}

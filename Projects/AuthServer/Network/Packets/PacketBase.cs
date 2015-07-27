// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using AuthServer.Network.Packets.Headers;

namespace AuthServer.Network.Packets
{
    abstract class PacketBase
    {
        public HeaderBase Header { get; set; }
        public Dictionary<string, object> Values{ get; set; }
        public byte[] Data { get; set; }

        public abstract void ReadHeader(Tuple<string, string[], int> headerInfo);
        public abstract void ReadData();
    }
}

// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using RealmServer.Constants.Net;

namespace RealmServer.Attributes
{
    class RealmPacketAttribute : Attribute
    {
        public ClientMessage Message { get; set; }

        public RealmPacketAttribute(ClientMessage message)
        {
            Message = message;
        }
    }
}

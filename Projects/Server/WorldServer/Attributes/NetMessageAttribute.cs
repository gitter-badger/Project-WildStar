// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using WorldServer.Constants.Net;

namespace WorldServer.Attributes
{
    class NetMessageAttribute : Attribute
    {
        public ClientMessage Message { get; set; }

        public NetMessageAttribute(ClientMessage message)
        {
            Message = message;
        }
    }
}

// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using AuthServer.Constants.Net;

namespace AuthServer.Attributes
{
    class AuthPacketAttribute : Attribute
    {
        public ClientMessage Message { get; set; }

        public AuthPacketAttribute(ClientMessage message)
        {
            Message = message;
        }
    }
}

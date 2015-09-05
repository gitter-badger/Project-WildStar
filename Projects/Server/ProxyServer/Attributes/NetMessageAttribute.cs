// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using ProxyServer.Constants.Net;

namespace ProxyServer.Attributes
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

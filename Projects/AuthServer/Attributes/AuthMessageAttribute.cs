// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using AuthServer.Constants.Net;

namespace AuthServer.Attributes
{
    class AuthMessageAttribute : Attribute
    {
        public AuthMessage Message { get; set; }
        
        public AuthMessageAttribute(AuthMessage message)
        {
            Message = message;
        }
    }
}

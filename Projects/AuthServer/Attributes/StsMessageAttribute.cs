// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using AuthServer.Constants.Net;

namespace AuthServer.Attributes
{
    class StsMessageAttribute : Attribute
    {
        public StsMessage Message { get; set; }

        public StsMessageAttribute(StsMessage message)
        {
            Message = message;
        }
    }
}

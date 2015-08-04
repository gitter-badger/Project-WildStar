// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using StsServer.Constants.Net;

namespace StsServer.Attributes
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

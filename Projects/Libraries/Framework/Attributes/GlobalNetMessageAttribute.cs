// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Constants.Net;
using System;


namespace Framework.Attributes
{
    public class GlobalNetMessageAttribute : Attribute
    {
        public GlobalClientMessage Message { get; set; }

        public GlobalNetMessageAttribute(GlobalClientMessage message)
        {
            Message = message;
        }
    }
}

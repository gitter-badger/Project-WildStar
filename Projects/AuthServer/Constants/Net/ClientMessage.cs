// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AuthServer.Constants.Net
{
    enum ClientMessage : ushort
    {
        State1      = 0x000,
        State2      = 0x001,
        SHello      = 0x003,
        Composite   = 0x22F,
        AuthRequest = 0x538,
    }
}

// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ProxyServer.Constants.Net
{
    enum ServerMessage: ushort
    {
        Composite        = 0x06D,
        Redirect         = 0x3B3,
        RedirectResponse = 0x542,
    }
}

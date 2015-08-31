// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace WorldServer.Constants.Net
{
    enum ClientMessage : ushort
    {
        Composite        = 0x230,
        GatewayRequest   = 0x53D,
        CharacterList    = 0x762,
        RealmListChanged = 0x731,
    }
}

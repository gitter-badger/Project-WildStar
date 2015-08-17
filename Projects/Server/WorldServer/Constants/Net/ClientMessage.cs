// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace WorldServer.Constants.Net
{
    enum ClientMessage : ushort
    {
        Composite        = 0x22F,
        GatewayRequest   = 0x536,
        CharacterList    = 0x75B,
        RealmListChanged = 0x72A,
    }
}

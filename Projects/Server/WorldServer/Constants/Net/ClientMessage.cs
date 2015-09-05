// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace WorldServer.Constants.Net
{
    enum ClientMessage : ushort
    {
        Composite            = 0x230,
        GatewayRequest       = 0x540,
        CharacterList        = 0x768,
        RealmListChanged     = 0x737,
        RequestRealmTransfer = 0x248,
    }
}

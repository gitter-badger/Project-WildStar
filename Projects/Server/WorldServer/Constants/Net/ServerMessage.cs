// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace WorldServer.Constants.Net
{
    enum ServerMessage : ushort
    {
        CharacterList            = 0x111,
        RealmTransfer            = 0x319,
        Composite                = 0x3B4,
        Realmlist                = 0x6FD,
        AccountEntitlementUpdate = 0x8E6,
    }
}

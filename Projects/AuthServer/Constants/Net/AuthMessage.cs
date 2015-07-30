// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AuthServer.Constants.Net
{
    public enum AuthMessage
    {
        Unknown          = -1,
        Connect          = 0,
        LoginStart       = 1,
        KeyData          = 2,
        LoginFinish      = 3,
        ListMyAccounts   = 4,
        RequestGameToken = 5,
        Ping             = 0xFF
    }
}

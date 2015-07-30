// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Framework.Cryptography
{
    public class PacketCrypt
    {
        ulong keyValue1 = 0x718DA9074F2DEB91;
        uint keyValue2 = 0xAA7F8EA9;

        byte[] keyState = new byte[8];
        byte[] key = new byte[128];

        public PacketCrypt()
        {
            var baseKey = CalculateBaseKey();
            var keyPart = keyValue2 * (baseKey + keyValue1);

            for (var i = 0; i < key.Length; i += 8)
            {
                var bytes = BitConverter.GetBytes(keyPart);

                Buffer.BlockCopy(bytes, 0, key, i, bytes.Length);

                keyPart = keyValue2 * (baseKey + keyPart);
            }

            var state = keyValue1;

            for (var i = 32; i < key.Length + 32; i += 32)
            {
                state = keyValue2 * (BitConverter.ToUInt64(key, i - 32) + state);
                state = keyValue2 * (BitConverter.ToUInt64(key, i - 24) + state);
                state = keyValue2 * (BitConverter.ToUInt64(key, i - 16) + state);
                state = keyValue2 * (BitConverter.ToUInt64(key, i - 8) + state);
            }

            keyState = BitConverter.GetBytes(state);
        }

        ulong CalculateBaseKey()
        {
            return keyValue1 * (keyValue2 * (keyValue1 - 0x502028AC11024130) + 9024) + 0x4282FFFAE72B3921;
        }
    }
}

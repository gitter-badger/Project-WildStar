// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Framework.Misc.Extensions
{
    public static class ArrayExtensions
    {
        public static bool Compare(this byte[] b, byte[] b2)
        {
            for (int i = 0; i < b2.Length; i++)
                if (b[i] != b2[i])
                    return false;

            return true;
        }

        public static byte[] Combine(this byte[] data, params byte[][] pData)
        {
            var combined = data;

            foreach (var arr in pData)
            {
                var currentSize = combined.Length;

                Array.Resize(ref combined, currentSize + arr.Length);

                Buffer.BlockCopy(arr, 0, combined, currentSize, arr.Length);
            }

            return combined;
        }

        public static string ToHexString(this byte[] data)
        {
            var hex = "";

            foreach (var b in data)
                hex += $"{b:X2}";

            return hex.ToUpper();
        }

        public static byte[] ReverseUInt32(this byte[] data)
        {
            var ret = new byte[data.Length];

            for (var i = 0; i < data.Length; i += 4)
                Buffer.BlockCopy(data, i, ret, ret.Length - (i + 4), 4);

            return ret;
        }

        public static byte[] Fill32Bits(this byte[] data)
        {
            return data.Combine(new byte[4 - (data.Length % 4)]);
        }
    }
}

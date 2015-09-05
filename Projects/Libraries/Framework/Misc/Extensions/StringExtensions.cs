// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Framework.Misc.Extensions
{
    public static class StringExtensions
    {
        public static byte[] ToByteArray(this string s)
        {
            var data = new byte[s.Length / 2];

            for (int i = 0; i < s.Length; i += 2)
                data[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);

            return data;
        }
    }
}

// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace Framework.Misc.Extensions
{
    public static class BinaryWriterExtensions
    {
        public static byte[] ToArray(this BinaryWriter br)
        {
            return (br.BaseStream as MemoryStream).ToArray();
        }
    }
}

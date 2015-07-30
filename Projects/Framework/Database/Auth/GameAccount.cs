// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Lappa_ORM;

namespace Framework.Database.Auth
{
    public class GameAccount : Entity
    {
        [AutoIncrement]
        public uint Id        { get; set; }
        public uint AccountId { get; set; }
        public string Alias   { get; set; }
        public string Created { get; set; }
    }
}

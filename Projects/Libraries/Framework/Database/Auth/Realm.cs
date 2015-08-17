// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Constants.Realm;
using Lappa_ORM;

namespace Framework.Database.Auth
{
    public class Realm : Entity
    {
        [AutoIncrement]
        public uint Id                    { get; set; }
        public string Name                { get; set; }
        public string IP                  { get; set; }
        public ushort Port                { get; set; }
        public RealmType Type             { get; set; }
        public RealmStatus Status         { get; set; }
        public RealmPopulation Population { get; set; }
        public uint Index                 { get; set; } // 0 = default realm
    }
}

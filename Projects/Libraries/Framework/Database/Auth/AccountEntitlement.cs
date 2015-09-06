// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Lappa_ORM;

namespace Framework.Database.Auth
{
    public class AccountEntitlement : Entity
    {
        public uint AccountId { get; set; }
        public uint Id        { get; set; }
        public uint Value     { get; set; }

        public virtual Account Account { get; set; }
    }
}

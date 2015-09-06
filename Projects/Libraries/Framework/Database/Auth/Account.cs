// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Lappa_ORM;

namespace Framework.Database.Auth
{
    public class Account: Entity
    {
        [AutoIncrement]
        public uint Id                 { get; set; }
        public string Alias            { get; set; } // GameAccount alias.
        public string Email            { get; set; } // Equals LoginName.
        public string PasswordVerifier { get; set; }
        public string Salt             { get; set; }
        public string GatewayTicket    { get; set; }
        public bool Online             { get; set; }

        public virtual List<AccountEntitlement> Entitlements { get; set; }
    }
}

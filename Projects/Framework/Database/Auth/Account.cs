// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Lappa_ORM;

namespace Framework.Database.Auth
{
    public class Account : Entity
    {
        public uint Id                 { get; set; }
        public string Email            { get; set; }
        public string UserName         { get; set; }
        public string FirstName        { get; set; }
        public string LastName         { get; set; }
        public string PasswordVerifier { get; set; }
        public string Salt             { get; set; }
        public bool Online             { get; set; }
    }
}

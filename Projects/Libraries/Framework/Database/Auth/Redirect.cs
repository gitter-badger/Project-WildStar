// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Lappa_ORM;

namespace Framework.Database.Auth
{
    public class Redirect : Entity
    {
        public uint AccountId { get; set; }
        public string IP      { get; set; }

        public virtual Account Account { get; set; }
    }
}

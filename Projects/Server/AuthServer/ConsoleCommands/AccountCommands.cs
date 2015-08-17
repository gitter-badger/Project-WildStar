// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AuthServer.Attributes;
using AuthServer.Cryptography;
using Framework.Database;
using Framework.Database.Auth;
using Framework.Logging;
using Framework.Misc;
using Framework.Misc.Extensions;

namespace AuthServer.ConsoleCommands
{
    class AccountCommands
    {
        [ConsoleCommand("CreateAccount", "")]
        public static void CreateAccount(string[] args)
        {
            var alias = Helper.Read<string>(args, 0);
            var email = Helper.Read<string>(args, 1);
            var password = Helper.Read<string>(args, 2);

            if (alias != null && email != null && password != null)
            {
                var salt = Helper.GenerateRandomKey(8).ToHexString();
                var result = DB.Auth.Any<Account>(a => a.Email == email || a.Alias == alias);

                if (!result)
                {
                    var srp = new SRP6a(salt);

                    srp.CalculateX(email, password, false);

                    var account = new Account
                    {
                        Email = email,
                        Alias = alias,
                        PasswordVerifier = srp.V.ToHexString(),
                        Salt = salt,
                        GatewayTicket = ""
                    };

                    if (DB.Auth.Add(account))
                        Log.Normal($"Account 'Email: {email}, Alias: {alias}' successfully created.");
                }
                else
                    Log.Error($"Account 'Email: {email}' or 'Alias: {alias}' already in database.");
            }
        }

        [ConsoleCommand("DeleteAccount", "")]
        public static void DeleteAccount(string[] args)
        {
            var email = Helper.Read<string>(args, 0);

            if (DB.Auth.Delete<Account>(a => a.Email == email))
                Log.Normal($"Account 'Email: {email}' successfully deleted.");
            else
                Log.Error($"Account 'Email: {email}' doesn't exist.");
        }
    }
}

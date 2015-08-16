// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using Framework.Logging;
using Framework.Misc.Extensions;

namespace Framework.Misc
{
    public static class Helper
    {
        public static byte[] GenerateRandomKey(int length)
        {
            var random = new Random((int)((uint)(Guid.NewGuid().GetHashCode() ^ 1 >> 89 << 2 ^ 42)).LeftRotate(13));
            var key = new byte[length];

            for (int i = 0; i < length; i++)
            {
                int randValue = -1;

                do
                {
                    randValue = (int)((uint)random.Next(0xFF)).LeftRotate(1) ^ i;
                } while (randValue > 0xFF && randValue <= 0);

                key[i] = (byte)randValue;
            }

            return key;
        }

        public static T Read<T>(string[] args, int index)
        {
            try
            {
                return args[index].ChangeType<T>();
            }
            catch
            {
                 Logging.Log.Error("Wrong arguments.");
            }

            return default(T);
        }

        public static void PrintHeader(string serverName)
        {

            Log.Init(@"_____________________WildStar_____________________");
            Log.Init(@"___  ___      _ _   _        _____                ");
            Log.Init(@"|  \/  |     | | | (_)      |  ___|               ");
            Log.Init(@"| .  . |_   _| | |_ _ ______| |__ _ __ ___  _   _ ");
            Log.Init(@"| |\/| | | | | | __| |______|  __| '_ ` _ \| | | |");
            Log.Init(@"| |  | | |_| | | |_| |      | |__| | | | | | |_| |");
            Log.Init(@"\_|  |_/\__,_|_|\__|_|      \____/_| |_| |_|\__,_|");

            var sb = new StringBuilder();

            sb.Append("__________________________________________________");

            var nameStart = (50 - serverName.Length) / 2;

            sb.Insert(nameStart, serverName);
            sb.Remove(nameStart + serverName.Length, serverName.Length);

            Log.Init(sb.ToString());
            Log.Init($"{"www.multi-emu.com", 33}");

            Log.Message();
            Log.Normal($"Starting Project WildStar {serverName}...");
        }
    }
}

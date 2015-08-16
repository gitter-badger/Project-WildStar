// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Text;
using Framework.Logging;
using Framework.Misc.Extensions;

namespace Framework.Misc
{
    public class Config
    {
        string[] configContent;

        public Config(string config)
        {
            if (!File.Exists(config))
            {
                Log.Error($"{config} doesn't exist!");

                Environment.Exit(0);
            }
            else
                configContent = File.ReadAllLines(config, Encoding.UTF8);
        }

        public T Read<T>(string name, T value, bool hex = false)
        {
            string nameValue = null;
            var lineCounter = 0;

            try
            {
                if (configContent.Length != 0)
                {
                    foreach (var option in configContent)
                    {
                        var configOption = option.Split(new char[] { '=' }, StringSplitOptions.None);

                        if (configOption[0].StartsWith(name, StringComparison.Ordinal))
                        {
                            if (configOption[1].Trim() == "")
                                nameValue = value.ToString();
                            else
                                nameValue = configOption[1].Replace("\"", "").Trim();
                        }

                        lineCounter++;
                    }
                }
                else
                {
                    nameValue = value.ToString();

                    Log.Error($"Can't find config option '{name}'");
                    Log.Error($"Use default value '{value}'");
                    Log.Message();
                }

                if (hex)
                    return Convert.ToInt32(nameValue, 16).ChangeType<T>();

                if (typeof(T) == typeof(bool))
                {
                    if (nameValue == "0")
                        return false.ChangeType<T>();
                    else if (nameValue == "1")
                        return true.ChangeType<T>();
                }

            }
            catch
            {
                Log.Error($"Error while reading config option: '{name}'");
            }

            if (typeof(T).IsEnum)
                return (T)Enum.Parse(typeof(T), nameValue, true);
            else
                return nameValue.ChangeType<T>();
        }
    }
}

// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Framework.Logging;
using WorldServer.Attributes;

namespace WorldServer.ConsoleCommands
{
    class ConsoleCommandManager
    {
        protected static Dictionary<string, HandleCommand> CommandHandlers = new Dictionary<string, HandleCommand>();
        public delegate void HandleCommand(string[] args);

        public static void DefineCommands()
        {
            var currentAsm = Assembly.GetEntryAssembly();

            foreach (var type in currentAsm.GetTypes())
            {
                foreach (var methodInfo in type.GetMethods())
                {
                    foreach (var commandAttr in methodInfo.GetCustomAttributes<ConsoleCommandAttribute>())
                        if (commandAttr != null)
                            CommandHandlers[commandAttr.Command] = (HandleCommand)Delegate.CreateDelegate(typeof(HandleCommand), methodInfo);
                }
            }
        }

        public static void InitCommands()
        {
            DefineCommands();

            Log.Normal("WorldServer successfully started");
            Log.Normal("Total Memory: {0} Kilobytes", GC.GetTotalMemory(false) / 1024);

            while (true)
            {
                Thread.Sleep(1);

                Log.Normal("Multi-Emu WorldServer >> ");

                var line = Console.ReadLine().Split(new string[] { " " }, StringSplitOptions.None);

                if (line.Length > 0)
                {
                    var args = new string[line.Length - 1];

                    if (args.Length > 0)
                        Array.Copy(line, 1, args, 0, args.Length);

                    InvokeHandler(line[0].ToLower(), args);
                }
            }
        }

        static void InvokeHandler(string command, params string[] args)
        {
            if (CommandHandlers.ContainsKey(command.ToLower()))
                CommandHandlers[command].Invoke(args);
            else if (command != "")
                Log.Error($"'{command}' isn't a valid console command.");
        }
    }
}

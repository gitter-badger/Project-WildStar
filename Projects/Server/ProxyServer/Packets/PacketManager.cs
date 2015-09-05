// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Attributes;
using Framework.Constants.Net;
using Framework.Logging;
using Framework.Packets;
using ProxyServer.Attributes;
using ProxyServer.Constants.Net;
using ProxyServer.Network;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ProxyServer.Packets
{
    class PacketManager
    {
        static ConcurrentDictionary<ushort, Tuple<MethodInfo, Type>> MessageHandlers = new ConcurrentDictionary<ushort, Tuple<MethodInfo, Type>>();

        public static void DefineMessageHandler()
        {
            var currentAsm = Assembly.GetExecutingAssembly();
            var globalAsm = typeof(GlobalNetMessageAttribute).Assembly;

            foreach (var type in currentAsm.GetTypes().Concat(globalAsm.GetTypes()))
            {
                foreach (var methodInfo in type.GetMethods())
                {
                    foreach (dynamic msgAttr in methodInfo.GetCustomAttributes())
                    {
                        if (msgAttr is GlobalNetMessageAttribute || msgAttr is NetMessageAttribute)
                            MessageHandlers.TryAdd((ushort)msgAttr.Message, Tuple.Create(methodInfo, methodInfo.GetParameters()[0].ParameterType));
                    }
                }
            }
        }

        public static async Task Invoke(Packet reader, Session session)
        {
            var message = reader.Header.Message;

            Tuple<MethodInfo, Type> data;

            if (MessageHandlers.TryGetValue(message, out data))
            {
                var handlerObj = Activator.CreateInstance(data.Item2) as ClientPacket;

                handlerObj.Packet = reader;

                await Task.Run(() => handlerObj.Read());

                // Fix the position after the last read.
                reader.Read(1);

                if (handlerObj.IsReadComplete)
                    data.Item1.Invoke(null, new object[] { handlerObj, session });
                else
                    Log.Error($"Packet read for '{data.Item2.Name}' failed.");
            }
            else
            {
                var msgName = Enum.GetName(typeof(ClientMessage), message) ?? Enum.GetName(typeof(GlobalClientMessage), message);

                if (msgName == null)
                    Log.Network($"Received unknown opcode '0x{message:X}, Length: {reader.Data.Length}'.");
                else
                    Log.Network($"Packet handler for '{msgName} (0x{message:X}), Length: {reader.Data.Length}' not implemented.");
            }

        }
    }
}

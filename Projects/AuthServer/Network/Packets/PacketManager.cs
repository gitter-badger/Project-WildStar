// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Reflection;
using AuthServer.Attributes;
using AuthServer.Constants.Net;
using AuthServer.Network.Packets.Headers;

namespace AuthServer.Network.Packets
{
    class PacketManager
    {
        static readonly ConcurrentDictionary<AuthMessage, HandleAuthPacket> AuthMessageHandlers = new ConcurrentDictionary<AuthMessage, HandleAuthPacket>();
        delegate void HandleAuthPacket(AuthPacket packet, AuthSession client);

        public static void DefineMessageHandler()
        {
            var currentAsm = Assembly.GetExecutingAssembly();

            foreach (var type in currentAsm.GetTypes())
                foreach (var methodInfo in type.GetMethods())
                    foreach (var msgAttr in methodInfo.GetCustomAttributes<AuthMessageAttribute>())
                        AuthMessageHandlers.TryAdd(msgAttr.Message, Delegate.CreateDelegate(typeof(HandleAuthPacket), methodInfo) as HandleAuthPacket);
        }

        public static bool Invoke(AuthPacket reader, AuthSession session)
        {
            var message = ((AuthHeader)reader.Header).Message;

            Console.WriteLine($"Received Auth packet: {message} (0x{message:X}), Length: {reader.Data.Length}");

            HandleAuthPacket packet;

            if (AuthMessageHandlers.TryGetValue(message, out packet))
            {
                packet.Invoke(reader, session);

                return true;
            }

            return false;
        }
    }
}

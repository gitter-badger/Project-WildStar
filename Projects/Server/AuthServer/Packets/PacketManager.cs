// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Reflection;
using AuthServer.Attributes;
using AuthServer.Constants.Net;
using AuthServer.Network;
using Framework.Logging;

namespace AuthServer.Packets
{
    class PacketManager
    {
        static readonly ConcurrentDictionary<StsMessage, HandleAuthPacket> AuthMessageHandlers = new ConcurrentDictionary<StsMessage, HandleAuthPacket>();
        delegate void HandleAuthPacket(Packet packet, Session client);

        public static void DefineMessageHandler()
        {
            var currentAsm = Assembly.GetExecutingAssembly();

            foreach (var type in currentAsm.GetTypes())
                foreach (var methodInfo in type.GetMethods())
                    foreach (var msgAttr in methodInfo.GetCustomAttributes<AuthMessageAttribute>())
                        AuthMessageHandlers.TryAdd(msgAttr.Message, Delegate.CreateDelegate(typeof(HandleAuthPacket), methodInfo) as HandleAuthPacket);
        }

        public static bool Invoke(Packet reader, Session session)
        {
            var message = reader.Header.Message;

            Log.Network($"Received Auth packet: {message} (0x{message:X}), Length: {reader.Data.Length}");

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

// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Reflection;
using StsServer.Attributes;
using StsServer.Constants.Net;
using StsServer.Network.Packets.Headers;

namespace StsServer.Network.Packets
{
    class StsPacketManager
    {
        static readonly ConcurrentDictionary<StsMessage, HandleAuthPacket> AuthMessageHandlers = new ConcurrentDictionary<StsMessage, HandleAuthPacket>();
        delegate void HandleAuthPacket(StsPacket packet, StsSession client);

        public static void DefineMessageHandler()
        {
            var currentAsm = Assembly.GetExecutingAssembly();

            foreach (var type in currentAsm.GetTypes())
                foreach (var methodInfo in type.GetMethods())
                    foreach (var msgAttr in methodInfo.GetCustomAttributes<StsMessageAttribute>())
                        AuthMessageHandlers.TryAdd(msgAttr.Message, Delegate.CreateDelegate(typeof(HandleAuthPacket), methodInfo) as HandleAuthPacket);
        }

        public static bool Invoke(StsPacket reader, StsSession session)
        {
            var message = ((StsHeader)reader.Header).Message;

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

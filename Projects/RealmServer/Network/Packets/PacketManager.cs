// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Reflection;
using RealmServer.Attributes;
using RealmServer.Constants.Net;

namespace RealmServer.Network.Packets
{
    class PacketManager
    {
        static readonly ConcurrentDictionary<ClientMessage, HandlePacket> ClientMessageHandlers = new ConcurrentDictionary<ClientMessage, HandlePacket>();
        delegate void HandlePacket(Packet packet, RealmSession client);

        public static void DefineMessageHandler()
        {
            var currentAsm = Assembly.GetExecutingAssembly();

            foreach (var type in currentAsm.GetTypes())
                foreach (var methodInfo in type.GetMethods())
                    foreach (var msgAttr in methodInfo.GetCustomAttributes<RealmPacketAttribute>())
                        ClientMessageHandlers.TryAdd(msgAttr.Message, Delegate.CreateDelegate(typeof(HandlePacket), methodInfo) as HandlePacket);
        }

        public static bool Invoke(Packet reader, RealmSession session)
        {
            var message = (ClientMessage)reader.Header.Message;

            Console.WriteLine($"Received Realm packet: {message} (0x{message:X}), Length: {reader.Data.Length}");

            HandlePacket packet;

            if (ClientMessageHandlers.TryGetValue(message, out packet))
            {
                packet.Invoke(reader, session);

                return true;
            }

            return false;
        }
    }
}

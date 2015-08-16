// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Framework.Attributes;
using Framework.Constants.Net;
using Framework.Network;
using Framework.Packets.Client.Misc;

namespace Framework.Packets.Handlers
{
    class MiscHandler
    {
        [GlobalNetMessage(GlobalClientMessage.State1)]
        public static void HandleState1(State1 state1, SessionBase session)
        {
            // Send same data back for now.
            session.SendRaw(state1.State);
        }

        [GlobalNetMessage(GlobalClientMessage.State2)]
        public static void HandleState2(State2 state2, SessionBase session)
        {
            // Send same data back for now.
            session.SendRaw(state2.State);
        }
    }
}

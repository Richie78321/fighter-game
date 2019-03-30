using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Network.Server;
using ProtoBuf;

namespace FighterGame.Network.Message
{
    [ProtoContract(SkipConstructor = true)]
    class TestMessage : IServerMessage
    {
        public void ServerAction(Lobby lobby, Server.Client client)
        {
            lobby.PrintMethod.Invoke("TEST MESSAGE ARRIVED FROM: " + client.ClientInfo.Username);
        }
    }
}

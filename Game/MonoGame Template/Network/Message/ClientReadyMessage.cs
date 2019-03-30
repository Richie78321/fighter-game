using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Network.Server;
using Microsoft.Xna.Framework;
using ProtoBuf;

namespace FighterGame.Network.Message
{
    [ProtoContract(SkipConstructor = true)]
    public class ClientReadyMessage : IServerMessage
    {
        public void ServerAction(Lobby lobby, Server.Client client)
        {
            client.IsReady = true;
            lobby.CheckReadiness();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Network.Server;
using FighterGame.Network;
using ProtoBuf;

namespace FighterGame.Network.Message
{
    [ProtoContract(SkipConstructor = true)]
    public interface IServerMessage : INetworkMessage
    {
        void ServerAction(Lobby lobby, Server.Client client);
    }
}

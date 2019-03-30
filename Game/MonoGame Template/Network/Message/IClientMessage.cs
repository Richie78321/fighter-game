using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Runtime;
using ProtoBuf;

namespace FighterGame.Network.Message
{
    [ProtoContract(SkipConstructor = true)]
    public interface IClientMessage : INetworkMessage
    {
        void ClientAction(GameSession gameSession);
    }
}

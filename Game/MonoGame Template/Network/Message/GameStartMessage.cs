using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Runtime;
using ProtoBuf;

namespace FighterGame.Network.Message
{
    [Serializable]
    [ProtoContract(SkipConstructor = true)]
    public class GameStartMessage : IClientMessage
    {
        public void ClientAction(GameSession gameSession)
        {
            gameSession.StartGame();
        }
    }
}

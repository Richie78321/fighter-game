using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Runtime;
using FighterGame.Network.Client;
using ProtoBuf;

namespace FighterGame.Network.Message
{
    [Serializable]
    [ProtoContract(SkipConstructor = true)]
    class ServerInfoMessage : IClientMessage
    {
        [ProtoMember(1)]
        private ServerInfo serverInfo;

        public ServerInfoMessage(ServerInfo serverInfo)
        {
            this.serverInfo = serverInfo;
        }

        public void ClientAction(GameSession gameSession)
        {
            gameSession.NetworkManager.SetServerInfo(serverInfo);
        }
    }
}

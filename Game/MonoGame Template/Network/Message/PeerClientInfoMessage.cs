using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Network.Client;
using FighterGame.Runtime;
using ProtoBuf;

namespace FighterGame.Network.Message
{
    [ProtoContract(SkipConstructor = true)]
    public class PeerClientInfoMessage : IClientMessage
    {
        [ProtoMember(1)]
        private PeerClientInfo peerClientInfo;

        public PeerClientInfoMessage(PeerClientInfo peerClientInfo)
        {
            this.peerClientInfo = peerClientInfo;
        }

        public void ClientAction(GameSession gameSession)
        {
            gameSession.NetworkManager.AddPeerClientInfo(peerClientInfo);
        }
    }
}

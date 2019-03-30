using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Runtime;
using FighterGame.Network.Server;
using ProtoBuf;
using FighterGame.Network.Client;

namespace FighterGame.Network.Message
{
    [Serializable]
    [ProtoContract(SkipConstructor = true)]
    public class ClientDisconnectMessage : IClientMessage
    {
        [ProtoMember(1)]
        private byte clientIndex;
        public ClientDisconnectMessage(Server.Client disconnectingClient)
        {
            clientIndex = disconnectingClient.ClientIndex;
        }

        public void ClientAction(GameSession gameSession)
        {
            //Remove all associated entities
            gameSession.gameMap.RemoveClientEntities(clientIndex);

            PeerClientInfo peerClientInfo = gameSession.NetworkManager.GetClientInfo(clientIndex);
            if (peerClientInfo != null) gameSession.graphicsUI.MessageManager.AddMessage(peerClientInfo.Username + " has disconnected!");
            else gameSession.graphicsUI.MessageManager.AddMessage("Client " + clientIndex + " has disconnected!");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Network.Server;
using FighterGame.Runtime;
using ProtoBuf;

namespace FighterGame.Network.Message
{
    [ProtoContract(SkipConstructor = true), ProtoInclude(7832, typeof(PlayerRemovalMessage))]
    public class EntityRemovalMessage : IClientMessage, IServerMessage
    {
        [ProtoMember(1)]
        protected int entityIndex;
        [ProtoMember(2)]
        protected int clientIndex;
        [ProtoMember(3)]
        protected bool serverEntity;

        public EntityRemovalMessage(NetworkManager networkManager, int entityIndex, bool serverEntity = false)
        {
            this.serverEntity = serverEntity;
            clientIndex = networkManager.ClientIndex;
            this.entityIndex = entityIndex;
        }

        public virtual void ClientAction(GameSession gameSession)
        {
            if (!serverEntity)
            {
                gameSession.gameMap.RemoveEntity(clientIndex, entityIndex);
            }
        }

        public virtual void ServerAction(Lobby lobby, Server.Client client)
        {
            if (serverEntity)
            {
                lobby.RemoveNetworkEntity(clientIndex, entityIndex);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Runtime;
using FighterGame.Entities;
using ProtoBuf;

namespace FighterGame.Network.Message
{
    [Serializable]
    [ProtoContract(SkipConstructor = true)]
    class NetworkEntityDataMessage : IClientMessage
    {
        [ProtoMember(1)]
        private NetworkEntityData networkEntityData;

        public NetworkEntityDataMessage(NetworkEntityData networkEntityData)
        {
            this.networkEntityData = networkEntityData;
        }

        public void ClientAction(GameSession gameSession)
        {
            if (gameSession.gameMap != null)
            {
                //Check for existing
                Entity[] entities = gameSession.gameMap.Entities;
                for (int i = 0; i < entities.Length; i++)
                {
                    if (entities[i] is NetworkEntity networkEntity)
                    {
                        if (networkEntity.ClientIndex == networkEntityData.ClientIndex && networkEntity.EntityIndex == networkEntityData.EntityIndex)
                        {
                            //Set data
                            networkEntityData.SetNetworkEntity(networkEntity, gameSession);
                            return;
                        }
                    }
                }

                //None exist, create new
                gameSession.gameMap.AddEntity(networkEntityData.DeployNetworkEntity(gameSession.LatestGameTime, gameSession, null));
            }
        }
    }
}

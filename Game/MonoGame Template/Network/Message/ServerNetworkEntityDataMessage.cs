using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using FighterGame.Entities;
using FighterGame.Network.Server;

namespace FighterGame.Network.Message
{
    [Serializable]
    [ProtoContract(SkipConstructor = true)]
    class ServerNetworkEntityDataMessage : IServerMessage
    {
        [ProtoMember(1)]
        private NetworkEntityData networkEntityData;

        public ServerNetworkEntityDataMessage(NetworkEntityData networkEntityData)
        {
            this.networkEntityData = networkEntityData;
        }

        public void ServerAction(Lobby lobby, Server.Client client)
        {
            lock (lobby.entityCheckLock)
            {
                //Check for existing
                NetworkEntity[] networkEntities = lobby.NetworkEntities;
                for (int i = 0; i < networkEntities.Length; i++)
                {
                    if (networkEntities[i].ClientIndex == networkEntityData.ClientIndex && networkEntities[i].EntityIndex == networkEntityData.EntityIndex)
                    {
                        //Set data
                        networkEntityData.SetNetworkEntity(networkEntities[i], null);
                        return;
                    }
                }

                //None exist, create new
                lobby.AddNetworkEntity(networkEntityData.DeployNetworkEntity(null, null, lobby));
            }
        }
    }
}

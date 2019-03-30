using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Entities;
using FighterGame.Entities.Player;
using FighterGame.Map.Effects;
using FighterGame.Runtime;
using PolygonalPhysics;
using ProtoBuf;

namespace FighterGame.Network.Message
{
    [ProtoContract(SkipConstructor = true)]
    class PlayerRemovalMessage : EntityRemovalMessage
    {
        [ProtoMember(1)]
        private bool onlyEffect;

        public PlayerRemovalMessage(NetworkManager networkManager, int entityIndex, bool onlyEffect = false) : base(networkManager, entityIndex, false)
        {
            this.onlyEffect = onlyEffect;
        }

        public override void ClientAction(GameSession gameSession)
        {
            NetworkEntity[] matchingEntities = gameSession.gameMap.NetworkEntities.Where(t => t.EntityIndex == entityIndex && t.ClientIndex == clientIndex).ToArray();
            for (int i = 0; i < matchingEntities.Length; i++)
            {
                if (matchingEntities[i] is NetworkPlayer b)
                {
                    if (!onlyEffect)
                    {
                        b.IsDead = true;
                        b.HealthBar.SetCurrentHealth(0, gameSession.LatestGameTime);
                    }
                    b.PlayerParticleManager.SpawnDeathParticles(gameSession.gameMap);
                    break;
                }
            }

            base.ClientAction(gameSession);
        }
    }
}

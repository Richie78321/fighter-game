using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Network.Server;
using FighterGame.Runtime;
using Microsoft.Xna.Framework;
using ProtoBuf;

namespace FighterGame.Entities.Player
{
    public class SimpleNetworkPlayerData : NetworkEntityData
    {
        public PlayerStandards.PlayerType playerType;
        public float playerPositionX;
        public float playerPositionY;

        public SimpleNetworkPlayerData(LocalEntity entity, int clientIndex) : base(entity, clientIndex)
        {
        }

        protected override NetworkEntity CreateNetworkEntity(GameTime gameTime, GameSession gameSession, Lobby lobby, int clientIndex, int entityIndex)
        {
            NetworkPlayer networkPlayer = new NetworkPlayer();
            networkPlayer.LoadPlayer(playerType, gameSession, lobby, clientIndex, entityIndex);
            return networkPlayer;
        }

        public override void SetNetworkEntity(NetworkEntity networkEntity, GameSession gameSession)
        {
            if (networkEntity is NetworkPlayer b)
            {
                b.PlayerPosition = new Vector2(playerPositionX, playerPositionY);
            }
            else throw new TypeDisagreementException();
        }

        protected override void CollectData(LocalEntity entity)
        {
            if (entity is LocalPlayer b)
            {
                playerPositionX = b.RigidBody.CollisionPolygon.CenterPoint.X;
                playerPositionY = b.RigidBody.CollisionPolygon.CenterPoint.Y;
                playerType = b.PlayerType;
            }
            else throw new TypeDisagreementException();
        }
    }
}

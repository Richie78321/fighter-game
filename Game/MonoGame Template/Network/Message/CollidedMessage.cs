using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Runtime;
using ProtoBuf;
using PolygonalPhysics;
using Microsoft.Xna.Framework;
using FighterGame.Entities.Server.CollisionDetection;
using FighterGame.Entities;

namespace FighterGame.Network.Message
{
    [ProtoContract(SkipConstructor = true)]
    public class CollidedMessage : IClientMessage
    {
        [ProtoMember(1)]
        private ColliderInfo collidedInfo;

        [ProtoMember(2)]
        private ColliderInfo colliderInfo;

        [ProtoMember(3)]
        private PortableVector2 collisionPosition;

        public CollidedMessage(ColliderInfo collidedInfo, ColliderInfo colliderInfo, Vector2 collisionPosition)
        {
            this.collidedInfo = collidedInfo;
            this.colliderInfo = colliderInfo;
            this.collisionPosition = new PortableVector2(collisionPosition);
        }

        public void ClientAction(GameSession gameSession)
        {
            bool amCollided = false;
            LocalEntity localEntity;
            if (collidedInfo.clientIndex == gameSession.NetworkManager.ClientIndex)
            {
                amCollided = true;
                localEntity = gameSession.gameMap.GetLocalEntity(collidedInfo.entityIndex);
            }
            else localEntity = gameSession.gameMap.GetLocalEntity(colliderInfo.entityIndex);

            if (localEntity != null && localEntity is ClientCollider b)
            {
                b.HandleCollision(gameSession, collidedInfo, colliderInfo, collisionPosition.Vector2, amCollided);
            }
            else throw new Exception("Error with ClientCollider!");
        }
    }

    [ProtoContract(SkipConstructor = true, ImplicitFields = ImplicitFields.AllPublic)]
    public class ColliderInfo
    {
        public readonly int clientIndex;
        public readonly int entityIndex;
        public readonly PortableVector2 colliderPosition;

        public readonly float CollisionDamage;

        public ColliderInfo(int clientIndex, int entityIndex, Vector2 colliderPosition, float CollisionDamage)
        {
            this.CollisionDamage = CollisionDamage;
            this.clientIndex = clientIndex;
            this.entityIndex = entityIndex;
            this.colliderPosition = new PortableVector2(colliderPosition);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Network.Server;
using FighterGame.Runtime;
using Microsoft.Xna.Framework;
using PolygonalPhysics;

namespace FighterGame.Entities.Server.CollisionDetection
{
    public class FullColliderData : NetworkEntityData
    {
        public PortablePolygon CollisionPolygon;

        public bool ActiveCollider;

        public float CollisionDamage;

        public bool Enabled;

        public FullColliderData(ClientCollider clientCollider, int clientIndex) : base(clientCollider, clientIndex)
        {
        }

        public override void SetNetworkEntity(NetworkEntity networkEntity, GameSession gameSession)
        {
            if (networkEntity is ServerCollider b)
            {
                b.UpdateInfo(CollisionPolygon.DeployPolygon(), ActiveCollider, CollisionDamage, Enabled);
            }
            else throw new TypeDisagreementException();
        }

        protected override void CollectData(LocalEntity entity)
        {
            if (entity is ClientCollider b)
            {
                CollisionPolygon = new PortablePolygon(b.CollisionPolygon);
                ActiveCollider = b.ActiveCollider;
                CollisionDamage = b.CollisionDamage;
                Enabled = b.Enabled;
            }
            else throw new TypeDisagreementException();
        }

        protected override NetworkEntity CreateNetworkEntity(GameTime gameTime, GameSession gameSession, Lobby lobby, int clientIndex, int entityIndex)
        {
            return new ServerCollider(CollisionPolygon.DeployPolygon(), lobby, ActiveCollider, ClientIndex, EntityIndex, CollisionDamage, Enabled);
        }
    }
}

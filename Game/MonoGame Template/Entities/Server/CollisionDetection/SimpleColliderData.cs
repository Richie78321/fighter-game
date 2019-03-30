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
    public class SimpleColliderData : NetworkEntityData
    {
        public PortableVector2 PolygonLocation;

        public bool ActiveCollider;

        public float CollisionDamage;

        public bool Enabled;

        public SimpleColliderData(ClientCollider clientCollider, int clientIndex) : base(clientCollider, clientIndex)
        {
        }

        public override void SetNetworkEntity(NetworkEntity networkEntity, GameSession gameSession)
        {
            if (networkEntity is ServerCollider b)
            {
                b.UpdateInfo(PolygonLocation.Vector2, ActiveCollider, CollisionDamage, Enabled);
            }
            else throw new TypeDisagreementException();
        }

        protected override void CollectData(LocalEntity entity)
        {
            if (entity is ClientCollider b)
            {
                PolygonLocation = new PortableVector2(b.CollisionPolygon.CenterPoint);
                ActiveCollider = b.ActiveCollider;
                CollisionDamage = b.CollisionDamage;
                Enabled = b.Enabled;
            }
            else throw new TypeDisagreementException();
        }

        protected override NetworkEntity CreateNetworkEntity(GameTime gameTime, GameSession gameSession, Lobby lobby, int clientIndex, int entityIndex)
        {
            return new ServerCollider(new Polygon(new Vector2[] { Vector2.Zero, Vector2.Zero, Vector2.Zero }), lobby, false, ClientIndex, EntityIndex, 0, false);
        }
    }
}

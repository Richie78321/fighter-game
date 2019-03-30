using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonalPhysics;
using FighterGame.Network.Server;
using FighterGame.Network.Message;

namespace FighterGame.Entities.Server.CollisionDetection
{
    class ServerCollider : NetworkEntity
    {
        private Collider collider;
        private Lobby lobby;

        private float collisionDamage;
        public float CollisionDamage => collisionDamage;

        public ServerCollider(Polygon collisionPolygon, Lobby lobby, bool activeCollider, int clientIndex, int entityIndex, float collisionDamage, bool enabled)
        {
            this.collisionDamage = collisionDamage;
            this.activeCollider = activeCollider;
            collider = new Collider(collisionPolygon, lobby.ColliderReferenceMap);
            SetClientIndex(clientIndex);
            SetEntityIndex(entityIndex);
            this.enabled = enabled;

            this.lobby = lobby;
        }

        private bool activeCollider;

        private bool enabled;
        public bool IsEnabled => enabled;

        public void UpdateInfo(Vector2 newPosition, bool activeCollider, float collisionDamage, bool enabled)
        {
            this.enabled = enabled;
            this.collisionDamage = collisionDamage;
            bool nowActive = false;
            if (!this.activeCollider && activeCollider) nowActive = true;
            this.activeCollider = activeCollider;

            ColliderEvent[] colliderEvents = collider.Translate(newPosition, ignoreCurrentlyColliding: nowActive);
            if (enabled && activeCollider)
            {
                //Find associated clients
                NetworkEntity[] networkEntities = lobby.NetworkEntities.Where(t => t is ServerCollider).ToArray();
                ColliderInfo[] collidedInfo = new ColliderInfo[colliderEvents.Length];

                for (int i = 0; i < collidedInfo.Length; i++)
                {
                    for (int j = 0; j < networkEntities.Length; j++)
                    {
                        if (networkEntities[j] is ServerCollider b && b.IsEnabled && b.collider == colliderEvents[i].CollidedWith)
                        {
                            collidedInfo[i] = b.ColliderInfo;
                            break;
                        }
                    }
                }

                //Forward to associated clients
                ColliderInfo colliderInfo = ColliderInfo;
                for (int i = 0; i < collidedInfo.Length; i++)
                {
                    if (collidedInfo[i] != null)
                    {
                        //Send message to both involved
                        CollidedMessage collidedMessage = new CollidedMessage(collidedInfo[i], colliderInfo, colliderEvents[i].CollisionPoint);
                        lobby.SendToClient(collidedInfo[i].clientIndex, collidedMessage);
                        lobby.SendToClient(ClientIndex, collidedMessage);
                    }
                }
            }
        }

        public void UpdateInfo(Polygon newPolygon, bool activeCollider, float collisionDamage, bool enabled)
        {
            Vector2 originalCenterPoint = newPolygon.CenterPoint;
            newPolygon.Translate(collider.CollisionPolygon.CenterPoint - newPolygon.CenterPoint);
            collider.CollisionPolygon = newPolygon;
            UpdateInfo(originalCenterPoint, activeCollider, collisionDamage, enabled);
        }

        public override void Draw(SpriteBatch spriteBatch, GameMap map)
        {
            throw new NotImplementedException();
        }

        protected override void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map)
        {
            throw new NotImplementedException();
        }

        public ColliderInfo ColliderInfo => new ColliderInfo(ClientIndex, EntityIndex, collider.CollisionPolygon.CenterPoint, collisionDamage);
    }
}

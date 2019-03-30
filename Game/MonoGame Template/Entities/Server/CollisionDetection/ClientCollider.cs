using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Map;
using FighterGame.Network.Message;
using FighterGame.Runtime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonalPhysics;

namespace FighterGame.Entities.Server.CollisionDetection
{
    public class ClientCollider : LocalEntity
    {
        public delegate void CollisionEvent(GameSession gameSession, ColliderInfo collidedInfo, ColliderInfo colliderInfo, Vector2 collisionPosition, bool amCollided);

        //Object
        public float CollisionDamage = 0;

        private bool polygonChanged = false;

        private Polygon _collisionPolygon;
        public Polygon CollisionPolygon => _collisionPolygon;
        public void SetCollisionPolygon(Polygon collisionPolygon)
        {
            Vector2 beforePosition = _collisionPolygon.CenterPoint;
            _collisionPolygon = new Polygon(collisionPolygon.Vertices);
            _collisionPolygon.CenterPoint = collisionPolygon.CenterPoint;
            _collisionPolygon.Translate(beforePosition - _collisionPolygon.CenterPoint);
            polygonChanged = true;
            facingState = SpriteManager.FacingStates.Right;
        }

        public object CollisionCooldownLock = new object();
        private List<CollisionCooldown> collisionCooldowns = new List<CollisionCooldown>();

        public bool ActiveCollider = false;

        public bool Enabled = true;

        private SpriteManager.FacingStates facingState;
        public SpriteManager.FacingStates FacingState => facingState;
        public void SetFacingState(SpriteManager.FacingStates newState)
        {
            if (facingState != newState)
            {
                CollisionPolygon.Reflect(new Line(CollisionPolygon.CenterPoint, float.PositiveInfinity));
                facingState = newState;
                polygonChanged = true;
            }
        }

        public ClientCollider(Polygon collisionPolygon, CollisionEvent collisionEvent)
        {
            OnCollision = collisionEvent;
            this._collisionPolygon = collisionPolygon;
        }

        public override void Draw(SpriteBatch spriteBatch, GameMap map)
        {
        }

        protected override void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map)
        {
            //Prune collision cooldowns
            lock (CollisionCooldownLock)
            {
                List<CollisionCooldown> expiredCooldowns = new List<CollisionCooldown>();
                for (int i = 0; i < collisionCooldowns.Count; i++) if (collisionCooldowns[i].HasExpired(gameTime)) expiredCooldowns.Add(collisionCooldowns[i]);
                collisionCooldowns.RemoveAll(t => expiredCooldowns.Contains(t));
            }
        }

        public void HandleCollision(GameSession gameSession, ColliderInfo collidedInfo, ColliderInfo colliderInfo, Vector2 collisionPosition, bool amCollided)
        {
            lock (CollisionCooldownLock)
            {
                //Check for existing collision cooldown
                bool inCooldown = false;
                for (int i = 0; i < collisionCooldowns.Count; i++)
                {
                    if (collisionCooldowns[i].collidedInfo.clientIndex == collidedInfo.clientIndex && collisionCooldowns[i].collidedInfo.entityIndex == collidedInfo.entityIndex && collisionCooldowns[i].colliderInfo.clientIndex == colliderInfo.clientIndex && collisionCooldowns[i].colliderInfo.entityIndex == colliderInfo.entityIndex)
                    {
                        inCooldown = true;
                        break;
                    }
                }

                if (!inCooldown)
                {
                    //Add collision cooldown
                    collisionCooldowns.Add(new CollisionCooldown(gameSession.LatestGameTime.TotalGameTime.TotalMilliseconds, colliderInfo, collidedInfo));

                    //Handle collision
                    OnCollision.Invoke(gameSession, collidedInfo, colliderInfo, collisionPosition, amCollided);
                }
                else return;
            }
        }

        private CollisionEvent OnCollision;

        private bool deployed = false;
        public override void SendNetworkEntityData(NetworkManager networkManager)
        {
            if (!deployed || polygonChanged)
            {
                networkManager.SendNetworkEntityData(new FullColliderData(this, networkManager.ClientIndex), server: true);
                deployed = true;
                polygonChanged = false;
            }
            else networkManager.SendNetworkEntityData(new SimpleColliderData(this, networkManager.ClientIndex), server: true);
        }
    }

    class CollisionCooldown
    {
        private const int DEFAULT_COOLDOWN_TIME = 200;

        //Object
        public readonly double TimeOfCollision;
        public readonly int CollisionCooldownTime;

        public readonly ColliderInfo colliderInfo;
        public readonly ColliderInfo collidedInfo;

        public CollisionCooldown(double TimeOfCollision, ColliderInfo colliderInfo, ColliderInfo collidedInfo, int CollisionCooldownTime = DEFAULT_COOLDOWN_TIME)
        {
            this.TimeOfCollision = TimeOfCollision;
            this.collidedInfo = collidedInfo;
            this.colliderInfo = colliderInfo;
            this.CollisionCooldownTime = CollisionCooldownTime;
        }

        public bool HasExpired(GameTime gameTime) { return gameTime.TotalGameTime.TotalMilliseconds - TimeOfCollision >= CollisionCooldownTime; }
    }
}

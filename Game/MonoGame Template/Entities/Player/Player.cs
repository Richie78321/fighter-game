using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FighterGame.Textures;
using Microsoft.Xna.Framework.Input;
using FighterGame.Map;
using PolygonalPhysics;
using FighterGame.Runtime;
using FighterGame.Entities.Server.CollisionDetection;
using FighterGame.GUI;
using FighterGame.Network.Message;
using FighterGame.Entities.Player.PlayerType;
using FighterGame.Map.Effects;
using FighterGame.GUI.Health;

namespace FighterGame.Entities.Player
{
    public abstract class Player : DamageableEntity
    {
        public static bool InFocus = true;
        public const float PLAYER_HEALTH = 100F;
        public const float DEFAULT_PLAYER_DAMAGE = 5F;
        public const int PLAYER_LIVES = 3;

        //Object
        protected RigidBody rigidBody;
        public RigidBody RigidBody => rigidBody;

        private Vector2 drawDimensions;
        public Vector2 DrawDimensions => new Vector2(drawDimensions.X, drawDimensions.Y);

        public readonly SpriteManager SpriteManager;
        private GameMap gameMap;

        private HealthBar healthBar;

        public readonly PlayerTexturePack PlayerTexturePack;

        protected PlayerParticleManager playerParticleManager;

        public Player(PlayerTexturePack texturePack, GameTime gameTime, RigidBody rigidBody, Vector2 drawDimensions, GameMap gameMap) : base(PLAYER_HEALTH, PLAYER_LIVES, rigidBody.CollisionPolygon, gameMap)
        {
            playerParticleManager = new PlayerParticleManager(texturePack, rigidBody.CollisionPolygon, drawDimensions);
            PlayerTexturePack = texturePack;
            this.gameMap = gameMap;
            SpriteManager = new SpriteManager(texturePack, gameTime);
            this.drawDimensions = drawDimensions;
            this.rigidBody = rigidBody;
            ClientCollider.CollisionDamage = DEFAULT_PLAYER_DAMAGE;

            healthBar = gameMap.gameSession.graphicsUI.HealthBarManager.AddHealthBar(gameMap.gameSession.DatabaseManager.Username, PLAYER_HEALTH, gameTime);
        }

        protected override void OnCollision(GameSession gameSession, ColliderInfo collidedInfo, ColliderInfo colliderInfo, Vector2 collisionPosition, bool amCollided)
        {
            if (amCollided)
            {
                //Knockback
                float angleOfCollision = (float)Math.Atan2(colliderInfo.colliderPosition.Y - rigidBody.CollisionPolygon.CenterPoint.Y, colliderInfo.colliderPosition.X - rigidBody.CollisionPolygon.CenterPoint.X);
                Vector2 velocityVector = new Vector2(-(float)Math.Cos(angleOfCollision), -(float)Math.Sin(angleOfCollision) - .5F);
                rigidBody.AddTranslationalVelocity(velocityVector);

                if (colliderInfo.colliderPosition.X < rigidBody.CollisionPolygon.CenterPoint.X) SpriteManager.FacingState = SpriteManager.FacingStates.Left;
                else SpriteManager.FacingState = SpriteManager.FacingStates.Right;

                //Damage
                //rigidBody.Mass = (CurrentHealth / 200) + .5F;

                //Spawn blood
                playerParticleManager.SpawnBlood(gameSession.gameMap);

                //Spawn damage indicator
                playerParticleManager.SpawnDamageNotifier(-colliderInfo.CollisionDamage, gameSession.gameMap);
            }
            else
            {
                RectangleF boundaryRectangle = rigidBody.CollisionPolygon.BoundaryRectangle;
                PlayerParticleManager PPM = new PlayerParticleManager(PlayerTexturePack, new RotationRectangle(new RectangleF(collidedInfo.colliderPosition.X - (boundaryRectangle.Width / 2), collidedInfo.colliderPosition.Y - (boundaryRectangle.Height / 2), boundaryRectangle.Width, boundaryRectangle.Height)), drawDimensions);
                PPM.SpawnBlood(gameMap);
                PPM.SpawnDamageNotifier(colliderInfo.CollisionDamage, gameMap);
            }
        }

        protected override void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map)
        {
            SpriteManager.Update(gameTime);
            ClientCollider.SetFacingState(SpriteManager.FacingState);

            //Ensure in bounds
            if (rigidBody.CollisionPolygon.CenterPoint.Y > MapStandards.MAP_SIZE)
                ApplyDamage(float.MaxValue, map.gameSession);
        }

        public override void Draw(SpriteBatch spriteBatch, GameMap map)
        {
            SpriteEffects spriteEffect;
            Texture2D playerTexture = SpriteManager.GetPlayerTexture(out spriteEffect);
            spriteBatch.Draw(playerTexture, rigidBody.CollisionPolygon.CenterPoint, null, Color.White, rigidBody.TotalRotation, SpriteManager.SpriteCenter, new Vector2(DrawDimensions.X / playerTexture.Width, DrawDimensions.Y / playerTexture.Height), spriteEffect, 0);
            if (PlayerTexturePack.IndicatorTexture != null) spriteBatch.Draw(PlayerTexturePack.IndicatorTexture, new Vector2(rigidBody.CollisionPolygon.CenterPoint.X, rigidBody.CollisionPolygon.CenterPoint.Y - (DrawDimensions.Y / 2)), null, Color.White, 0, new Vector2(PlayerTexturePack.IndicatorTexture.Width / 2, PlayerTexturePack.IndicatorTexture.Height / 2), DrawDimensions.X / (Math.Max(PlayerTexturePack.IndicatorTexture.Width, PlayerTexturePack.IndicatorTexture.Height) * 3), SpriteEffects.None, 0);
        }

        protected override void OnHealthChange()
        {
            healthBar.SetCurrentHealth(CurrentHealth, gameMap.gameSession.LatestGameTime);
        }

        protected override void OnConsumeLife()
        {
            rigidBody.SetTranslationalVelocity(Vector2.Zero, isGlobal: false);
            playerParticleManager.SpawnDeathParticles(gameMap);
            rigidBody.CollisionPolygon.Translate(GetSpawnPosition() - rigidBody.CollisionPolygon.CenterPoint);
        }

        protected Vector2 GetSpawnPosition()
        {
            Platform spawnPlatform = gameMap.MapArchitecture.MapPlatforms[gameMap.gameSession.LocalRandom.Next(0, gameMap.MapArchitecture.MapPlatforms.Length)];
            int spawnTile = gameMap.gameSession.LocalRandom.Next(0, spawnPlatform.TileLength);
            return new Vector2(spawnPlatform.Position.X + (MapStandards.TILE_SIZE * (spawnTile + .5F)) - (rigidBody.CollisionPolygon.BoundaryRectangle.Width / 2), spawnPlatform.Position.Y - rigidBody.CollisionPolygon.BoundaryRectangle.Height);
        }
    }
}

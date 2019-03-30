using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Entities;
using FighterGame.Runtime;
using FighterGame.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonalPhysics;

namespace FighterGame.Map.Effects
{
    class SpriteParticle : Particle
    {
        private BasicSpriteManager spriteManager;
        public static RectangleF FitRecToSprite(RectangleF collisionRectangle, Texture2D spriteSample)
        {
            float scale = Math.Min(collisionRectangle.Width / spriteSample.Width, collisionRectangle.Height / spriteSample.Height);
            return new RectangleF(collisionRectangle.X + ((collisionRectangle.Width - (spriteSample.Width * scale)) / 2), collisionRectangle.Y + ((collisionRectangle.Height - (spriteSample.Height * scale)) / 2), spriteSample.Width * scale, spriteSample.Height * scale);
        }

        public SpriteParticle(GameMap gameMap, GameTime gameTime, RectangleF collisionRectangle, Vector2 initialVelocity, SpriteSheet spriteSheet, float initialAngularVelocity = 0, int lifetime = Particle.DEFAULT_PARTICLE_LIFETIME, bool endAfterLoop = false, bool ignoreCollisions = false, bool enableRotation = false, RigidBody.VelocityRestriction[] allowedCollisionsOverride = null) : base(gameMap, gameTime, FitRecToSprite(collisionRectangle, spriteSheet.textures[0]), initialVelocity, lifetime, ignoreCollisions, enableRotation: enableRotation)
        {
            this.gameMap = gameMap;
            spriteManager = new BasicSpriteManager(gameTime, spriteSheet);
            if (endAfterLoop) spriteManager.OnSpriteEnd += SpriteManager_OnSpriteEnd;
            rigidBody.AddAngularVelocity(initialAngularVelocity, isGlobal: false);
            if (allowedCollisionsOverride != null) rigidBody.AllowedCollisions = allowedCollisionsOverride;
        }

        private GameMap gameMap;

        private void SpriteManager_OnSpriteEnd(object sender, EventArgs e)
        {
            Dispose(gameMap);
        }

        protected override void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map)
        {
            base.Update(gameTime, keyboardState, pastKeyboardState, mouseState, pastMouseState, map);

            spriteManager.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameMap map)
        {
            spriteBatch.Draw(spriteManager.CurrentTexture, rigidBody.CollisionPolygon.CenterPoint, null, Color.White, rigidBody.TotalRotation, new Vector2(spriteManager.CurrentTexture.Width / 2, spriteManager.CurrentTexture.Height / 2), Math.Min(collisionRectangle.Width / spriteManager.CurrentTexture.Width, collisionRectangle.Height / spriteManager.CurrentTexture.Height), SpriteEffects.None, 0);
        }

        public override void SendNetworkEntityData(NetworkManager networkManager)
        {
        }
    }
}

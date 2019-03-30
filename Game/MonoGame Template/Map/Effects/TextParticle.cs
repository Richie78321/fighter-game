using FighterGame.GUI;
using FighterGame.Runtime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonalPhysics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterGame.Map.Effects
{
    class TextParticle : Particle
    {
        public static SpriteFont ParticleFont = GameButton.TextFont;

        private static RectangleF FindScale(string particleText, RectangleF collisionRectangle)
        {
            //Set text scale
            Vector2 measureString = ParticleFont.MeasureString(particleText);
            float textScale = Math.Min(collisionRectangle.Width / measureString.X, collisionRectangle.Height / measureString.Y);

            return new RectangleF(collisionRectangle.X + ((collisionRectangle.Width - (measureString.X * textScale)) / 2), collisionRectangle.Y + ((collisionRectangle.Height - (measureString.Y * textScale)) / 2), measureString.X * textScale, measureString.Y * textScale);
        }

        //Object
        private float textScale;
        private string particleText;
        private Color textColor;

        public TextParticle(string particleText, GameMap gameMap, GameTime gameTime, RectangleF collisionRectangle, Vector2 initialVelocity, Color textColor) : base(gameMap, gameTime, FindScale(particleText, collisionRectangle), initialVelocity)
        {
            this.particleText = particleText;
            this.textColor = textColor;

            Vector2 measureString = ParticleFont.MeasureString(particleText);
            textScale = Math.Min(collisionRectangle.Width / measureString.X, collisionRectangle.Height / measureString.Y);
        }


        public override void Draw(SpriteBatch spriteBatch, GameMap map)
        {
            Vector2 measureString = ParticleFont.MeasureString(particleText);
            spriteBatch.DrawString(ParticleFont, particleText, new Vector2(rigidBody.CollisionPolygon.BoundaryRectangle.Location.X + (rigidBody.CollisionPolygon.BoundaryRectangle.Width / 2), rigidBody.CollisionPolygon.BoundaryRectangle.Location.Y + (rigidBody.CollisionPolygon.BoundaryRectangle.Height / 2)), textColor, rigidBody.TotalRotation, new Vector2(measureString.X / 2, measureString.Y / 2), textScale, SpriteEffects.None, 0);
        }

        public override void SendNetworkEntityData(NetworkManager networkManager)
        {
        }
    }
}

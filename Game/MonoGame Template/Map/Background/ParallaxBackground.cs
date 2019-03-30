using FighterGame.Entities.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterGame.Map.Background
{
    public class ParallaxBackground
    {
        public const int MAX_BACKGROUND_DEPTH = 25;

        //Object
        private BackgroundImage[] backgroundImages;
        private float[] scales;

        public ParallaxBackground(BackgroundImage[] backgroundImages)
        {
            this.backgroundImages = backgroundImages;

            //Find maximum player padding
            float maximumScale = PlayerStandards.PlayerTypes[0].GetPlayerScale();
            for (int i = 1; i < PlayerStandards.PlayerTypes.Length; i++) if (PlayerStandards.PlayerTypes[i].GetPlayerScale() > maximumScale) maximumScale = PlayerStandards.PlayerTypes[i].GetPlayerScale();

            float additionalScalePadding = maximumScale * MapStandards.PLAYER_SCALE * (Camera.PLAYER_VIEW_PADDING - 1);
            scales = new float[backgroundImages.Length];
            for (int i = 0; i < scales.Length; i++)
            {
                float sizeRequirement = (MapStandards.MAP_SIZE * (1 + (1F / backgroundImages[i].Depth))) + additionalScalePadding;
                scales[i] = sizeRequirement / Math.Min(backgroundImages[i].BackgroundTexture.Width, backgroundImages[i].BackgroundTexture.Height);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 focusPoint, float scale, GraphicsDevice graphicsDevice)
        {
            float minScale = Math.Min(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height) / MapStandards.MAP_SIZE;
            for (int i = backgroundImages.Length - 1; i >= 0; i--)
            {
                float depthScale = minScale + ((scale - minScale) / backgroundImages[i].Depth);
                spriteBatch.Begin(transformMatrix: Matrix.CreateScale(depthScale), samplerState: SamplerState.PointClamp);
                spriteBatch.Draw(backgroundImages[i].BackgroundTexture, new Vector2((graphicsDevice.Viewport.Width / (2 * depthScale)) - ((focusPoint.X - (MapStandards.MAP_SIZE / 2)) / backgroundImages[i].Depth), (graphicsDevice.Viewport.Height / (2 * depthScale)) - ((focusPoint.Y - (MapStandards.MAP_SIZE / 2)) / backgroundImages[i].Depth)), null, Color.White, 0, new Vector2(backgroundImages[i].BackgroundTexture.Width / 2, backgroundImages[i].BackgroundTexture.Height /2), scales[i], SpriteEffects.None, 0);
                spriteBatch.End();
            }
        }
    }
}

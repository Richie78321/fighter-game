using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FighterGame.Textures
{
    public class SpriteSheet
    {
        public readonly Texture2D[] textures;
        public readonly int FrameTime;

        public SpriteSheet(Texture2D spriteSheetTexture, int tileWidth, int tileHeight, int FrameTime, GraphicsDevice graphicsDevice)
        {
            //Ensure proper dimensions
            if (spriteSheetTexture.Width % tileWidth != 0 || spriteSheetTexture.Height % tileHeight != 0) throw new Exception("Incorrect tile dimensions of SpriteSheet.");

            this.FrameTime = FrameTime;

            //Gather textures
            List<Texture2D> tileTextures = new List<Texture2D>();
            int tilePixelWidth = spriteSheetTexture.Width / tileWidth;
            int tilePixelHeight = spriteSheetTexture.Height / tileHeight;

            for (int j = 0; j < tileHeight; j++)
            {
                for (int i = 0; i < tileWidth; i++)
                {
                    Rectangle sourceRectangle = new Rectangle(tilePixelWidth * i, tilePixelHeight * j, tilePixelWidth, tilePixelHeight);
                    Texture2D tileTexture = new Texture2D(graphicsDevice, tilePixelWidth, tilePixelHeight);

                    Color[] data = new Color[tilePixelWidth * tilePixelHeight];
                    spriteSheetTexture.GetData(0, sourceRectangle, data, 0, data.Length);

                    tileTexture.SetData(data);
                    if (!IsEmpty(tileTexture)) tileTextures.Add(tileTexture);
                }
            }

            textures = tileTextures.ToArray();
        }

        private static bool IsEmpty(Texture2D texture)
        {
            Color[] colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);

            bool isEmpty = true;
            for (int i = 0; i < colorData.Length; i++)
            {
                if (colorData[i] != Color.Transparent)
                {
                    isEmpty = false;
                    break;
                }
            }

            return isEmpty;
        }
    }
}

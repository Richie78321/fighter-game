using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FighterGame.Entities;

namespace FighterGame.Map
{
    [Serializable]
    public class Platform
    {
        private float[] position;
        public Vector2 Position => new Vector2(position[0], position[1]);

        public RectangleF NonRelativeRectangle => new RectangleF(position[0], position[1], MapStandards.TILE_SIZE * TileLength, MapStandards.TILE_SIZE);
        public RectangleF RelativeRectangle(float scale) { return new RectangleF(position[0] * scale, position[1] * scale, MapStandards.TILE_SIZE * TileLength * scale, MapStandards.TILE_SIZE * scale); }

        public readonly int TileLength;

        public Platform(Vector2 position, int tileLength)
        {
            this.position = new float[] { position.X, position.Y };
            TileLength = tileLength;
        }

        public void Draw(SpriteBatch spriteBatch, MapTexturePack texturePack)
        {
            float textureScale = MapStandards.TILE_SIZE / texturePack.PlatformTexturePack.PlatformTop[0].Width;
            for (int i = 0; i < TileLength; i++)
            {
                if (i == 0)
                {
                    spriteBatch.Draw(texturePack.PlatformTexturePack.PlatformTop[(int)PlatformTexturePack.PlatformTexture.Left], new Vector2(position[0] + (i * MapStandards.TILE_SIZE), position[1]), null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, textureScale, SpriteEffects.None, 0);
                }
                else if (i == TileLength - 1)
                {
                    spriteBatch.Draw(texturePack.PlatformTexturePack.PlatformTop[(int)PlatformTexturePack.PlatformTexture.Right], new Vector2(position[0] + (i * MapStandards.TILE_SIZE), position[1]), null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, textureScale, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(texturePack.PlatformTexturePack.PlatformTop[(int)PlatformTexturePack.PlatformTexture.Middle], new Vector2(position[0] + (i * MapStandards.TILE_SIZE), position[1]), null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, textureScale, SpriteEffects.None, 0);
                }
            }

            for (int i = 0; i < TileLength; i++)
            {
                if (i == 0)
                {
                    spriteBatch.Draw(texturePack.PlatformTexturePack.PlatformBottom[(int)PlatformTexturePack.PlatformTexture.Left], new Vector2(position[0] + (i * MapStandards.TILE_SIZE), position[1] + MapStandards.TILE_SIZE), null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, textureScale, SpriteEffects.None, 0);
                }
                else if (i == TileLength - 1)
                {
                    spriteBatch.Draw(texturePack.PlatformTexturePack.PlatformBottom[(int)PlatformTexturePack.PlatformTexture.Right], new Vector2(position[0] + (i * MapStandards.TILE_SIZE), position[1] + MapStandards.TILE_SIZE), null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, textureScale, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(texturePack.PlatformTexturePack.PlatformBottom[(int)PlatformTexturePack.PlatformTexture.Middle], new Vector2(position[0] + (i * MapStandards.TILE_SIZE), position[1] + MapStandards.TILE_SIZE), null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, textureScale, SpriteEffects.None, 0);
                }
            }
        }
    }
}

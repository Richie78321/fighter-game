using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace FighterGame.Map
{
    [Serializable]
    public class MapArchitecture
    {
        private List<Platform> mapPlatforms = new List<Platform>();
        public Platform[] MapPlatforms => mapPlatforms.ToArray();
        public bool AddPlatform(Platform platformToAdd)
        {
            //Ensure in bounds
            if (platformToAdd.Position.X >= 0 && platformToAdd.Position.Y >= 0 && platformToAdd.Position.X + (MapStandards.TILE_SIZE * platformToAdd.TileLength) <= MapStandards.MAP_SIZE && platformToAdd.Position.Y + MapStandards.TILE_SIZE <= MapStandards.MAP_SIZE)
            {
                //In bounds, check overlap
                RectangleF platformRec = platformToAdd.NonRelativeRectangle;
                for (int i = 0; i < mapPlatforms.Count; i++) if (mapPlatforms[i].NonRelativeRectangle.IntersectsWith(platformRec)) return false;

                //No overlap, add
                mapPlatforms.Add(platformToAdd);
                return true;
            }
            else return false;
        }

        public void Draw(SpriteBatch spriteBatch, MapTexturePack texturePack)
        {
            //Draw platforms
            for (int i = 0; i < mapPlatforms.Count; i++) mapPlatforms[i].Draw(spriteBatch, texturePack);
        }
    }
}

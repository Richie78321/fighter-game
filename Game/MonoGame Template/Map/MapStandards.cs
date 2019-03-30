using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterGame.Map
{
    public static class MapStandards
    {
        //File names
        public const string MAP_COLLECTION_FILE_NAME = "mapCollection";

        //Properties
        public const float PLAYER_SCALE = TILE_SIZE;
        public const int MAP_TILE_DIMENSIONS = 40;
        public const int MAP_SIZE = 200;

        //Physics Properties
        public const int REFERENCE_MAP_RESOLUTION = 100;
        public const float GRAVITY_ACCELERATION = .07F;

        //Derived constants
        public const float TILE_SIZE = MAP_SIZE / MAP_TILE_DIMENSIONS;
    }
}

using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using FighterGame.Entities;
using PolygonalPhysics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FighterGame.Entities.Player;
using FighterGame.Runtime;
using FighterGame.Network.Message;

namespace FighterGame.Map.Background
{
    public class BackgroundImage
    {
        public readonly Texture2D BackgroundTexture;
        public readonly int Depth;

        public BackgroundImage(Texture2D BackgroundTexture, int Depth)
        {
            this.BackgroundTexture = BackgroundTexture;
            this.Depth = Depth;
        }
    }
}

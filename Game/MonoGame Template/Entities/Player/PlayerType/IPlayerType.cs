using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FighterGame.Entities.Player.PlayerType
{
    public interface IPlayerType
    {
        PlayerStandards.PlayerType GetPlayerType();
        float GetPlayerScale();
        PlayerTexturePack LoadTexturePack(ContentManager Content, GraphicsDevice graphicsDevice);
        LocalPlayer DeployPlayer(ContentManager Content, GraphicsDevice graphicsDevice, GameMap gameMap, GameTime gameTime, bool mobile = true);
    }
}

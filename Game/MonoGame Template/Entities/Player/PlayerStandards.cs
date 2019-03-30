using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Entities.Player.PlayerType;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FighterGame.Map;

namespace FighterGame.Entities.Player
{
    public static class PlayerStandards
    {
        public static void LoadPlayerTypes()
        {
            List<IPlayerType> playerTypes = new List<IPlayerType>
            {
                new KnightType()
            };

            //Set
            PlayerTypes = playerTypes.ToArray();
        }

        public static IPlayerType[] PlayerTypes;

        public enum PlayerType
        {
            Knight
        }
    }
}

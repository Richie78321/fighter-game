using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Textures;
using Microsoft.Xna.Framework.Graphics;

namespace FighterGame.Entities.Player
{
    public class PlayerTexturePack : EntityTexturePack
    {
        public readonly SpriteSheet[] BloodSheets;
        public readonly SpriteSheet[] DeathParticles;

        public readonly Texture2D IndicatorTexture;

        public PlayerTexturePack(SpriteSheet[] loopSheets, SpriteEvent[][] eventSheets, SpriteSheet[] bloodSheets, SpriteSheet[] deathParticles, Texture2D indicatorTexture) : base(loopSheets, eventSheets)
        {
            this.BloodSheets = bloodSheets;
            this.DeathParticles = deathParticles;
            this.IndicatorTexture = indicatorTexture;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FighterGame.Map;
using FighterGame.Textures;

namespace FighterGame.Entities
{
    public class SpriteManager
    {
        public enum FacingStates
        {
            Right,
            Left
        }

        public enum SpritePosition
        {
            Highest,
            Lowest,
            Leftmost,
            Rightmost
        }

        public static int SpritesheetMaximum(SpriteSheet spriteSheet, SpritePosition spritePosition)
        {
            switch (spritePosition)
            {
                case SpritePosition.Highest:
                    int highestPixel = int.MaxValue;
                    for (int i = 0; i < spriteSheet.textures.Length; i++)
                    {
                        int textureHighestPixel = SpriteHighestPixel(spriteSheet.textures[i]);
                        if (textureHighestPixel < highestPixel) highestPixel = textureHighestPixel;
                    }
                    return highestPixel;
                case SpritePosition.Lowest:
                    int lowestPixel = 0;
                    for (int i = 0; i < spriteSheet.textures.Length; i++)
                    {
                        int textureLowestPixel = SpriteLowestPixel(spriteSheet.textures[i]);
                        if (textureLowestPixel > lowestPixel) lowestPixel = textureLowestPixel;
                    }
                    return lowestPixel;
                case SpritePosition.Leftmost:
                    int leftmostPixel = int.MaxValue;
                    for (int i = 0; i < spriteSheet.textures.Length; i++)
                    {
                        int textureLeftmostPixel = SpriteLeftmostPixel(spriteSheet.textures[i]);
                        if (textureLeftmostPixel < leftmostPixel) leftmostPixel = textureLeftmostPixel;
                    }
                    return leftmostPixel;
                case SpritePosition.Rightmost:
                    int rightmostPixel = 0;
                    for (int i = 0; i < spriteSheet.textures.Length; i++)
                    {
                        int textureRightmostPixel = SpriteRightmostPixel(spriteSheet.textures[i]);
                        if (textureRightmostPixel > rightmostPixel) rightmostPixel = textureRightmostPixel;
                    }
                    return rightmostPixel;
                default: return 0;
            }
        }

        public static int SpriteHighestPixel(Texture2D sprite)
        {
            Color[] colorData = new Color[sprite.Height * sprite.Width];
            sprite.GetData(colorData);

            int highestPixel = sprite.Height;
            for (int y = 0; y < sprite.Height; y++)
            {
                bool found = false;
                for (int x = 0; x < sprite.Width; x++)
                {
                    if (colorData[(y * sprite.Width) + x] != Color.Transparent)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    highestPixel = y;
                    break;
                }
            }

            return highestPixel;
        }

        public static int SpriteLowestPixel(Texture2D sprite)
        {
            Color[] colorData = new Color[sprite.Height * sprite.Width];
            sprite.GetData(colorData);

            int lowestPixel = 0;
            for (int y = sprite.Height - 1; y >= 0; y--)
            {
                bool found = false;
                for (int x = 0; x < sprite.Width; x++)
                {
                    if (colorData[(y * sprite.Width) + x] != Color.Transparent)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    lowestPixel = y;
                    break;
                }
            }

            return lowestPixel;
        }

        public static int SpriteLeftmostPixel(Texture2D sprite)
        {
            Color[] colorData = new Color[sprite.Height * sprite.Width];
            sprite.GetData(colorData);

            int leftmostPixel = sprite.Width;
            for (int x = 0; x < sprite.Width; x++)
            {
                bool found = false;
                for (int y = 0; y < sprite.Height; y++)
                {
                    if (colorData[(y * sprite.Width) + x] != Color.Transparent)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    leftmostPixel = x;
                    break;
                }
            }

            return leftmostPixel;
        }

        public static int SpriteRightmostPixel(Texture2D sprite)
        {
            Color[] colorData = new Color[sprite.Height * sprite.Width];
            sprite.GetData(colorData);

            int rightmostPixel = 0;
            for (int x = sprite.Width - 1; x >= 0; x--)
            {
                bool found = false;
                for (int y = 0; y < sprite.Height; y++)
                {
                    if (colorData[(y * sprite.Width) + x] != Color.Transparent)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    rightmostPixel = x;
                    break;
                }
            }

            return rightmostPixel;
        }

        //Object
        public readonly EntityTexturePack TexturePack;

        private Vector2 spriteCenter;
        public Vector2 SpriteCenter => spriteCenter;

        public SpriteManager(EntityTexturePack texturePack, GameTime gameTime)
        {
            this.TexturePack = texturePack;
            frameChangeTime = gameTime.TotalGameTime.TotalMilliseconds;
            spriteCenter = new Vector2((float)(SpriteLeftmostPixel(texturePack.LoopSheets[0].textures[0]) + SpriteRightmostPixel(texturePack.LoopSheets[0].textures[0])) / 2, (float)(SpriteHighestPixel(texturePack.LoopSheets[0].textures[0]) + SpriteLowestPixel(texturePack.LoopSheets[0].textures[0])) / 2);
        }

        private int spriteState = 0;
        public int SpriteState => spriteState;
        public void SetSpriteState(int value, GameTime gameTime)
        {
            if (value != spriteState && spriteEvent == null)
            {
                _StateChanged = true;
                spriteState = value;
                sheetPos = 0;
                frameChangeTime = gameTime.TotalGameTime.TotalMilliseconds;
            }
        }

        public FacingStates FacingState = FacingStates.Right;

        private SpriteEvent spriteEvent = null;
        public SpriteEvent SpriteEvent => spriteEvent;
        private event EventHandler OnEventEnd;
        private int spriteEventIndex = -1;
        public int SpriteEventIndex => spriteEventIndex;
        private int sheetSpecification = -1;
        public int SheetSpecification => sheetSpecification;
        public void SetSpriteEvent(int value, GameTime gameTime, EventHandler OnEventEnd = null, int sheetSpecification = 0)
        {
            if (spriteEvent == null)
            {
                _StateChanged = true;
                if (OnEventEnd == null) this.OnEventEnd = new EventHandler((a, e) => { });
                else this.OnEventEnd = OnEventEnd;
                spriteEvent = new SpriteEvent(TexturePack.EventSheets[value][sheetSpecification].spriteSheet, gameTime, TexturePack.EventSheets[value][sheetSpecification].stickFrame);
                spriteEventIndex = value;
                this.sheetSpecification = sheetSpecification;
            }
        }
        public void RemoveSpriteEvent(GameTime gameTime)
        {
            if (spriteEvent != null)
            {
                _StateChanged = true;
                spriteEvent = null;
                frameChangeTime = gameTime.TotalGameTime.TotalMilliseconds;
                sheetPos = 0;
                spriteEventIndex = -1;
                sheetSpecification = -1;

                OnEventEnd.Invoke(this, null);
                OnEventEnd = null;
            }
        }

        private int sheetPos = 0;
        public int SheetPosition => sheetPos;
        private double frameChangeTime;
        public Texture2D GetPlayerTexture(out SpriteEffects spriteEffect)
        {
            //Set effect
            if (FacingState == FacingStates.Left) spriteEffect = SpriteEffects.FlipHorizontally;
            else spriteEffect = SpriteEffects.None;

            if (spriteEvent == null)
            {
                return TexturePack.LoopSheets[spriteState].textures[sheetPos];
            }
            else
            {
                return spriteEvent.spriteSheet.textures[spriteEvent.spriteSheetPos];
            }
        }

        public void Update(GameTime gameTime)
        {
            if (spriteEvent == null)
            {
                //Increment sheet position
                if (gameTime.TotalGameTime.TotalMilliseconds - frameChangeTime >= TexturePack.LoopSheets[spriteState].FrameTime)
                {
                    _StateChanged = true;
                    sheetPos = (sheetPos + 1) % TexturePack.LoopSheets[spriteState].textures.Length;
                    frameChangeTime = gameTime.TotalGameTime.TotalMilliseconds - ((gameTime.TotalGameTime.TotalMilliseconds - frameChangeTime) - TexturePack.LoopSheets[spriteState].FrameTime);
                }
            }
            else
            {
                //Increment spriteEvent
                if (gameTime.TotalGameTime.TotalMilliseconds - spriteEvent.frameChangeTime >= spriteEvent.FrameTime)
                {
                    _StateChanged = true;
                    spriteEvent.spriteSheetPos++;
                    spriteEvent.frameChangeTime = gameTime.TotalGameTime.TotalMilliseconds - (spriteEvent.FrameTime - (gameTime.TotalGameTime.TotalMilliseconds - spriteEvent.frameChangeTime));
                    if (spriteEvent.spriteSheetPos >= spriteEvent.spriteSheet.textures.Length)
                    {
                        //Check for stick frame
                        if (spriteEvent.stickFrame)
                        {
                            spriteEvent.spriteSheetPos = spriteEvent.spriteSheet.textures.Length - 1;
                        }
                        else
                        {
                            RemoveSpriteEvent(gameTime);
                        }
                    }
                }
            }
        }

        public SpriteManagerData SpriteManagerData
        {
            get
            {
                _StateChanged = false;
                return new SpriteManagerData(spriteState, spriteEventIndex, sheetSpecification, FacingState);
            }
        }

        private bool _StateChanged = true;
        public bool StateChanged => _StateChanged;

        public void SetSpriteManagerData(SpriteManagerData spriteManagerData, GameTime gameTime)
        {
            _StateChanged = true;
            FacingState = spriteManagerData.facingStates;
            if (spriteManagerData.spriteState != spriteState)
            {
                SetSpriteState(spriteManagerData.spriteState, gameTime);
            }
            if (spriteManagerData.spriteEvent != spriteEventIndex || spriteManagerData.spriteSpecification != sheetSpecification)
            {
                RemoveSpriteEvent(gameTime);
                if (spriteManagerData.spriteEvent != -1)
                {
                    SetSpriteEvent(spriteManagerData.spriteEvent, gameTime, sheetSpecification: spriteManagerData.spriteSpecification);
                }
            }
        }
    }
}

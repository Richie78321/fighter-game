using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameGUI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using FighterGame.Runtime;

namespace FighterGame.GUI
{
    class GameButton : GUIButton
    {
        public static GameButtonData BackButtonData(GameSession gameSession)
        {
            return new GameButtonData("Back", new ButtonAction((e) => { gameSession.graphicsUI.GoPreviousMenu(); }), null);
        }

        public static void LoadContent(ContentManager Content)
        {
            ModularTextures = new Texture2D[] { Content.Load<Texture2D>("GUI/Button/buttonLeft"), Content.Load<Texture2D>("GUI/Button/buttonMiddle"), Content.Load<Texture2D>("GUI/Button/buttonRight"), Content.Load<Texture2D>("GUI/Button/buttonLeftHovered"), Content.Load<Texture2D>("GUI/Button/buttonMiddleHovered"), Content.Load<Texture2D>("GUI/Button/buttonRightHovered") };
            TextFont = Content.Load<SpriteFont>("GUI/Button/buttonFont");
            TextColor = Color.LightGray;
            HoverTextColor = Color.White;
            ClickSound = Content.Load<SoundEffect>("GUI/Button/buttonClickSound");
            HoverSound = Content.Load<SoundEffect>("GUI/Button/buttonHoverSound");
        }

        private static Texture2D[] ModularTextures;
        public static SpriteFont TextFont;
        private static Color TextColor, HoverTextColor;
        private static SoundEffect ClickSound;
        private static SoundEffect HoverSound;

        //Object
        public GameButton(Point centerPosition, Rectangle elementRestriction, string text, ButtonAction ClickAction, ButtonAction HoverAction, ButtonAction UnHoverAction = null) : base(centerPosition, elementRestriction, text, TextFont, TextColor, ModularTextures, ClickAction, HoverAction: HoverAction, UnHoverAction: UnHoverAction)
        {
            this.ClickAction += GameButton_ClickAction;
            this.HoverAction += GameButton_HoverAction;
            this.UnHoverAction += GameButton_UnHoverAction;
        }

        public GameButton(Point centerPosition, Rectangle elementRestriction, GameButtonData gameButtonData) : this(centerPosition, elementRestriction, gameButtonData.text, gameButtonData.ClickAction, gameButtonData.HoverAction)
        {
        }

        private void GameButton_ClickAction(GUIButton button)
        {
            //Play click sound
            ClickSound.Play();
        }

        private void GameButton_HoverAction(GUIButton button)
        {
            TextLabel.TextColor = HoverTextColor;

            //Play hover sound
            HoverSound.Play();
        }

        private void GameButton_UnHoverAction(GUIButton button)
        {
            TextLabel.TextColor = TextColor;
        }
    }

    public class GameButtonData : IMenuElementData
    {
        public readonly GUIButton.ButtonAction ClickAction;
        public readonly GUIButton.ButtonAction HoverAction;
        public readonly string text;

        public GameButtonData(string text, GUIButton.ButtonAction ClickAction, GUIButton.ButtonAction HoverAction)
        {
            this.ClickAction = ClickAction;
            this.HoverAction = HoverAction;
            this.text = text;
        }

        public GUIElement DeployElement(Point centerPoint, Rectangle elementRestrictions)
        {
            return new GameButton(centerPoint, elementRestrictions, this);
        }
    }

    public interface IMenuElementData
    {
        GUIElement DeployElement(Point centerPoint, Rectangle elementRestrictions);
    }
}

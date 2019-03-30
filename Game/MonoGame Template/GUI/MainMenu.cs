using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Runtime;
using GameGUI;
using Microsoft.Xna.Framework;
using FighterGame.Map.Background;
using FighterGame.Map;
using FighterGame.Database;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace FighterGame.GUI
{
    public class MainMenu : Menu
    {
        private const float MAIN_MENU_SCREEN_PORTION = 0F;

        public static Song MainMenuSong = null;

        public static void LoadContent(ContentManager Content)
        {
            MainMenuSong = Content.Load<Song>("GUI/Fighter_Game_Main_Menu");
        }

        //Object
        private GameSession gameSession;

        private TimeSpan songPosition;

        public override bool Visible
        {
            get => base.Visible;
            set
            {
                if (MainMenuSong != null)
                {
                    if (value)
                    {
                        MediaPlayer.IsRepeating = true;
                        MediaPlayer.Play(MainMenuSong, songPosition);
                    }
                    else
                    {
                        songPosition = MediaPlayer.PlayPosition;
                        MediaPlayer.Stop();
                    }
                }

                base.Visible = value;
            }
        }

        public MainMenu(GraphicsDevice graphicsDevice, GameSession gameSession) : base(new Microsoft.Xna.Framework.Rectangle(graphicsDevice.Viewport.Width / 4, 0, graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height))
        {
            this.gameSession = gameSession;
            SetMenu(DeployGameButtonData(), MAIN_MENU_SCREEN_PORTION);
        }

        private IMenuElementData[] DeployGameButtonData()
        {
            List<IMenuElementData> gameButtonData = new List<IMenuElementData>()
            {
                new GameButtonData("Start Game", StartGame_ClickAction, null),
                new GameButtonData("Practice Mode", PracticeMode_ClickAction, null),
                new GameButtonData("Inventory", Inventory_ClickAction, null),
                new GameButtonData("Settings", Settings_ClickAction, null),
                new GameButtonData("Exit", Exit_ClickAction, null)
            };

            return gameButtonData.ToArray();
        }
        
        private void Inventory_ClickAction(GUIButton button)
        {
            gameSession.graphicsUI.SetCurrentMenu(gameSession.graphicsUI.InventoryMenu);
        }

        private async void PracticeMode_ClickAction(GUIButton button)
        {
            gameSession.graphicsUI.SetCurrentMenu(null);
            if (!await gameSession.EstablishGame(false))
            {
                gameSession.graphicsUI.SetCurrentMenu(gameSession.graphicsUI.MainMenu);
            }

            ActiveGameActions();
        }

        private void ActiveGameActions()
        {
            gameSession.graphicsUI.EnableActiveGamePanel(true);
        }

        private async void StartGame_ClickAction(GUIButton button)
        {
            gameSession.graphicsUI.SetCurrentMenu(null);
            if (!await gameSession.EstablishGame(true))
            {
                //Unsuccessful
                gameSession.graphicsUI.SetCurrentMenu(gameSession.graphicsUI.MainMenu);
            }
            else
            {
                //Set status message
                if (!gameSession.GameRunning) gameSession.graphicsUI.MessageManager.AddMessage(new MessageBox("Waiting for the game to start...", gameSession.LatestGameTime, gameSession.graphicsUI.MessageManager.RemoveMessage, dismissButtonText: "Cancel", OnDismiss: new GUIButton.ButtonAction((e) => { gameSession.EndCurrentGame(false); })));
            }

            ActiveGameActions();
        }

        private void Settings_ClickAction(GUIButton button)
        {
            gameSession.graphicsUI.SetCurrentMenu(gameSession.graphicsUI.SettingsMenu);
        }

        private void Exit_ClickAction(GUIButton button)
        {
            gameSession.ExitGame();
        }
    }
}

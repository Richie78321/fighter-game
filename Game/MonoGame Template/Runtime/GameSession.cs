using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Map;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FighterGame.Entities.Player.PlayerType;
using FighterGame.GUI;
using FighterGame.Entities.Player;
using Microsoft.Xna.Framework.Content;
using FighterGame.Network.Message;
using FighterGame.Database;

namespace FighterGame.Runtime
{
    public class GameSession
    {
        public const int MAX_TARGET_PLAYERS = 10;
        public const int MIN_TARGET_PLAYERS = 2;

        //Object
        public int targetPlayers = MIN_TARGET_PLAYERS;

        public readonly Random LocalRandom = new Random();

        public GameMap gameMap = null;
        public GraphicsUI graphicsUI;
        public readonly NetworkManager NetworkManager;
        public readonly DatabaseManager DatabaseManager;

        public readonly GraphicsDevice graphicsDevice;
        public readonly ContentManager Content;

        public readonly Game1 GameInstance;

        public GameSession(GraphicsDevice graphicsDevice, ContentManager Content, Game1 gameInstance)
        {
            this.GameInstance = gameInstance;
            this.Content = Content;
            this.graphicsDevice = graphicsDevice;
            NetworkManager = new NetworkManager(this);
            graphicsUI = new GraphicsUI(graphicsDevice, this);
            DatabaseManager = new DatabaseManager();
        }

        public bool GameRunning => _GameRunning;
        private bool _GameRunning = false;
        public void StartGame()
        {
            _GameRunning = true;
            graphicsUI.MessageManager.ClearMessages();
            graphicsUI.BackgroundPanel.Visible = false;
            graphicsUI.BackgroundPanel.Enabled = false;
        }
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            //Draw map
            if (GameRunning && gameMap != null)
            {
                gameMap.Draw(spriteBatch, graphicsDevice);
            }

            //Draw GUI
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            graphicsUI.Draw(spriteBatch, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), new GameGUI.DrawSpecifics(1F));
            spriteBatch.End();
        }

        private GameTime latestGameTime;
        public GameTime LatestGameTime => latestGameTime;

        private KeyboardState pastKeyboardState = Keyboard.GetState();
        private MouseState pastMouseState = Mouse.GetState();
        public void Update(GraphicsDevice graphicsDevice, GameTime gameTime, KeyboardState keyboardState, MouseState mouseState)
        {
            Player.InFocus = GameInstance.IsActive;

            latestGameTime = gameTime;

            //Update network
            NetworkManager.Update(this);

            //Update map
            if (GameRunning && gameMap != null)
            {
                gameMap.Update(graphicsDevice, gameTime, keyboardState, mouseState);
            }

            //Update GUI
            graphicsUI.Update(gameTime);
            if (GameInstance.IsActive) graphicsUI.Interact(new GameGUI.GUIInput(keyboardState, pastKeyboardState, mouseState, pastMouseState));

            pastKeyboardState = keyboardState;
            pastMouseState = mouseState;
        }

#pragma warning disable CS0162 // Unreachable code detected
        public async Task<bool> EstablishGame(bool connectOnline)
        {
            return await Task.Run(new Func<bool>(() =>
            {
                if (connectOnline)
                {
                    //Connect to matchmaker
                    if (!NetworkManager.Connect(new Network.Client.ClientInfo(new Network.Client.LobbyInfo(targetPlayers, new Network.Client.LobbyAspect[0]), DatabaseManager.Username)))
                    {
                        graphicsUI.MessageManager.AddMessage("Failed to connect to the " + NetworkManager.CurrentConnectionType.ToString().ToLower() + " matchmaking server.");
                        return false;
                    }
                    else return true;
                }
                else
                {
                    //Start local game
                    gameMap = new GameMap(new Random(), graphicsDevice, this);
                    gameMap.AddEntity(PlayerStandards.PlayerTypes[(int)PlayerStandards.PlayerType.Knight].DeployPlayer(Content, graphicsDevice, gameMap, new GameTime()));

                    StartGame();
                    return true;
                }
            }));
        }
#pragma warning restore CS0162 // Unreachable code detected

        public async void LoadMap(Random random)
        {
            await Task.Run(() =>
            {
                //Load map
                gameMap = new Map.GameMap(random, graphicsDevice, this);

                //TEMP (ADD PLAYER SELECTION)
                gameMap.AddEntity(PlayerStandards.PlayerTypes[(int)PlayerStandards.PlayerType.Knight].DeployPlayer(Content, graphicsDevice, gameMap, LatestGameTime));

                //Send ready message
                NetworkManager.SendMessage(new ClientReadyMessage());
            });
        }

        public void ExitGame()
        {
            //ADD SAFE DISCONNECT
            GameInstance.Exit();
        }

        public void EndCurrentGame(bool networkDisconnect)
        {
            if (!networkDisconnect) NetworkManager.Disable();
            else
            {
                //graphicsUI.MessageManager.AddMessage("Disconnected from the lobby.");
                gameMap = null;
                graphicsUI.EnableActiveGamePanel(false);
                graphicsUI.SetCurrentMenu(graphicsUI.MainMenu);
                graphicsUI.BackgroundPanel.Visible = true;
                graphicsUI.BackgroundPanel.Enabled = true;
                _GameRunning = false;
            }
        }
    }
}
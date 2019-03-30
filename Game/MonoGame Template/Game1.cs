using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FighterGame.Textures;
using FighterGame.Entities.Player;
using FighterGame.Entities.Player.PlayerType;
using PolygonalPhysics;
using FighterGame.Map;
using FighterGame.Runtime;
using FighterGame.GUI;

namespace FighterGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = false;

            IsMouseVisible = true;
        }

        public GameSession GameSession;
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Network entity data
            NetworkManager.LoadNetworkContent();

            //Supply network player
            NetworkPlayer.Content = Content;
            NetworkPlayer.graphicsDevice = GraphicsDevice;

            //Load player standards
            PlayerStandards.LoadPlayerTypes();

            GraphicsUI.LoadContent(Content, GraphicsDevice);
            GameMap.LoadMapContent(Content);

            //Load GameSession
            GameSession = new GameSession(GraphicsDevice, Content, this);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private bool escaping = false;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (!escaping)
                {
                    if (GameSession.GameRunning) GameSession.EndCurrentGame(false);
                    else Exit();
                    escaping = true;
                }
            }
            else escaping = false;

            //Update GameSession
            GameSession.Update(GraphicsDevice, gameTime, Keyboard.GetState(), Mouse.GetState());

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Draw GameSession
            GameSession.Draw(spriteBatch, GraphicsDevice);
            spriteBatch.Begin();
            if (GameSession.DatabaseManager.LoggedIn) spriteBatch.DrawString(Content.Load<SpriteFont>("GUI/Button/buttonFont"), GameSession.DatabaseManager.UserIndex.ToString(), Vector2.Zero, Color.White, 0, Vector2.Zero, .05F, SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

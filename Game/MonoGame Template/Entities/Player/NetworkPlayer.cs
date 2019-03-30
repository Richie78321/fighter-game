using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using FighterGame.Runtime;
using FighterGame.Network.Server;
using FighterGame.GUI.Health;
using FighterGame.Network.Client;
using PolygonalPhysics;

namespace FighterGame.Entities.Player
{
    public class NetworkPlayer : NetworkEntity
    {
        public static ContentManager Content;
        public static GraphicsDevice graphicsDevice;

        //Object
        private PlayerStandards.PlayerType playerType;
        private SpriteManager spriteManager = null;
        public SpriteManager SpriteManager => spriteManager;
        private Vector2 drawDimensions;
        public Vector2 DrawDimensions => drawDimensions;

        public bool IsDead = false;

        private PeerClientInfo clientInfo;

        private Polygon polygonalRepresentation;

        public PlayerParticleManager PlayerParticleManager => playerParticleManager;
        private PlayerParticleManager playerParticleManager;

        public void LoadPlayer(PlayerStandards.PlayerType playerType, GameSession gameSession, Lobby lobby, int clientIndex, int entityIndex)
        {
            //Ensure unloaded
            if (spriteManager == null)
            {
                this.playerType = playerType;
                PlayerTexturePack texturePack = PlayerStandards.PlayerTypes[(int)playerType].LoadTexturePack(Content, graphicsDevice);
                spriteManager = new SpriteManager(texturePack, gameSession.LatestGameTime);
                drawDimensions = new Vector2(PlayerStandards.PlayerTypes[(int)playerType].GetPlayerScale() * MapStandards.PLAYER_SCALE, PlayerStandards.PlayerTypes[(int)playerType].GetPlayerScale() * MapStandards.PLAYER_SCALE);
                polygonalRepresentation = new RotationRectangle(new RectangleF(PlayerPosition.X - (DrawDimensions.X / 2), PlayerPosition.Y - (DrawDimensions.Y / 2), DrawDimensions.X, DrawDimensions.Y));

                playerParticleManager = new PlayerParticleManager(texturePack, polygonalRepresentation, DrawDimensions);
                clientInfo = gameSession.NetworkManager.GetClientInfo(clientIndex);
                if (clientInfo != null) healthBar = gameSession.graphicsUI.HealthBarManager.AddHealthBar(clientInfo.Username, Player.PLAYER_HEALTH, gameSession.LatestGameTime);
                else throw new Exception("Error with client info!");
            }
        }

        private HealthBar healthBar;
        public HealthBar HealthBar => healthBar;

        public Vector2 PlayerPosition
        {
            get
            {
                return playerPosition;
            }
            set
            {
                playerPosition = value;
                polygonalRepresentation.Translate(playerPosition - polygonalRepresentation.CenterPoint);
            }
        }
        private Vector2 playerPosition = Vector2.Zero;

        public override void Draw(SpriteBatch spriteBatch, GameMap map)
        {
            //Ensure loaded
            if (spriteManager != null)
            {
                SpriteEffects spriteEffect;
                Texture2D playerTexture = spriteManager.GetPlayerTexture(out spriteEffect);
                spriteBatch.Draw(playerTexture, PlayerPosition, null, Color.White, 0, spriteManager.SpriteCenter, new Vector2(drawDimensions.X / playerTexture.Width, drawDimensions.Y / playerTexture.Height), spriteEffect, 0);

                Vector2 measureString = FighterGame.GUI.GameButton.TextFont.MeasureString(clientInfo.Username);
                spriteBatch.DrawString(FighterGame.GUI.GameButton.TextFont, clientInfo.Username, new Vector2(PlayerPosition.X, PlayerPosition.Y - (DrawDimensions.Y / 2)), Color.White, 0, new Vector2(measureString.X / 2, measureString.Y / 2), Math.Min(DrawDimensions.X / measureString.X, DrawDimensions.Y / (3 * measureString.Y)), SpriteEffects.None, 0);
            }
        }

        protected override void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map)
        {
            spriteManager.Update(gameTime);
        }
    }
}

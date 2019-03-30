using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FighterGame.Textures;
using Microsoft.Xna.Framework.Content;
using PolygonalPhysics;
using Microsoft.Xna.Framework.Input;
using FighterGame.Map;
using FighterGame.Runtime;

namespace FighterGame.Entities.Player
{
    public class LocalPlayer : Player
    {
        //Object
        public readonly PlayerStandards.PlayerType PlayerType;

        public RectangleF DrawRectangle
        {
            get
            {
                Vector2 centerPoint = RigidBody.CollisionPolygon.CenterPoint;
                return new RectangleF(centerPoint.X - (DrawDimensions.X / 2), centerPoint.Y - (DrawDimensions.Y / 2), DrawDimensions.X, DrawDimensions.Y);
            }
        }

        public LocalPlayer(PlayerTexturePack texturePack, RigidBody rigidBody, GameTime gameTime, Vector2 drawDimensions, PlayerStandards.PlayerType PlayerType, GameMap gameMap) : base(texturePack, gameTime, rigidBody, drawDimensions, gameMap)
        {
            this.PlayerType = PlayerType;
            this.rigidBody = rigidBody;
        }

        protected override void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map)
        {
            base.Update(gameTime, keyboardState, pastKeyboardState, mouseState, pastMouseState, map);

            //Update rigidBody
            rigidBody.Update(gameTime);
            ClientCollider.CollisionPolygon.Translate(rigidBody.CollisionPolygon.CenterPoint - ClientCollider.CollisionPolygon.CenterPoint);
        }

        public override void SendNetworkEntityData(NetworkManager networkManager)
        {
            if (SpriteManager.StateChanged) networkManager.SendNetworkEntityData(new NetworkPlayerData(this, networkManager.ClientIndex));
            else networkManager.SendNetworkEntityData(new SimpleNetworkPlayerData(this, networkManager.ClientIndex));
        }

        protected override void OnDeath(GameSession gameSession)
        {  
        }
    }
}

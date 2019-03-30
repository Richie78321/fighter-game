using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonalPhysics;
using FighterGame.Textures;
using FighterGame.Map;
using static FighterGame.Entities.SpriteManager;

namespace FighterGame.Entities.Player.PlayerType
{
    public abstract class StandardWalker : LocalPlayer
    {
        public static Keys MOVE_LEFT = Keys.A;
        public static Keys MOVE_RIGHT = Keys.D;
        public static Keys JUMP = Keys.Space;
        public static Keys DODGE = Keys.S;

        public enum MoveState
        {
            Idle,
            Run,
        }
        public enum EventState
        {
            Jump,
            Fall,
            DodgeRoll
        }

        public static RigidBody CreateRigidBody(Texture2D boundaryTexture, float mass, CollisionReferenceMap<RigidBody> referenceMap, Vector2 drawDimensions, GameMap gameMap, bool noCollide = false)
        {
            int spriteHighest = SpriteHighestPixel(boundaryTexture);
            int spriteLowest = SpriteLowestPixel(boundaryTexture);
            int spriteLeftmost = SpriteLeftmostPixel(boundaryTexture);
            int spriteRightmost = SpriteRightmostPixel(boundaryTexture);

            Vector2 colliderSize = new Vector2((spriteRightmost - spriteLeftmost + 2) * (drawDimensions.X / boundaryTexture.Height), (spriteLowest - spriteHighest + 2) * (drawDimensions.Y / boundaryTexture.Height));
            Platform spawnPlatform = gameMap.MapArchitecture.MapPlatforms[gameMap.gameSession.LocalRandom.Next(0, gameMap.MapArchitecture.MapPlatforms.Length)];
            int spawnTile = gameMap.gameSession.LocalRandom.Next(0, spawnPlatform.TileLength);
            Vector2 initialPosition = new Vector2(spawnPlatform.Position.X + (MapStandards.TILE_SIZE * (spawnTile + .5F)) - (colliderSize.X / 2), spawnPlatform.Position.Y - colliderSize.Y);

            //Create polygon
            RotationRectangle rotRec = new RotationRectangle(new RectangleF(initialPosition.X, initialPosition.Y, colliderSize.X, colliderSize.Y));

            RigidBody.VelocityRestriction[] velocityRestrictions = new RigidBody.VelocityRestriction[Enum.GetNames(typeof(RigidBody.VelocityRestriction)).Length];
            for (int i = 0; i < velocityRestrictions.Length; i++) velocityRestrictions[i] = (RigidBody.VelocityRestriction)i;
            return new RigidBody(rotRec, mass, referenceMap, velocityRestrictions, staticBody: true, rotationOnForce: false);
        }

        //Object
        public StandardWalker(PlayerTexturePack playerTexturePack, float strafeAcceleration, float maxStrafeVelocity, float jumpVelocity, float dodgeVelocity, float mass, CollisionReferenceMap<RigidBody> referenceMap, GameTime gameTime, Vector2 drawDimensions, PlayerStandards.PlayerType PlayerType, GameMap gameMap, bool mobile = true) : base(playerTexturePack, CreateRigidBody(playerTexturePack.LoopSheets[0].textures[0], mass, referenceMap, drawDimensions, gameMap), gameTime, drawDimensions, PlayerType, gameMap)
        {
            this.Mobile = mobile;
            this.strafeAcceleration = strafeAcceleration;
            this.maxStrafeVelocity = maxStrafeVelocity;
            this.jumpVelocity = jumpVelocity;
            this.dodgeVelocity = dodgeVelocity;
            movementFriction = this.strafeAcceleration;
            dodgeTime = playerTexturePack.EventSheets[(int)EventState.DodgeRoll][0].FrameTime * playerTexturePack.EventSheets[(int)EventState.DodgeRoll][0].spriteSheet.textures.Length;
            lastDodge = -dodgeTime;
        }

        protected override void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map)
        {
            base.Update(gameTime, keyboardState, pastKeyboardState, mouseState, pastMouseState, map);

            //Control player
            rigidBody.AddTranslationalVelocity(new Vector2(0, MapStandards.GRAVITY_ACCELERATION), isGlobal: false);
            ControlPlayer(gameTime, keyboardState, pastKeyboardState, mouseState, pastMouseState, map);
        }

        public bool Mobile = true;
        public bool RestrictedMobility = false;

        protected float jumpVelocity;
        protected float strafeAcceleration;
        protected float maxStrafeVelocity;
        protected float dodgeVelocity;
        protected int dodgeTime;
        protected double lastDodge;
        protected float movementFriction;
        private void ControlPlayer(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map)
        {
            //Update sprite state
            if (SpriteManager.SpriteEventIndex == (int)EventState.Jump)
            {
                if (rigidBody.RelativeTranslationalVelocity.Y > 0)
                {
                    SpriteManager.RemoveSpriteEvent(gameTime);
                    SpriteManager.SetSpriteEvent((int)EventState.Fall, gameTime);
                }
            }
            else if (SpriteManager.SpriteEventIndex == (int)EventState.Fall)
            {
                if (rigidBody.VelocityRestrictions[(int)RigidBody.VelocityRestriction.Down] != null) SpriteManager.RemoveSpriteEvent(gameTime);
            }

            if (Mobile)
            {
                //Strafe
                bool isRunning = false;
                if (keyboardState.IsKeyDown(MOVE_LEFT) && keyboardState.IsKeyUp(MOVE_RIGHT))
                {
                    //Left
                    float velocityAddition = MathHelper.Clamp(-(rigidBody.RelativeTranslationalVelocity.X + maxStrafeVelocity), -strafeAcceleration, 0);
                    rigidBody.AddTranslationalVelocity(new Vector2(velocityAddition, 0), isGlobal: false);
                    SpriteManager.FacingState = SpriteManager.FacingStates.Left;
                    isRunning = true;
                }
                else if (keyboardState.IsKeyDown(MOVE_RIGHT) && keyboardState.IsKeyUp(MOVE_LEFT))
                {
                    //Right
                    float velocityAddition = MathHelper.Clamp(-(rigidBody.RelativeTranslationalVelocity.X - maxStrafeVelocity), 0, strafeAcceleration);
                    rigidBody.AddTranslationalVelocity(new Vector2(velocityAddition, 0), isGlobal: false);
                    SpriteManager.FacingState = SpriteManager.FacingStates.Right;
                    isRunning = true;
                }

                if (rigidBody.VelocityRestrictions[(int)RigidBody.VelocityRestriction.Down] != null || rigidBody.VelocityRestrictions[(int)RigidBody.VelocityRestriction.Left] != null || rigidBody.VelocityRestrictions[(int)RigidBody.VelocityRestriction.Right] != null)
                {
                    //On ground
                    //Jump
                    if (keyboardState.IsKeyDown(JUMP) && pastKeyboardState.IsKeyUp(JUMP))
                    {
                        rigidBody.SetTranslationalVelocity(new Vector2(rigidBody.RelativeTranslationalVelocity.X, -jumpVelocity), isGlobal: false);
                        SpriteManager.SetSpriteEvent((int)EventState.Jump, gameTime);
                    }
                    else if (!RestrictedMobility && keyboardState.IsKeyDown(DODGE) && pastKeyboardState.IsKeyUp(DODGE) && gameTime.TotalGameTime.TotalMilliseconds - lastDodge > dodgeTime)
                    {
                        ClientCollider.Enabled = false;
                        if (SpriteManager.FacingState == SpriteManager.FacingStates.Left) rigidBody.AddTranslationalVelocity(new Vector2(-dodgeVelocity, 0), isGlobal: false);
                        else if (SpriteManager.FacingState == SpriteManager.FacingStates.Right) rigidBody.AddTranslationalVelocity(new Vector2(dodgeVelocity, 0), isGlobal: false);
                        SpriteManager.SetSpriteEvent((int)EventState.DodgeRoll, gameTime, new EventHandler((sender, e) =>
                        {
                            ClientCollider.Enabled = true;
                        }));
                        lastDodge = gameTime.TotalGameTime.TotalMilliseconds;
                    }

                    if (isRunning) SpriteManager.SetSpriteState((int)MoveState.Run, gameTime);
                    else SpriteManager.SetSpriteState((int)MoveState.Idle, gameTime);
                }
                else SpriteManager.SetSpriteEvent((int)EventState.Fall, gameTime);
            }
        }
    }
}

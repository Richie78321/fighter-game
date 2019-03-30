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
using FighterGame.Runtime;

namespace FighterGame.Entities.Player.PlayerType
{
    class KnightType : IPlayerType
    {
        private const float STRAFE_ACCELERATION = .08F;
        private const float MAX_STRAFE_VELOCITY = .64F;
        private const float JUMP_VELOCITY = 2F;
        private const float DODGE_VELOCITY = 1.6F;
        private const float PLAYER_SCALE_AMOUNT = 2F;
        private const float MASS = 1F;

        private const float AIR_SIDE_LUNGE_SPEED = DODGE_VELOCITY * (2F / 4);
        private const float AIR_UP_LUNGE_SPEED = DODGE_VELOCITY * .6F;
        private const float AIR_DOWN_LUNGE_SPEED = DODGE_VELOCITY * (1F / 4);
        private const float GROUND_LUNGE_SPEED = DODGE_VELOCITY * (3F / 4);

        private const int BODY_PARTS_COUNT = 8;

        private const PlayerStandards.PlayerType playerType = PlayerStandards.PlayerType.Knight;

        public PlayerTexturePack LoadTexturePack(ContentManager Content, GraphicsDevice graphicsDevice)
        {
            SpriteSheet[] loopSheets = new SpriteSheet[Enum.GetNames(typeof(StandardAttacker.MoveState)).Length];
            loopSheets[(int)StandardAttacker.MoveState.Idle] = new SpriteSheet(Content.Load<Texture2D>("Player/player_idle"), 3, 1, 200, graphicsDevice);
            loopSheets[(int)StandardAttacker.MoveState.Run] = new SpriteSheet(Content.Load<Texture2D>("Player/player_run"), 4, 3, 75, graphicsDevice);

            SpriteEvent[][] eventSheets = new SpriteEvent[Enum.GetNames(typeof(StandardAttacker.EventState)).Length + Enum.GetNames(typeof(StandardAttacker.AttackEvent)).Length][];
            eventSheets[(int)StandardAttacker.EventState.Jump] = new SpriteEvent[] { new SpriteEvent(new SpriteSheet(Content.Load<Texture2D>("Player/player_jump"), 4, 1, 100, graphicsDevice), new GameTime(), stickFrame: true) };
            eventSheets[(int)StandardAttacker.EventState.DodgeRoll] = new SpriteEvent[] { new SpriteEvent(new SpriteSheet(Content.Load<Texture2D>("Player/player_dodgeroll"), 4, 2, 100, graphicsDevice), new GameTime()) };
            eventSheets[(int)StandardAttacker.EventState.Fall] = new SpriteEvent[] { new SpriteEvent(new SpriteSheet(Content.Load<Texture2D>("Player/player_fall"), 4, 1, 100, graphicsDevice), new GameTime(), stickFrame: true) };
            eventSheets[StandardAttacker.AttackEventSpritePosition(StandardAttacker.AttackEvent.AirAttackSide)] = new SpriteEvent[] { new SpriteEvent(new SpriteSheet(Content.Load<Texture2D>("Player/player_attack_air_horizontal"), 4, 2, 75, graphicsDevice), new GameTime()) };
            eventSheets[StandardAttacker.AttackEventSpritePosition(StandardAttacker.AttackEvent.AirAttackDown)] = new SpriteEvent[] { new SpriteEvent(new SpriteSheet(Content.Load<Texture2D>("Player/player_attack_down"), 5, 2, 75, graphicsDevice), new GameTime(), stickFrame: true) };
            eventSheets[StandardAttacker.AttackEventSpritePosition(StandardAttacker.AttackEvent.AirAttackUp)] = new SpriteEvent[] { new SpriteEvent(new SpriteSheet(Content.Load<Texture2D>("Player/player_attack_up"), 4, 2, 75, graphicsDevice), new GameTime()) };
            eventSheets[StandardAttacker.AttackEventSpritePosition(StandardAttacker.AttackEvent.Hit)] = new SpriteEvent[] { new SpriteEvent(new SpriteSheet(Content.Load<Texture2D>("Player/player_damaged"), 4, 2, 75, graphicsDevice), new GameTime()) };

            //Combo moves
            eventSheets[StandardAttacker.AttackEventSpritePosition(StandardAttacker.AttackEvent.SideAttackLight)] = new SpriteEvent[] { new SpriteEvent(new SpriteSheet(Content.Load<Texture2D>("Player/player_attack_side_light_1"), 5, 1, 75, graphicsDevice), new GameTime()), new SpriteEvent(new SpriteSheet(Content.Load<Texture2D>("Player/player_attack_side_light_2"), 5, 1, 75, graphicsDevice), new GameTime()) };
            eventSheets[StandardAttacker.AttackEventSpritePosition(StandardAttacker.AttackEvent.SideAttackHeavy)] = new SpriteEvent[] { new SpriteEvent(new SpriteSheet(Content.Load<Texture2D>("Player/player_attack_side_heavy_1"), 4, 2, 75, graphicsDevice), new GameTime()), new SpriteEvent(new SpriteSheet(Content.Load<Texture2D>("Player/player_attack_side_heavy_2"), 4, 2, 75, graphicsDevice), new GameTime()) };

            SpriteSheet[] bloodSheets = new SpriteSheet[3];
            bloodSheets[0] = new SpriteSheet(Content.Load<Texture2D>("Player/Blood/blood0"), 4, 1, 75, graphicsDevice);
            bloodSheets[1] = new SpriteSheet(Content.Load<Texture2D>("Player/Blood/blood1"), 4, 1, 75, graphicsDevice);
            bloodSheets[2] = new SpriteSheet(Content.Load<Texture2D>("Player/Blood/blood2"), 4, 1, 75, graphicsDevice);

            SpriteSheet[] bodyParts = new SpriteSheet[BODY_PARTS_COUNT];
            for (int i = 0; i < bodyParts.Length; i++) bodyParts[i] = new SpriteSheet(Content.Load<Texture2D>("Player/BodyParts/bodyPart" + i), 1, 1, int.MaxValue, graphicsDevice);

            return new PlayerTexturePack(loopSheets, eventSheets, bloodSheets, bodyParts, Content.Load<Texture2D>("Player/Indicator"));
        }

        //Object
        public PlayerStandards.PlayerType GetPlayerType()
        {
            return playerType;
        }

        public float GetPlayerScale()
        {
            return PLAYER_SCALE_AMOUNT;
        }

        public LocalPlayer DeployPlayer(ContentManager Content, GraphicsDevice graphicsDevice, GameMap gameMap, GameTime gameTime, bool mobile = true)
        {
            AttackSpecification[] attackSpecifications = new AttackSpecification[Enum.GetNames(typeof(StandardAttacker.AttackEvent)).Length];
            attackSpecifications[(int)StandardAttacker.AttackEvent.SideAttackLight] = new StandardGroundAttack(GROUND_LUNGE_SPEED);
            attackSpecifications[(int)StandardAttacker.AttackEvent.SideAttackHeavy] = new StandardGroundAttack(GROUND_LUNGE_SPEED);
            attackSpecifications[(int)StandardAttacker.AttackEvent.AirAttackSide] = new StandardAirSideAttack(AIR_SIDE_LUNGE_SPEED);
            attackSpecifications[(int)StandardAttacker.AttackEvent.AirAttackUp] = new StandardAirUpAttack(AIR_UP_LUNGE_SPEED);
            attackSpecifications[(int)StandardAttacker.AttackEvent.AirAttackDown] = new StandardAirDownAttack(AIR_DOWN_LUNGE_SPEED);

            return new StandardAttacker(LoadTexturePack(Content, graphicsDevice), STRAFE_ACCELERATION, MAX_STRAFE_VELOCITY, JUMP_VELOCITY, DODGE_VELOCITY, attackSpecifications, MASS, gameMap.ReferenceMap, gameTime, new Vector2(PLAYER_SCALE_AMOUNT * Map.MapStandards.PLAYER_SCALE, PLAYER_SCALE_AMOUNT * Map.MapStandards.PLAYER_SCALE), playerType, gameMap, mobile);
        }
    }

    class StandardGroundAttack : AttackSpecification
    {
        private const int COMBO_FRAMES = 2;

        //Object
        private float lungeSpeed;

        public StandardGroundAttack(float lungeSpeed)
        {
            this.lungeSpeed = lungeSpeed;
        }

        public override void Activate(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map, StandardAttacker player)
        {
            bool willActivate = false;
            StandardAttacker.AttackEvent eventToActivate = StandardAttacker.AttackEvent.SideAttackLight;
            if (mouseState.LeftButton == ButtonState.Pressed && pastMouseState.LeftButton == ButtonState.Released)
            {
                willActivate = true;
                eventToActivate = StandardAttacker.AttackEvent.SideAttackLight;
            }
            else if (mouseState.RightButton == ButtonState.Pressed && pastMouseState.RightButton == ButtonState.Released)
            {
                willActivate = true;
                eventToActivate = StandardAttacker.AttackEvent.SideAttackHeavy;
            }

            if (willActivate)
            {
                int comboLevel;
                if (StandardAttacker.AttackEventSpritePosition(eventToActivate) != player.SpriteManager.SpriteEventIndex) comboLevel = 0;
                else comboLevel = player.SpriteManager.SheetSpecification + 1;

                player.SpriteManager.RemoveSpriteEvent(gameTime);
                player.SpriteManager.SetSpriteEvent(StandardAttacker.AttackEventSpritePosition(eventToActivate), gameTime, OnEventEnd: new EventHandler((e, s) => { player.HandleAttackEnd(); }), sheetSpecification: comboLevel);

                player.ClientCollider.SetCollisionPolygon(player.AttackColliders[(int)eventToActivate][comboLevel]);
                player.ClientCollider.ActiveCollider = true;
                player.RestrictedMobility = true;
                if (eventToActivate == StandardAttacker.AttackEvent.SideAttackHeavy)
                {
                    player.Mobile = false;
                    player.ClientCollider.CollisionDamage = Player.DEFAULT_PLAYER_DAMAGE + 3;
                }

                player.ClientCollider.CollisionDamage *= (comboLevel + 1);

                //Lunge
                if (player.SpriteManager.FacingState == SpriteManager.FacingStates.Left) player.RigidBody.AddTranslationalVelocity(new Vector2(-lungeSpeed, 0), isGlobal: false);
                else player.RigidBody.AddTranslationalVelocity(new Vector2(lungeSpeed, 0), isGlobal: false);
            }
            else throw new Exception("Activation called incorrectly!");
        }

        public override bool ShouldActivate(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map, StandardAttacker player)
        {
            if (Player.InFocus && player.RigidBody.VelocityRestrictions[(int)RigidBody.VelocityRestriction.Down] != null)
            {
                StandardAttacker.AttackEvent eventToCheck = StandardAttacker.AttackEvent.SideAttackLight;
                bool shouldCheck = false;
                if (mouseState.LeftButton == ButtonState.Pressed && pastMouseState.LeftButton == ButtonState.Released)
                {
                    shouldCheck = true;
                    eventToCheck = StandardAttacker.AttackEvent.SideAttackLight;
                }
                else if (mouseState.RightButton == ButtonState.Pressed && pastMouseState.RightButton == ButtonState.Released)
                {
                    shouldCheck = true;
                    eventToCheck = StandardAttacker.AttackEvent.SideAttackHeavy;
                }

                if (shouldCheck)
                {
                    //Light attack
                    if (player.SpriteManager.SpriteEventIndex == -1)
                    {
                        //Begin move
                        return true;
                    }
                    else if (player.SpriteManager.SpriteEventIndex == StandardAttacker.AttackEventSpritePosition(eventToCheck) && player.SpriteManager.SheetSpecification < player.SpriteManager.TexturePack.EventSheets[StandardAttacker.AttackEventSpritePosition(eventToCheck)].Length - 1)
                    {
                        //Combo possible
                        if (player.SpriteManager.TexturePack.EventSheets[StandardAttacker.AttackEventSpritePosition(eventToCheck)][player.SpriteManager.SheetSpecification].spriteSheet.textures.Length - player.SpriteManager.SpriteEvent.spriteSheetPos <= COMBO_FRAMES)
                        {
                            //Combo made
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override bool ShouldDisable(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map, StandardAttacker player)
        {
            return false;
        }
    }

    class StandardAirSideAttack : AttackSpecification
    {
        public static Keys SIDE_ATTACK_KEY_LEFT = StandardWalker.MOVE_LEFT;
        public static Keys SIDE_ATTACK_KEY_RIGHT = StandardWalker.MOVE_RIGHT;

        //Object
        private float lungeSpeed;

        public StandardAirSideAttack(float lungeSpeed)
        {
            this.lungeSpeed = lungeSpeed;
        }

        public override void Activate(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map, StandardAttacker player)
        {
            player.SpriteManager.RemoveSpriteEvent(gameTime);
            player.SpriteManager.SetSpriteEvent(StandardAttacker.AttackEventSpritePosition(StandardAttacker.AttackEvent.AirAttackSide), gameTime, OnEventEnd: new EventHandler((e, s) => { player.HandleAttackEnd(); }));

            player.ClientCollider.SetCollisionPolygon(player.AttackColliders[(int)StandardAttacker.AttackEvent.AirAttackSide][0]);
            player.ClientCollider.ActiveCollider = true;
            player.RestrictedMobility = true;

            //Air lunge
            if (player.SpriteManager.FacingState == SpriteManager.FacingStates.Left) player.RigidBody.AddTranslationalVelocity(new Vector2(-lungeSpeed, 0), isGlobal: false);
            else player.RigidBody.AddTranslationalVelocity(new Vector2(lungeSpeed, 0), isGlobal: false);
        }

        public override bool ShouldActivate(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map, StandardAttacker player)
        {
            if (player.RigidBody.VelocityRestrictions[(int)RigidBody.VelocityRestriction.Down] == null && mouseState.LeftButton == ButtonState.Pressed && pastMouseState.LeftButton == ButtonState.Released)
            {
                if (player.SpriteManager.SpriteEventIndex == (int)StandardWalker.EventState.Fall || player.SpriteManager.SpriteEventIndex == (int)StandardWalker.EventState.Jump || player.SpriteManager.SpriteEventIndex == -1)
                {
                    return keyboardState.IsKeyDown(SIDE_ATTACK_KEY_RIGHT) || keyboardState.IsKeyDown(SIDE_ATTACK_KEY_LEFT);
                }
            }

            return false;
        }

        public override bool ShouldDisable(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map, StandardAttacker player)
        {
            return false;
        }
    }

    class StandardAirUpAttack : AttackSpecification
    {
        public static Keys ATTACK_UP_KEY = Keys.W;

        //Object
        private float lungeSpeed;

        public StandardAirUpAttack(float lungeSpeed)
        {
            this.lungeSpeed = lungeSpeed;
        }

        public override void Activate(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map, StandardAttacker player)
        {
            player.SpriteManager.RemoveSpriteEvent(gameTime);
            player.SpriteManager.SetSpriteEvent(StandardAttacker.AttackEventSpritePosition(StandardAttacker.AttackEvent.AirAttackUp), gameTime, OnEventEnd: new EventHandler((e, s) => { player.HandleAttackEnd(); }));

            player.ClientCollider.SetCollisionPolygon(player.AttackColliders[(int)StandardAttacker.AttackEvent.AirAttackUp][0]);
            player.ClientCollider.ActiveCollider = true;
            player.RestrictedMobility = true;

            //Air lunge
            player.RigidBody.AddTranslationalVelocity(new Vector2(0, -lungeSpeed), isGlobal: false);
        }

        public override bool ShouldActivate(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map, StandardAttacker player)
        {
            if (player.RigidBody.VelocityRestrictions[(int)RigidBody.VelocityRestriction.Down] == null && mouseState.LeftButton == ButtonState.Pressed && pastMouseState.LeftButton == ButtonState.Released)
            {
                if (player.SpriteManager.SpriteEventIndex == (int)StandardWalker.EventState.Fall || player.SpriteManager.SpriteEventIndex == (int)StandardWalker.EventState.Jump || player.SpriteManager.SpriteEventIndex == -1)
                {
                    return keyboardState.IsKeyDown(ATTACK_UP_KEY);
                }
            }

            return false;
        }

        public override bool ShouldDisable(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map, StandardAttacker player)
        {
            return false;
        }
    }

    class StandardAirDownAttack : AttackSpecification
    {
        public static Keys ATTACK_DOWN_KEY = Keys.S;

        //Object
        private float lungeSpeed;

        public StandardAirDownAttack(float lungeSpeed)
        {
            this.lungeSpeed = lungeSpeed;
        }

        public override void Activate(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map, StandardAttacker player)
        {
            player.SpriteManager.RemoveSpriteEvent(gameTime);
            player.SpriteManager.SetSpriteEvent(StandardAttacker.AttackEventSpritePosition(StandardAttacker.AttackEvent.AirAttackDown), gameTime, OnEventEnd: new EventHandler((e, s) => { player.HandleAttackEnd(); }));

            player.ClientCollider.SetCollisionPolygon(player.AttackColliders[(int)StandardAttacker.AttackEvent.AirAttackDown][0]);
            player.ClientCollider.ActiveCollider = true;
            player.RestrictedMobility = true;

            //Air lunge
            player.RigidBody.AddTranslationalVelocity(new Vector2(0, lungeSpeed), isGlobal: false);
        }

        public override bool ShouldActivate(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map, StandardAttacker player)
        {
            if (player.RigidBody.VelocityRestrictions[(int)RigidBody.VelocityRestriction.Down] == null && mouseState.LeftButton == ButtonState.Pressed && pastMouseState.LeftButton == ButtonState.Released)
            {
                if (player.SpriteManager.SpriteEventIndex == (int)StandardWalker.EventState.Fall || player.SpriteManager.SpriteEventIndex == (int)StandardWalker.EventState.Jump || player.SpriteManager.SpriteEventIndex == -1)
                {
                    return keyboardState.IsKeyDown(ATTACK_DOWN_KEY);
                }
            }

            return false;
        }

        public override bool ShouldDisable(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map, StandardAttacker player)
        {
            return (player.SpriteManager.SpriteEventIndex == StandardAttacker.AttackEventSpritePosition(StandardAttacker.AttackEvent.AirAttackDown) && player.RigidBody.VelocityRestrictions[(int)RigidBody.VelocityRestriction.Down] != null);
        }
    }
}

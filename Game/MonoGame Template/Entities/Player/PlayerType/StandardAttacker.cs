using FighterGame.Map;
using FighterGame.Map.Effects;
using FighterGame.Network.Message;
using FighterGame.Runtime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonalPhysics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterGame.Entities.Player.PlayerType
{
    public class StandardAttacker : StandardWalker
    {
        public static Keys UP_ATTACK = Keys.W;
        public static Keys LEFT_ATTACK = MOVE_LEFT;
        public static Keys RIGHT_ATTACK = MOVE_RIGHT;
        public static Keys DOWN_ATTACK = Keys.S;

        public enum AttackEvent
        {
            SideAttackLight,
            SideAttackHeavy,
            AirAttackUp,
            AirAttackDown,
            AirAttackSide,
            Hit
        }

        public static int AttackEventSpritePosition(AttackEvent attackEvent) { return Enum.GetNames(typeof(EventState)).Length + (int)attackEvent; }

        //Object
        public readonly Polygon[][] AttackColliders = new Polygon[Enum.GetNames(typeof(AttackEvent)).Length][];

        public StandardAttacker(PlayerTexturePack playerTexturePack, float strafeAcceleration, float maxStrafeVelocity, float jumpVelocity, float dodgeVelocity, AttackSpecification[] attackSpecifications, float mass, CollisionReferenceMap<RigidBody> referenceMap, GameTime gameTime, Vector2 drawDimensions, PlayerStandards.PlayerType PlayerType, GameMap gameMap, bool mobile = true) : base(playerTexturePack, strafeAcceleration, maxStrafeVelocity, jumpVelocity, dodgeVelocity, mass, referenceMap, gameTime, drawDimensions, PlayerType, gameMap, mobile)
        {
            if (attackSpecifications.Length == Enum.GetNames(typeof(AttackEvent)).Length) this.attackSpecifications = attackSpecifications;
            else throw new Exception("Incorrect lunge speed data.");

            BakeAttackColliders(playerTexturePack);
        }

        private void BakeAttackColliders(EntityTexturePack entityTexturePack)
        {
            for (int i = Enum.GetNames(typeof(AttackEvent)).Length - 1; i >= 0; i--)
            {
                AttackColliders[i] = new Polygon[entityTexturePack.EventSheets[AttackEventSpritePosition((AttackEvent)i)].Length];
                for (int j = 0; j < AttackColliders[i].Length; j++)
                {
                    int spriteRightmost = SpriteManager.SpritesheetMaximum(entityTexturePack.EventSheets[AttackEventSpritePosition((AttackEvent)i)][j].spriteSheet, SpriteManager.SpritePosition.Rightmost);
                    int spriteLeftmost = SpriteManager.SpritesheetMaximum(entityTexturePack.EventSheets[AttackEventSpritePosition((AttackEvent)i)][j].spriteSheet, SpriteManager.SpritePosition.Leftmost);
                    int spriteLowest = SpriteManager.SpritesheetMaximum(entityTexturePack.EventSheets[AttackEventSpritePosition((AttackEvent)i)][j].spriteSheet, SpriteManager.SpritePosition.Lowest);
                    int spriteHighest = SpriteManager.SpritesheetMaximum(entityTexturePack.EventSheets[AttackEventSpritePosition((AttackEvent)i)][j].spriteSheet, SpriteManager.SpritePosition.Highest);

                    Vector2[] vertices = new Vector2[] { new Vector2(0, (spriteHighest - (entityTexturePack.EventSheets[AttackEventSpritePosition((AttackEvent)i)][j].spriteSheet.textures[0].Height / 2)) * (DrawDimensions.Y / entityTexturePack.EventSheets[AttackEventSpritePosition((AttackEvent)i)][j].spriteSheet.textures[0].Height)), new Vector2((spriteRightmost - (entityTexturePack.EventSheets[AttackEventSpritePosition((AttackEvent)i)][j].spriteSheet.textures[0].Width / 2)) * (DrawDimensions.X / entityTexturePack.EventSheets[AttackEventSpritePosition((AttackEvent)i)][j].spriteSheet.textures[0].Width), 0), new Vector2(0, (spriteLowest - (entityTexturePack.EventSheets[AttackEventSpritePosition((AttackEvent)i)][j].spriteSheet.textures[0].Height / 2)) * (DrawDimensions.Y / entityTexturePack.EventSheets[AttackEventSpritePosition((AttackEvent)i)][j].spriteSheet.textures[0].Height)), new Vector2((spriteLeftmost - (entityTexturePack.EventSheets[AttackEventSpritePosition((AttackEvent)i)][j].spriteSheet.textures[0].Width / 2)) * (DrawDimensions.X / entityTexturePack.EventSheets[AttackEventSpritePosition((AttackEvent)i)][j].spriteSheet.textures[0].Width), 0) };
                    AttackColliders[i][j] = new Polygon(vertices);
                    AttackColliders[i][j].CenterPoint = Vector2.Zero;
                }
            }
        }

        protected override void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map)
        {
            base.Update(gameTime, keyboardState, pastKeyboardState, mouseState, pastMouseState, map);
            CombatControl(gameTime, keyboardState, pastKeyboardState, mouseState, pastMouseState, map);
        }

        private int lastAttack = -1;
        private void CombatControl(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map)
        {
            //TEST PARTICLES
            if (keyboardState.IsKeyDown(Keys.P) && pastKeyboardState.IsKeyUp(Keys.P))
            {
                playerParticleManager.SpawnDeathParticles(map);
            }

            for (int i = 0; i < attackSpecifications.Length; i++)
            {
                if (attackSpecifications[i] != null && attackSpecifications[i].ShouldActivate(gameTime, keyboardState, pastKeyboardState, mouseState, pastMouseState, map, this))
                {
                    lastAttack = i;
                    attackSpecifications[i].Activate(gameTime, keyboardState, pastKeyboardState, mouseState, pastMouseState, map, this);
                    break;
                }
            }

            if (lastAttack != -1 && attackSpecifications[lastAttack].ShouldDisable(gameTime, keyboardState, pastKeyboardState, mouseState, pastMouseState, map, this)) SpriteManager.RemoveSpriteEvent(gameTime);
        }

        public void HandleAttackEnd()
        {
            RestrictedMobility = false;
            Mobile = true;
            ClientCollider.ActiveCollider = false;
            ClientCollider.SetCollisionPolygon(rigidBody.CollisionPolygon);
            ClientCollider.CollisionDamage = Player.DEFAULT_PLAYER_DAMAGE;
        }

        private AttackSpecification[] attackSpecifications;

        private const float HIT_PARTICLE_VEL = .5F;
        protected override void OnCollision(GameSession gameSession, ColliderInfo collidedInfo, ColliderInfo colliderInfo, Vector2 collisionPosition, bool amCollided)
        {
            base.OnCollision(gameSession, collidedInfo, colliderInfo, collisionPosition, amCollided);
        }

        protected override void OnDeath(GameSession gameSession)
        {
            playerParticleManager.SpawnDeathParticles(gameSession.gameMap);
            gameSession.gameMap.RemoveEntity(EntityIndex, messageOverride: new PlayerRemovalMessage(gameSession.NetworkManager, EntityIndex));
        }
    }

    public abstract class AttackSpecification
    {
        public abstract bool ShouldActivate(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map, StandardAttacker player);
        public abstract void Activate(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map, StandardAttacker player);
        public abstract bool ShouldDisable(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map, StandardAttacker player);
    }
}

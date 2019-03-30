using FighterGame.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Map.Effects;
using PolygonalPhysics;
using Microsoft.Xna.Framework;

namespace FighterGame.Entities.Player
{
    public class PlayerParticleManager
    {
        private PlayerTexturePack playerTexturePack;
        private Polygon playerPolygon;
        private Vector2 drawScale;

        public PlayerParticleManager(PlayerTexturePack playerTexturePack, Polygon playerPolygon, Vector2 drawScale)
        {
            this.drawScale = drawScale;
            this.playerTexturePack = playerTexturePack;
            this.playerPolygon = playerPolygon;
        }

        private const float PLAYER_SIZE_PORTION = .25F;
        private const float MAX_RANDOM_VELOCITY = 1F;
        public void SpawnDeathParticles(GameMap gameMap)
        {
            RigidBody.VelocityRestriction[] velocityRestrictions = RigidBody.AllRestrictions;

            for (int i = 0; i < playerTexturePack.DeathParticles.Length; i++)
            {
                gameMap.AddEntity(new SpriteParticle(gameMap, gameMap.gameSession.LatestGameTime, new RectangleF(playerPolygon.CenterPoint.X - (drawScale.X * PLAYER_SIZE_PORTION / 2), playerPolygon.CenterPoint.Y - (drawScale.Y * PLAYER_SIZE_PORTION / 2), drawScale.X * PLAYER_SIZE_PORTION, drawScale.Y * PLAYER_SIZE_PORTION), new Vector2(MAX_RANDOM_VELOCITY - (2 * MAX_RANDOM_VELOCITY * (float)gameMap.gameSession.LocalRandom.NextDouble()), MAX_RANDOM_VELOCITY - (2 * MAX_RANDOM_VELOCITY * (float)gameMap.gameSession.LocalRandom.NextDouble())), playerTexturePack.DeathParticles[i], enableRotation: true, allowedCollisionsOverride: velocityRestrictions));
            }
        }

        public void SpawnBlood(GameMap gameMap)
        {
            gameMap.AddEntity(new SpriteParticle(gameMap, gameMap.gameSession.LatestGameTime, playerPolygon.BoundaryRectangle, new Vector2(MAX_RANDOM_VELOCITY - (2 * MAX_RANDOM_VELOCITY * (float)gameMap.gameSession.LocalRandom.NextDouble()), MAX_RANDOM_VELOCITY - (2 * MAX_RANDOM_VELOCITY * (float)gameMap.gameSession.LocalRandom.NextDouble())), playerTexturePack.BloodSheets[gameMap.gameSession.LocalRandom.Next(0, playerTexturePack.BloodSheets.Length)], endAfterLoop: true, ignoreCollisions: true));
        }

        public void SpawnDamageNotifier(float damage, GameMap gameMap)
        {
            string additive = "";
            Color drawColor = Color.LightGreen;
            if (damage < 0) drawColor = Color.Red;
            else additive += "+";

            gameMap.AddEntity(new TextParticle(additive + damage.ToString(), gameMap, gameMap.gameSession.LatestGameTime, playerPolygon.BoundaryRectangle, new Vector2(MAX_RANDOM_VELOCITY - (2 * MAX_RANDOM_VELOCITY * (float)gameMap.gameSession.LocalRandom.NextDouble()), MAX_RANDOM_VELOCITY - (2 * MAX_RANDOM_VELOCITY * (float)gameMap.gameSession.LocalRandom.NextDouble())), drawColor));
        }
    }
}

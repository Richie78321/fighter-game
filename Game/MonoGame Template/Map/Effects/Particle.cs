using FighterGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolygonalPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FighterGame.Map.Effects
{
    abstract class Particle : LocalEntity
    {
        private const float DEFAULT_PARTICLE_MASS = 1F;
        public const int DEFAULT_PARTICLE_LIFETIME = 10000;

        //Object
        protected RigidBody rigidBody;
        protected RectangleF collisionRectangle;
        private double timeOfCreation;
        private int lifetime;

        private bool strictBounds;

        public Particle(GameMap gameMap, GameTime gameTime, RectangleF collisionRectangle, Vector2 initialVelocity, int lifetime = DEFAULT_PARTICLE_LIFETIME, bool ignoreCollisions = false, bool enableRotation = false)
        {
            strictBounds = enableRotation;
            this.collisionRectangle = collisionRectangle;
            this.lifetime = lifetime;
            timeOfCreation = gameTime.TotalGameTime.TotalMilliseconds;
            if (ignoreCollisions)
            {
                RigidBody.VelocityRestriction[] velocityRestrictions = new RigidBody.VelocityRestriction[Enum.GetNames(typeof(RigidBody.VelocityRestriction)).Length];
                for (int i = 0; i < velocityRestrictions.Length; i++) velocityRestrictions[i] = (RigidBody.VelocityRestriction)i;
                rigidBody = new RigidBody(new RotationRectangle(collisionRectangle), DEFAULT_PARTICLE_MASS, gameMap.NoCollideMap, velocityRestrictions, staticBody: false, rotationOnForce: enableRotation);
            }
            else rigidBody = new RigidBody(new RotationRectangle(collisionRectangle), DEFAULT_PARTICLE_MASS, gameMap.ParticleReferenceMap, new RigidBody.VelocityRestriction[0], staticBody: false, rotationOnForce: enableRotation);
            rigidBody.SetTranslationalVelocity(initialVelocity, isGlobal: false);
        }

        protected override void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map)
        {
            //Apply gravity
            float frameTimePortion = (float)gameTime.ElapsedGameTime.TotalMilliseconds / RigidBody.TARGET_FRAME_TIME;
            rigidBody.AddTranslationalVelocity(new Vector2(0, MapStandards.GRAVITY_ACCELERATION * frameTimePortion), isGlobal: false);
            rigidBody.Update(gameTime);

            //Check expiration
            if (gameTime.TotalGameTime.TotalMilliseconds - timeOfCreation >= lifetime || (strictBounds && !rigidBody.ReferenceMap.InBounds(rigidBody.CollisionPolygon.BoundaryRectangle)))
            {
                Dispose(map);
            }
        }

        protected void Dispose(GameMap gameMap)
        {
            rigidBody.RemoveFromReferenceMap();
            gameMap.RemoveEntity(EntityIndex);
        }
    }
}

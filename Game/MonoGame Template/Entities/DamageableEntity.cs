using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Entities.Server.CollisionDetection;
using FighterGame.Map;
using FighterGame.Map.Effects;
using FighterGame.Network.Message;
using FighterGame.Runtime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonalPhysics;

namespace FighterGame.Entities
{
    public abstract class DamageableEntity : LocalEntity
    {
        public void SetInitialHealth(float initialHealth) { this.initialHealth = initialHealth; }
        private float initialHealth;
        public float InitialHealth => initialHealth;
        private float currentHealth;
        public float CurrentHealth => currentHealth;

        private ClientCollider clientCollider;
        public ClientCollider ClientCollider => clientCollider;

        public DamageableEntity(float initialHealth, int initialLives, Polygon collisionPolygon, GameMap gameMap)
        {
            clientCollider = new ClientCollider(collisionPolygon, HandleCollision);
            gameMap.AddEntity(clientCollider);

            currentLives = initialLives;
            currentHealth = initialHealth;
            this.initialHealth = initialHealth;
        }

        private int currentLives;
        public int CurrentLives => currentLives;

        public bool ApplyDamage(float damage, GameSession gameSession)
        {
            currentHealth -= DamageMultipler(damage);
            if (currentHealth <= 0)
            {
                if (damage != 0) OnHealthChange();
                if (currentLives > 1)
                {
                    //Consume life
                    currentLives--;
                    ApplyHealing(float.MaxValue * 2);
                    OnConsumeLife();
                    return false;
                }
                else
                {
                    //Die
                    DeathSequence(gameSession);
                    return true;
                }
            }
            else
            {
                if (damage != 0) OnHealthChange();
                return false;
            }
        }

        public void ApplyHealing(float healing)
        {
            float newHealth = MathHelper.Clamp(currentHealth + HealingMultiplier(healing), 0, initialHealth);
            bool different = newHealth != currentHealth;
            currentHealth = newHealth;
            if (different)
            {
                OnHealthChange();
                if (currentHealth > 0 && IsDead) dead = false;
            }
        }

        private const float HIT_PARTICLE_VEL = .5F;
        private void HandleCollision(GameSession gameSession, ColliderInfo collidedInfo, ColliderInfo colliderInfo, Vector2 collisionPosition, bool amCollided)
        {
            if (amCollided)
            {
                //Apply damage
                if (ApplyDamage(colliderInfo.CollisionDamage, gameSession)) return;
            }

            OnCollision(gameSession, collidedInfo, colliderInfo, collisionPosition, amCollided);
        }

        protected abstract void OnCollision(GameSession gameSession, ColliderInfo collidedInfo, ColliderInfo colliderInfo, Vector2 collisionPosition, bool amCollided);

        protected virtual float HealingMultiplier(float healing) { return healing; }

        protected virtual float DamageMultipler(float damage) { return damage; }

        protected abstract void OnConsumeLife();

        protected abstract void OnHealthChange();

        private bool dead = false;
        public bool IsDead => dead;

        private void DeathSequence(GameSession gameSession)
        {
            dead = true;
            OnDeath(gameSession);
        }

        protected abstract void OnDeath(GameSession gameSession);
    }
}

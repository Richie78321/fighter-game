using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FighterGame.Map;
using FighterGame.Runtime;

namespace FighterGame.Entities
{
    [Serializable]
    public abstract class Entity
    {
        private static int LastEntityIndex = -1;

        //Object
        private int entityIndex = -1;
        public int EntityIndex => entityIndex;
        public void SetEntityIndex(int entityIndex)
        {
            if (this.entityIndex == -1) this.entityIndex = entityIndex;
        }

        public Entity(bool localEntity = true)
        {
            if (localEntity) SetEntityIndex(++LastEntityIndex);
        }

        public abstract void Draw(SpriteBatch spriteBatch, GameMap map);

        protected abstract void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState pastKeyboardState, MouseState mouseState, MouseState pastMouseState, GameMap map);

        private KeyboardState pastKeyboardState = Keyboard.GetState();
        private MouseState pastMouseState = Mouse.GetState();
        public void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState, GameMap map)
        {
            Update(gameTime, keyboardState, pastKeyboardState, mouseState, pastMouseState, map);
            pastKeyboardState = keyboardState;
            pastMouseState = mouseState;
        }

        public virtual void OnRemove() { }
    }
}

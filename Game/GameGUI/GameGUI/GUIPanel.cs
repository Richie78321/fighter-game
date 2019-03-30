using GameGUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameGUI
{
    public class GUIPanel : GUIElement
    {
        private List<GUIElement> containedElements = new List<GUIElement>();
        protected object elementLock = new object();

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                if (base.Enabled != value)
                {
                    foreach (GUIElement containedElement in containedElements)
                    {
                        containedElement.Enabled = value;
                    }
                }
                base.Enabled = value;
            }
        }

        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                if (base.Visible != value)
                {
                    foreach (GUIElement containedElement in containedElements)
                    {
                        containedElement.Visible = value;
                    }
                }
                base.Visible = value;
            }
        }

        public void AddElement(GUIElement elementToAdd)
        {
            lock (elementLock)
            {
                containedElements.Add(elementToAdd);
            }
        }

        public void RemoveElement(GUIElement elementToRemove)
        {
            lock (elementLock)
            {
                containedElements.Remove(elementToRemove);
            }
        }

        public void RemoveElement(int index)
        {
            containedElements.RemoveAt(index);
        }

        protected void ClearElements()
        {
            lock (elementLock) containedElements.Clear();
        }

        private bool consumeMouse;

        public GUIElement[] ContainedElements => containedElements.ToArray();

        public GUIPanel(Rectangle elementRectangle, bool consumeMouse = false, bool fadeVisibility = false)
        {
            this.consumeMouse = consumeMouse;
            base.elementRectangle = elementRectangle;
        }

        protected override void HandleDraw(SpriteBatch spriteBatch, Rectangle scissorRectangle, DrawSpecifics drawSpecifics)
        {
            Rectangle containedScissorRectangle = Rectangle.Intersect(scissorRectangle, elementRectangle);
            lock (elementLock)
            {
                for (int i = 0; i < containedElements.Count; i++)
                {
                    containedElements[i].Draw(spriteBatch, containedScissorRectangle, drawSpecifics);
                }
            }
        }

        protected override void HandleInteract(GUIInput guiInput)
        {
            lock (elementLock)
            {
                for (int i = containedElements.Count - 1; i >= 0; i--)
                {
                    containedElements[i].Interact(guiInput);
                }

                if (consumeMouse && elementRectangle.Contains(guiInput.MouseState.Position)) guiInput.MouseConsumed = true;
            }
        }

        protected override void HandleUpdate(GameTime gameTime)
        {
            lock (elementLock)
            {
                for (int i = 0; i < containedElements.Count; i++)
                {
                    containedElements[i].Update(gameTime);
                }
            }
        }
    }
}

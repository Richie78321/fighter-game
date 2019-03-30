using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Runtime;
using GameGUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using FighterGame.GUI.Health;
using FighterGame.GUI.Inventory;

namespace FighterGame.GUI
{
    public class GraphicsUI : GUIPanel
    {
        public static void LoadContent(ContentManager Content, GraphicsDevice graphicsDevice)
        {
            GameButton.LoadContent(Content);
            GamePanel.LoadContent(Content);
            MessageBox.GraphicsDevice = graphicsDevice;
            HealthBar.LoadContent(Content, graphicsDevice);
            MainMenu.LoadContent(Content);
            InventoryPanel.LoadContent(Content);
        }

        //Object
        private Menu currentMenu = null;
        public Menu CurrentMenu => currentMenu;
        private Menu previousMenu;
        public void SetCurrentMenu(Menu menu)
        {
            previousMenu = currentMenu;
            currentMenu = menu;

            if (previousMenu != null)
            {
                previousMenu.Visible = false;
                previousMenu.Enabled = false;
            }

            if (currentMenu != null)
            {
                currentMenu.Visible = true;
                currentMenu.Enabled = true;
            }
        }

        public void GoPreviousMenu()
        {
            Menu temp = currentMenu;
            currentMenu = previousMenu;
            previousMenu = temp;

            if (previousMenu != null)
            {
                previousMenu.Visible = false;
                previousMenu.Enabled = false;
            }

            if (currentMenu != null)
            {
                currentMenu.Visible = true;
                currentMenu.Enabled = true;
            }
        }

        private readonly GUIPanel ActiveGamePanel;
        public readonly HealthBarManager HealthBarManager;
        public readonly MainMenu MainMenu;
        public readonly MessageManager MessageManager;
        public readonly BackgroundPanel BackgroundPanel;
        public readonly LoginMenu LoginMenu;
        public readonly SettingsMenu SettingsMenu;
        public readonly InventoryMenu InventoryMenu;

        public GraphicsUI(GraphicsDevice graphicsDevice, GameSession gameSession) : base(new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height))
        {
            MainMenu = new MainMenu(graphicsDevice, gameSession);
            HealthBarManager = new HealthBarManager(graphicsDevice);
            BackgroundPanel = new BackgroundPanel(new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), gameSession);
            LoginMenu = new LoginMenu(new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), gameSession);
            SettingsMenu = new SettingsMenu(graphicsDevice, gameSession);
            InventoryMenu = new InventoryMenu(new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), gameSession);
            ActiveGamePanel = new GUIPanel(new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height));
            ActiveGamePanel.AddElement(HealthBarManager);

            AddElement(ActiveGamePanel);
            AddElement(BackgroundPanel);
            AddElement(MainMenu);
            AddElement(InventoryMenu);
            AddElement(LoginMenu);
            AddElement(SettingsMenu);

            //Disable all
            foreach (GUIElement e in ContainedElements) InitElement(e);

            BackgroundPanel.Visible = true;
            BackgroundPanel.Enabled = true;

            //Constant menus
            MessageManager = new MessageManager(new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), gameSession);
            AddElement(MessageManager);

            //Set initial menu
            SetCurrentMenu(LoginMenu);
        }

        private void InitElement(GUIElement element)
        {
            element.Visible = false;
            element.Enabled = false;
        }

        public void EnableActiveGamePanel(bool enableStatus)
        {
            ActiveGamePanel.Visible = enableStatus;
            ActiveGamePanel.Enabled = enableStatus;
            if (!enableStatus) HealthBarManager.ClearHealthBars();
        }
    }
}

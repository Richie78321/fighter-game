using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Map.Background;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FighterGame.Map
{
    public class MapTextureLoader
    {
        public const string MAP_FOLDER_DIRECTORY = "Map/";
        public const string BACKGROUND_TEXTURE_FOLDER = "Background";

        //Object
        private string textureDirectoryName;
        private ContentManager Content;

        private PlatformTextureLoader platformTextureLoader;

        public MapTextureLoader(string textureDirectoryName, ContentManager Content)
        {
            this.Content = Content;
            this.textureDirectoryName = textureDirectoryName;

            platformTextureLoader = new PlatformTextureLoader(textureDirectoryName, Content);
        }

        public MapTexturePack Deploy()
        {
            List<BackgroundImage> backgroundImages = new List<BackgroundImage>();
            for (int i = 1; i < ParallaxBackground.MAX_BACKGROUND_DEPTH; i++)
            {
                try
                {
                    Texture2D backgroundTexture = Content.Load<Texture2D>(MAP_FOLDER_DIRECTORY + "/" + textureDirectoryName + "/"  + BACKGROUND_TEXTURE_FOLDER + "/" + i.ToString());
                    backgroundImages.Add(new BackgroundImage(backgroundTexture, i));
                }
                catch { }
            }

            return new MapTexturePack(platformTextureLoader.Deploy(), backgroundImages.ToArray());
        }
    }

    public class PlatformTextureLoader
    {
        public const string PLATFORM_TEXTURE_FOLDER = "Platform";
        public const string PLATFORM_TOP_TEXTURE = "top";
        public const string PLATFORM_BOTTOM_TEXTURE = "bottom";

        //Object
        private string textureDirectoryName;
        private ContentManager Content;

        public PlatformTextureLoader(string textureDirectoryName, ContentManager Content)
        {
            this.textureDirectoryName = textureDirectoryName;
            this.Content = Content;
        }

        public PlatformTexturePack Deploy()
        {
            Texture2D[] topTextures = new Texture2D[Enum.GetNames(typeof(PlatformTexturePack.PlatformTexture)).Length];
            for (int i = 0; i < topTextures.Length; i++) topTextures[i] = Content.Load<Texture2D>(MapTextureLoader.MAP_FOLDER_DIRECTORY + "/" + textureDirectoryName + "/" + PLATFORM_TEXTURE_FOLDER + "/" + PLATFORM_TOP_TEXTURE + ((PlatformTexturePack.PlatformTexture)i).ToString());

            Texture2D[] bottomTextures = new Texture2D[Enum.GetNames(typeof(PlatformTexturePack.PlatformTexture)).Length];
            for (int i = 0; i < bottomTextures.Length; i++) bottomTextures[i] = Content.Load<Texture2D>(MapTextureLoader.MAP_FOLDER_DIRECTORY + "/" + textureDirectoryName + "/" + PLATFORM_TEXTURE_FOLDER + "/" + PLATFORM_BOTTOM_TEXTURE + ((PlatformTexturePack.PlatformTexture)i).ToString());

            return new PlatformTexturePack(topTextures, bottomTextures);
        }
    }
}

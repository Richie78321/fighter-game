using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using FighterGame.Entities;
using PolygonalPhysics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FighterGame.Entities.Player;
using FighterGame.Runtime;
using FighterGame.Network.Message;
using FighterGame.Map.Background;

namespace FighterGame.Map
{
    public class GameMap
    {
        public static Keys PLATFORM_FALLTHROUGH_KEY = Keys.LeftShift;

        //Object
        public static Polygon SERVERCP;

        public static MapArchitecture[] MapArchitectures;
        public static MapTextureLoader[] MapTexturePacks;

        public static void LoadMapContent(ContentManager Content)
        {
            //Deserialize map architectures
            MapArchitectures = (MapArchitecture[])new BinaryFormatter().Deserialize(new FileStream(Directory.GetCurrentDirectory() + "/" + MapStandards.MAP_COLLECTION_FILE_NAME + ".bin", FileMode.Open, FileAccess.Read));

            //Load map texture packs
            List<MapTextureLoader> mapTexturePacks = new List<MapTextureLoader>();
            mapTexturePacks.Add(new MapTextureLoader("placeholder", Content));

            MapTexturePacks = mapTexturePacks.ToArray();
        }

        //Object
        private MapArchitecture mapArchitecture;
        public MapArchitecture MapArchitecture => mapArchitecture;
        public MapTexturePack texturePack;

        private Camera camera;

        private ParallaxBackground parallaxBackground;

        public readonly GameSession gameSession;
        public Random CurrentRandom => gameSession.NetworkManager.ServerRandom;

        public GameMap(Random random, GraphicsDevice graphicsDevice, GameSession gameSession)
        {
            this.gameSession = gameSession;
            mapArchitecture = MapArchitectures[random.Next(0, MapArchitectures.Length)];

            texturePack = MapTexturePacks[random.Next(0, MapTexturePacks.Length)].Deploy();
            ReferenceMap = new CollisionReferenceMap<RigidBody>(MapStandards.REFERENCE_MAP_RESOLUTION, MapStandards.REFERENCE_MAP_RESOLUTION, MapStandards.MAP_SIZE, MapStandards.MAP_SIZE);
            ParticleReferenceMap = new CollisionReferenceMap<RigidBody>(MapStandards.REFERENCE_MAP_RESOLUTION, MapStandards.REFERENCE_MAP_RESOLUTION, MapStandards.MAP_SIZE, MapStandards.MAP_SIZE);
            NoCollideMap = new CollisionReferenceMap<RigidBody>(1, 1, MapStandards.MAP_SIZE, MapStandards.MAP_SIZE);

            parallaxBackground = new ParallaxBackground(texturePack.BackgroundImages);

            camera = new Camera(this, graphicsDevice);

            AddArchitectureCollision();
        }

        public void RemoveEntity(int entityIndex, bool networkSync = true, bool serverEntity = false, EntityRemovalMessage messageOverride = null)
        {
            Entity[] entities = Entities;
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] is LocalEntity b && b.EntityIndex == entityIndex)
                {
                    this.entities.Remove(entities[i]);
                    
                    if (networkSync)
                    {
                        if (messageOverride != null)
                        {
                            gameSession.NetworkManager.SendMessage(messageOverride);
                        }
                        else
                        {
                            gameSession.NetworkManager.SendMessage(new EntityRemovalMessage(gameSession.NetworkManager, entityIndex, serverEntity: serverEntity));
                        }
                    }

                    return;
                }
            }
        }

        public void RemoveEntity(int clientIndex, int entityIndex)
        {
            NetworkEntity[] networkEntities = NetworkEntities;
            for (int i = 0; i < networkEntities.Length; i++)
            {
                if (networkEntities[i].ClientIndex == clientIndex && networkEntities[i].EntityIndex == entityIndex)
                {
                    entities.Remove(networkEntities[i]);
                    return;
                }
            }
        }

        private RigidBody[] platformRigidBodies;
        private void AddArchitectureCollision()
        {
            RigidBody.VelocityRestriction[] velocityRestrictions = new RigidBody.VelocityRestriction[3];
            velocityRestrictions[0] = RigidBody.VelocityRestriction.Down;
            velocityRestrictions[1] = RigidBody.VelocityRestriction.Left;
            velocityRestrictions[2] = RigidBody.VelocityRestriction.Right;

            platformRigidBodies = new RigidBody[mapArchitecture.MapPlatforms.Length];
            for (int i = 0; i < mapArchitecture.MapPlatforms.Length; i++)
            {
                platformRigidBodies[i] = new RigidBody(new RotationRectangle(new RectangleF(mapArchitecture.MapPlatforms[i].Position, new Vector2(mapArchitecture.MapPlatforms[i].TileLength * MapStandards.TILE_SIZE, 0)), 0), 1, ReferenceMap, allowedCollisions: velocityRestrictions, staticBody: true, friction: .1F);
                new RigidBody(new RotationRectangle(new RectangleF(mapArchitecture.MapPlatforms[i].Position, new Vector2(mapArchitecture.MapPlatforms[i].TileLength * MapStandards.TILE_SIZE, .1F)), 0), 1, ParticleReferenceMap, allowedCollisions: velocityRestrictions, staticBody: true, friction: .1F);
            }
        }

        public readonly CollisionReferenceMap<RigidBody> ReferenceMap;
        public readonly CollisionReferenceMap<RigidBody> ParticleReferenceMap;
        public readonly CollisionReferenceMap<RigidBody> NoCollideMap;

        private List<Entity> entities = new List<Entity>();
        public Entity[] Entities => entities.ToArray(); 
        public void AddEntity(Entity entityToAdd)
        {
            entities.Add(entityToAdd);
            if (entityToAdd is Player player)
            {
                players.Add(player);
            }
            else if (entityToAdd is NetworkPlayer networkPlayer)
            {
                networkPlayers.Add(networkPlayer);
            }

            if (entityToAdd is NetworkEntity networkEntity) networkEntities.Add(networkEntity);
        }

        public LocalEntity GetLocalEntity(int entityIndex)
        {
            Entity[] entities = Entities;
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] is LocalEntity b && b.EntityIndex == entityIndex) return b;
            }
            return null;
        }

        private List<Player> players = new List<Player>();
        public Player[] Players => players.ToArray();

        private List<NetworkPlayer> networkPlayers = new List<NetworkPlayer>();
        public NetworkPlayer[] NetworkPlayers => networkPlayers.ToArray();

        private List<NetworkEntity> networkEntities = new List<NetworkEntity>();
        public NetworkEntity[] NetworkEntities => networkEntities.ToArray();

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            Vector2 focusPoint = camera.FocusPoint;
            float scale = camera.Scale;

            //Draw background
            parallaxBackground.Draw(spriteBatch, focusPoint, scale, graphicsDevice);

            spriteBatch.End();

            Matrix testMatrix = Matrix.CreateScale(scale);
            testMatrix *= Matrix.CreateTranslation((graphicsDevice.Viewport.Width / 2) - (focusPoint.X * scale), (graphicsDevice.Viewport.Height / 2) - (focusPoint.Y * scale), 0);

            //Test
            spriteBatch.Begin(transformMatrix: testMatrix, samplerState: SamplerState.PointClamp);

            //Draw architecture
            mapArchitecture.Draw(spriteBatch, texturePack);

            //Draw entities
            for (int i = 0; i < entities.Count; i++) entities[i].Draw(spriteBatch, this);

            spriteBatch.End();
        }

        private KeyboardState pastKeyboardState = Keyboard.GetState();
        private MouseState pastMouseState = Mouse.GetState();
        public void Update(GraphicsDevice graphicsDevice, GameTime gameTime, KeyboardState keyboardState, MouseState mouseState)
        {
            //Check for fallthrough
            if (keyboardState.IsKeyDown(PLATFORM_FALLTHROUGH_KEY) && pastKeyboardState.IsKeyUp(PLATFORM_FALLTHROUGH_KEY))
            {
                //Fallthrough on
                SetPlatformFallthrough(true);
            }
            else if (keyboardState.IsKeyUp(PLATFORM_FALLTHROUGH_KEY) && pastKeyboardState.IsKeyDown(PLATFORM_FALLTHROUGH_KEY))
            {
                //Fallthrough off
                SetPlatformFallthrough(false);
            }

            //Update entities
            for (int i = 0; i < entities.Count; i++) entities[i].Update(gameTime, keyboardState, mouseState, this);

            camera.Update(graphicsDevice, gameTime);

            //Update pasts
            pastKeyboardState = keyboardState;
            pastMouseState = mouseState;
        }

        private void SetPlatformFallthrough(bool value)
        {
            List<RigidBody.VelocityRestriction> velocityRestrictions = new List<RigidBody.VelocityRestriction>();
            velocityRestrictions.Add(RigidBody.VelocityRestriction.Down);
            velocityRestrictions.Add(RigidBody.VelocityRestriction.Left);
            velocityRestrictions.Add(RigidBody.VelocityRestriction.Right);
            if (value) velocityRestrictions.Add(RigidBody.VelocityRestriction.Up);

            foreach (RigidBody b in platformRigidBodies)
            {
                b.AllowedCollisions = velocityRestrictions.ToArray();
            }
        }

        public NetworkPlayer FindNetworkPlayer(int clientIndex)
        {
            NetworkPlayer[] networkPlayers = NetworkPlayers;
            for (int i = 0; i < networkPlayers.Length; i++)
            {
                if (networkPlayers[i].ClientIndex == clientIndex) return networkPlayers[i];
            }

            return null;
        }

        public void RemoveClientEntities(byte clientIndex)
        {
            List<Entity> entitiesToRemove = new List<Entity>();
            for (int i = 0; i < entities.Count; i++) if (entities[i] is NetworkEntity b) if (b.ClientIndex == clientIndex) entitiesToRemove.Add(entities[i]);
            for (int i = 0; i < entitiesToRemove.Count; i++) entities.Remove(entitiesToRemove[i]);
        }
    }
}

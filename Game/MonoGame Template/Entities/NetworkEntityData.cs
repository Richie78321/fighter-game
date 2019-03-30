using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Runtime;
using Microsoft.Xna.Framework;
using ProtoBuf;
using FighterGame.Entities.Player;
using System.ComponentModel;
using ProtoBuf.Meta;
using System.Reflection;
using FighterGame.Network.Server;

namespace FighterGame.Entities
{
    public abstract class NetworkEntityData
    {
        /// <summary>
        /// This lovely class used to employ generics for type security; sadly, protocol buffers do not support generics...
        /// </summary>
        private const int INDEX_OFFSET = 100;

        public static void LoadSerialization()
        {
            //Add subtypes
            if (!RuntimeTypeModel.Default.IsDefined(typeof(NetworkEntityData)))
            {
                MetaType entityDataType = RuntimeTypeModel.Default.Add(typeof(NetworkEntityData), false).Add(GetFields(typeof(NetworkEntityData)));
                entityDataType.UseConstructor = false;
                Type[] subTypes = Assembly.GetAssembly(typeof(NetworkEntityData)).GetTypes().Where(t => (t.IsSubclassOf(typeof(NetworkEntityData)))).ToArray();
                for (int i = 0; i < subTypes.Length; i++)
                {
                    RuntimeTypeModel.Default.Add(subTypes[i], false).Add(GetFields(subTypes[i])).UseConstructor = false;
                    entityDataType.AddSubType(INDEX_OFFSET + i, subTypes[i]);
                }
            }
        }

        private static string[] GetFields(Type type)
        {
            FieldInfo[] fields = type.GetFields();
            FieldInfo[] parentFields = type.BaseType.GetFields();
            List<string> fieldNames = new List<string>();
            for (int i = 0; i < fields.Length; i++)
            {
                if (!parentFields.Contains(fields[i])) fieldNames.Add(fields[i].Name);
            }
            return fieldNames.ToArray();
        }

        //Object
        public readonly int EntityIndex;
        public readonly int ClientIndex;

        public NetworkEntityData(LocalEntity entity, int clientIndex)
        {
            EntityIndex = entity.EntityIndex;
            this.ClientIndex = clientIndex;

            CollectData(entity);
        }

        protected abstract void CollectData(LocalEntity entity);

        public NetworkEntity DeployNetworkEntity(GameTime gameTime, GameSession gameSession, Lobby lobby)
        {
            NetworkEntity networkEntity = CreateNetworkEntity(gameTime, gameSession, lobby, ClientIndex, EntityIndex);

            //Set indices
            networkEntity.SetClientIndex(ClientIndex);
            networkEntity.SetEntityIndex(EntityIndex);

            return networkEntity;
        }

        protected abstract NetworkEntity CreateNetworkEntity(GameTime gameTime, GameSession gameSession, Lobby lobby, int clientIndex, int entityIndex);

        public abstract void SetNetworkEntity(NetworkEntity networkEntity, GameSession gameSession);
    }

    public class TypeDisagreementException : Exception
    {
        public override string Message => "The type of the entity inputted does not match the intended type. (This likely means that there has been an unintended disconnect between client and entity indices)";
    }
}

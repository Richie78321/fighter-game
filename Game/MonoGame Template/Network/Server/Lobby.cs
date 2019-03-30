using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Network.Message;
using FighterGame.Entities;
using PolygonalPhysics;
using FighterGame.Map;

namespace FighterGame.Network.Server
{
    public class Lobby
    {
        private Client hostClient;
        public Client HostClient => hostClient;
        private List<Client> clients = new List<Client>();
        public Client.Print PrintMethod;
        public Client[] Clients => clients.ToArray();

        private int lobbySeed;
        public int LobbySeed => lobbySeed;

        public readonly CollisionReferenceMap<Collider> ColliderReferenceMap; 

        public Lobby(Client hostClient, List<Lobby> lobbyCollection, Client.Print printMethod, Random serverRandom)
        {
            //Set vars
            this.hostClient = hostClient;
            this.lobbyCollection = lobbyCollection;
            this.PrintMethod = printMethod;
            lobbySeed = serverRandom.Next();
            ColliderReferenceMap = new CollisionReferenceMap<Collider>(MapStandards.REFERENCE_MAP_RESOLUTION, MapStandards.REFERENCE_MAP_RESOLUTION, MapStandards.MAP_SIZE, MapStandards.MAP_SIZE);

            //Add host to clients
            clients.Add(hostClient);
        }

        public bool IsJoinable => (clients.Count < hostClient.ClientInfo.LobbyInfo.TargetPlayers && !IsRunning);

        /// <summary>
        /// Reference to Matchmaker lobby collection (for self removal upon closure)
        /// </summary>
        private List<Lobby> lobbyCollection;
        public void RemoveClient(Client clientToRemove, bool lobbyClose)
        {
            //Remove associated network entities
            lock (networkEntityLock)
            {
                NetworkEntity[] networkEntities = NetworkEntities;
                for (int i = 0; i < networkEntities.Length; i++)
                {
                    if (networkEntities[i].ClientIndex == clientToRemove.ClientIndex)
                    {
                        this.networkEntities.Remove(networkEntities[i]);
                        networkEntities[i].OnRemove();
                    }
                }
            }

            //Remove client
            if (clients.Remove(clientToRemove) && !lobbyClose && (clients.Count <= 1))
            {
                //Minimum players lost or host client lost, close lobby
                CloseLobby(true);
            }
            else if (!lobbyClose)
            {
                //Forward disconnect message to remaining players
                ForwardToClients(clientToRemove, new ClientDisconnectMessage(clientToRemove));
            }
        }

        public object entityCheckLock = new object();
        private object networkEntityLock = new object();
        private List<NetworkEntity> networkEntities = new List<NetworkEntity>();
        public NetworkEntity[] NetworkEntities => networkEntities.ToArray();
        public void AddNetworkEntity(NetworkEntity networkEntity)
        {
            lock (networkEntityLock) networkEntities.Add(networkEntity);
        }

        public void RemoveNetworkEntity(int clientIndex, int entityIndex)
        {
            NetworkEntity[] networkEntities = NetworkEntities;
            for (int i = 0; i < networkEntities.Length; i++)
            {
                if (networkEntities[i].ClientIndex == clientIndex && networkEntities[i].EntityIndex == entityIndex)
                {
                    lock (networkEntityLock) this.networkEntities.Remove(networkEntities[i]);
                    networkEntities[i].OnRemove();
                    break;
                }
            }
        }

        private bool isRunning = false;
        public bool IsRunning => isRunning;
        public bool IsReady
        {
            get
            {
                bool clientCount = clients.Count >= hostClient.ClientInfo.LobbyInfo.TargetPlayers;
                if (clientCount)
                {
                    for (int i = 0; i < clients.Count; i++) if (!clients[i].IsReady) return false;
                    return true;
                }
                else return false;
            }
        }

        public void SendClientInformation(Client recipient)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i] != recipient) recipient.SendMessage(new PeerClientInfoMessage(clients[i].PeerClientInfo));
            }
        }

        public bool AddClient(Client clientToAdd)
        {
            //Check if lobby is joinable
            if (IsJoinable)
            {
                //Add client to lobby
                clients.Add(clientToAdd);

                return true;
            }
            else return false;
        }

        public void CheckReadiness()
        {
            //Check if requirements have been reached
            if (IsReady)
            {
                isRunning = true;
                ForwardToClients(null, new GameStartMessage());
            }
        }

        /// <summary>
        /// Close the lobby.
        /// </summary>
        /// <param name="playerLoss">Value representing if the close was triggered by player loss.</param>
        public void CloseLobby(bool playerLoss)
        {
            //Close remaining players
            while (clients.Count > 0) clients[0].Close(false, true);

            //Remove lobby from lobby collection
            if (!lobbyCollection.Remove(this)) throw new Exception("Unauthorized edits made to the matchmaking lobby list!");

            isRunning = false;

            //Send message
            PrintMethod.Invoke("Lobby closed. Player Loss: " + playerLoss);
        }

        public void ForwardToClients(Client sender, IClientMessage clientMessage, bool sendTCP = true)
        {
            //Forward message to all clients
            for (int i = 0; i < clients.Count; i++)
            {
                //Ensure not sender
                if (clients[i] != sender) clients[i].SendMessage(clientMessage, sendTCP);
            }
        }

        public void SendToClient(int clientIndex, IClientMessage clientMessage)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].ClientIndex == clientIndex)
                {
                    clients[i].SendMessage(clientMessage);
                    return;
                }
            }
        }
    }
}

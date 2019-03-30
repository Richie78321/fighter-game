using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using FighterGame.Network.Server;
using FighterGame.Network.Client;

namespace FighterMatchmaker
{
    class Matchmaker
    {
        private const int SERVER_PORT = 7832;

        private TcpListener tcpListener;
        private Random serverRandom = new Random();

        /// <summary>
        /// Instantiates a new matchmaker object.
        /// </summary>
        /// <param name="tcpListener">The tcp listener to listen for clients.</param>
        public Matchmaker(TcpListener tcpListener)
        {
            this.tcpListener = tcpListener;
        }

        /// <summary>
        /// Starts the client listener.
        /// </summary>
        public void StartClientListener()
        {
            ClientListener();
        }

        private bool enabled = true;
        /// <summary>
        /// Listens for clients.
        /// </summary>
        private async void ClientListener()
        {
            while (enabled)
            {
                //Await a client
                try
                {
                    new Client(await tcpListener.AcceptTcpClientAsync(), new Client.LobbySorter(LobbySorter), new Client.Print((b) => { Console.WriteLine(b); }));
                    Console.WriteLine("Accepted client.");
                }
                catch
                {
                    //Error while reading for clients, close
                    Console.WriteLine("ERROR WHILE ACCEPTING CLIENT, CLOSING.");
                    Disable();
                }
            }
        }

        private List<Lobby> lobbies = new List<Lobby>(); 
        /// <summary>
        /// Sorts a client into a new or existing lobby.
        /// </summary>
        /// <param name="client">The client to be sorted.</param>
        /// <returns>Returns the lobby that the client was sorted into.</returns>
        public Lobby LobbySorter(Client client, out ServerInfo serverInfo)
        {
            //Check existing lobbies
            int maxScore = -1;
            Lobby lobbyMaxScore = null;
            for (int i = 0; i < lobbies.Count; i++)
            {
                //Ensure lobby is joinable
                if (lobbies[i].IsJoinable)
                {
                    //Check for compare score
                    int compareScore = client.ClientInfo.LobbyInfo.CompareTo(lobbies[i].HostClient.ClientInfo.LobbyInfo);
                    if (compareScore > maxScore)
                    {
                        //Is better lobby choice
                        lobbyMaxScore = lobbies[i];
                        maxScore = compareScore;
                    }
                }
            }

            //Determine if lobby of minimum requirement has been found
            if (lobbyMaxScore != null && maxScore >= 0 && lobbyMaxScore.AddClient(client))
            {
                //Added to existing lobby
                Console.WriteLine("Client added to existing lobby.");
                serverInfo = new ServerInfo(lobbyMaxScore.LobbySeed, (byte)(lobbyMaxScore.Clients.Length - 1));
                return lobbyMaxScore;
            }
            else
            {
                //Add to new lobby
                Console.WriteLine("Client added to new lobby.");
                Lobby newLobby = new Lobby(client, lobbies, new Client.Print((b) => { Console.WriteLine(b); }), serverRandom);
                lobbies.Add(newLobby);
                serverInfo = new ServerInfo(newLobby.LobbySeed, 0);
                return newLobby;
            }
        }

        public void Disable()
        {
            tcpListener.Stop();
            enabled = false;
        }
    }
}

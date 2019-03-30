using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using FighterGame.Network.Standardization;
using FighterGame.Network.Client;
using FighterGame.Network.Message;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using FighterGame.Runtime;

namespace FighterGame.Network.Server
{
    public class Client
    {
        public delegate Lobby LobbySorter(Client client, out ServerInfo serverInfo);
        public delegate void Print(string message);

        private const int SERVER_UDP_SEND = 7831;
        private const int SERVER_UDP_RECEIVE = 7830;

        private const int SERVER_UDP_SEND_ALT = 8831;
        private const int SERVER_UDP_RECEIVE_ALT = 8830;

        //Object
        public bool IsReady = false;

        private TcpClient tcpClient;
        private StandardizedTCP standardizedTCP;

        private StandardizedUDP standardizedUDP;

        private Print printMethod;

        /// <summary>
        /// Creates a new client.
        /// </summary>
        /// <param name="tcpClient">The tcp client of the client.</param>
        public Client(TcpClient tcpClient, LobbySorter lobbySorter, Print printMethod)
        {
            this.tcpClient = tcpClient;
            this.printMethod = printMethod;

            //Set standard stream
            standardizedTCP = new StandardizedTCP(tcpClient.GetStream());

            //Get standard udp
            IPAddress address = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
            try
            {
                standardizedUDP = new StandardizedUDP(new IPEndPoint(address, SERVER_UDP_RECEIVE), new IPEndPoint(address, SERVER_UDP_SEND));
            }
            catch
            {
                standardizedUDP = new StandardizedUDP(new IPEndPoint(address, SERVER_UDP_RECEIVE_ALT), new IPEndPoint(address, SERVER_UDP_SEND_ALT));
            }

            //Establish client
            EstablishClient(lobbySorter);
        }

        public PeerClientInfo PeerClientInfo => new PeerClientInfo(ClientIndex, ClientInfo.Username);

        private ClientInfo clientInfo;
        public ClientInfo ClientInfo => clientInfo;

        private Lobby lobby = null;
        public Lobby Lobby => lobby;

        public byte ClientIndex;

        private async void EstablishClient(LobbySorter lobbySorter)
        {
            //Wait for client info object
            object receivedObject;
            do
            {
                //Wait for object
                receivedObject = await standardizedTCP.ReadObject();

                //Ensure no error
                if (receivedObject == null)
                {
                    //Error in connection, close
                    Close(true, false);
                    return;
                }
            }
            while (!(receivedObject is ClientInfoMessage));

            //Cast to client info
            clientInfo = ((ClientInfoMessage)receivedObject).ClientInfo;

            //Send client to lobbies
            ServerInfo serverInfo;
            lobby = lobbySorter.Invoke(this, out serverInfo);
            ClientIndex = serverInfo.ClientIndex;

            //Send server information
            SendMessage(new ServerInfoMessage(serverInfo));

            //Send client information
            lobby.ForwardToClients(this, new PeerClientInfoMessage(PeerClientInfo));

            //Request client information
            lobby.SendClientInformation(this);

            //Start client readers
            ClientReaderTCP();
            ClientReaderUDP();
        }

        public async void SendMessage(IClientMessage messageToSend, bool sendTCP = true)
        {
            if (enabled)
            {
                if (sendTCP) await standardizedTCP.SendMessage(messageToSend);
                else await standardizedUDP.SendMessage(messageToSend);
            }
        }

        private bool enabled = true;
        /// <summary>
        /// Asynchronous method that checks for incoming client messages.
        /// </summary>
        private async void ClientReaderTCP()
        {
            if (enabled)
            {
                //Await an object
                object recievedObject = await standardizedTCP.ReadObject();

                //Check if error
                if (recievedObject == null)
                {
                    //Error while reading messages, disconnect
                    Close(true, false);
                    return;
                }
                else
                {
                    //Call new client reader
                    ClientReaderTCP();
                }

                //Parse based on type
                if (recievedObject is IClientMessage)
                {
                    //Forward message to other clients
                    if (enabled) lobby.ForwardToClients(this, (IClientMessage)recievedObject);
                }
                if (recievedObject is IServerMessage)
                {
                    //Run server message
                    if (enabled) ((IServerMessage)recievedObject).ServerAction(lobby, this);
                }
            }
        }

        private async void ClientReaderUDP()
        {
            if (enabled)
            {
                //Await an object
                object recievedObject = await standardizedUDP.ReadObject();

                //Check if error
                if (recievedObject == null)
                {
                    //Error while reading messages, disconnect
                    Close(true, false);
                    return;
                }
                else
                {
                    //Call new client reader
                    ClientReaderUDP();
                }

                //Parse based on type
                if (recievedObject is IClientMessage)
                {
                    //Forward message to other clients
                    if (enabled) lobby.ForwardToClients(this, (IClientMessage)recievedObject, sendTCP: false);
                }
                if (recievedObject is IServerMessage)
                {
                    //Run server message
                    if (enabled) ((IServerMessage)recievedObject).ServerAction(lobby, this);
                }
            }
        }

        /// <summary>
        /// Closes the client connections and removes it from lobbies.
        /// </summary>
        /// <param name="faultyConnection">Value representing the state of client connection.</param>
        public void Close(bool faultyConnection, bool lobbyClose)
        {
            if (enabled)
            {
                //Set status
                enabled = false;

                //Remove from lobby
                if (lobby != null) lobby.RemoveClient(this, lobbyClose);

                //Handle connection
                standardizedTCP.Disable();
                standardizedUDP.Disable();

                //Print message
                printMethod.Invoke("Client closed. Faulty Connection: " + faultyConnection + ", Lobby-Close: " + lobbyClose);
            }
        }
    }
}

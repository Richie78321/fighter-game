using FighterGame.Network.Standardization;
using FighterGame.Network.Client;
using FighterGame.Network.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using FighterGame.Entities;
using FighterGame.Entities.Player;
using ProtoBuf;
using Microsoft.Xna.Framework;

namespace FighterGame.Runtime
{
    public class NetworkManager
    {
        public const int PUBLIC_SERVER_PORT_TCP = 7832;

        public const int CLIENT_UDP_SEND = 7830;
        public const int CLIENT_UDP_RECEIVE = 7831;

        public const int CLIENT_UDP_SEND_ALT = 8830;
        public const int CLIENT_UDP_RECEIVE_ALT = 8831;

        public enum ConnectionType
        {
            Public,
            Private
        }

        //Object
        public ConnectionType CurrentConnectionType = ConnectionType.Public;
        public string PrivateServerIP = "";

        public StandardizedTCP standardizedTCP = null;
        private TcpClient tcpClient;

        public StandardizedUDP standardizedUDP = null;

        private List<PeerClientInfo> peerClientInfos = new List<PeerClientInfo>();
        public PeerClientInfo[] PeerClientInfos => peerClientInfos.ToArray();
        public void AddPeerClientInfo(PeerClientInfo peerClientInfo)
        {
            peerClientInfos.Add(peerClientInfo);
        }
        public PeerClientInfo GetClientInfo(int clientIndex)
        {
            for (int i = 0; i < peerClientInfos.Count; i++) if (peerClientInfos[i].ClientIndex == clientIndex) return peerClientInfos[i];
            return null;
        }

        private GameSession gameSession;

        public static void LoadNetworkContent()
        {
            NetworkEntityData.LoadSerialization();
        }

        public NetworkManager(GameSession gameSession)
        {
            this.gameSession = gameSession;
        }

        public bool Connect(ClientInfo clientInfo)
        {
            tcpClient = new TcpClient();

            //Connect
            IPAddress ipAddress = null;
            if (CurrentConnectionType == ConnectionType.Public)
            {
                ipAddress = IPAddress.Parse(GetPublicIPAddress());
            }
            else if (CurrentConnectionType == ConnectionType.Private)
            {
                if (!IPAddress.TryParse(PrivateServerIP, out ipAddress))
                {
                    return false;
                }
            }

            try
            {
                //Connect tcp
                tcpClient.Connect(ipAddress, PUBLIC_SERVER_PORT_TCP);
            }
            catch
            {
                return false;
            }

            try
            {
                standardizedUDP = new StandardizedUDP(new IPEndPoint(ipAddress, CLIENT_UDP_RECEIVE), new IPEndPoint(ipAddress, CLIENT_UDP_SEND));
            }
            catch
            {
                try
                {
                    standardizedUDP = new StandardizedUDP(new IPEndPoint(ipAddress, CLIENT_UDP_RECEIVE_ALT), new IPEndPoint(ipAddress, CLIENT_UDP_SEND_ALT));
                }
                catch
                {
                    return false;
                }
            }

            standardizedTCP = new StandardizedTCP(tcpClient.GetStream());

            enabled = true;

            //Send client info
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            standardizedTCP.SendMessage(new ClientInfoMessage(clientInfo));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            //Start accepting messages
            AcceptMessage();
            AcceptMessageUDP();

            return true;
        }

        private async void AcceptMessage()
        {
            if (enabled)
            {
                //Await message
                object acceptedObject = await standardizedTCP.ReadObject();

                //Check for error
                if (acceptedObject == null)
                {
                    //Error has occured
                    Disable();
                    return;
                }
                else AcceptMessage();

                if (acceptedObject is IClientMessage)
                {
                    lock (messageLock)
                    {
                        clientMessages.Add((IClientMessage)acceptedObject);
                    }
                }
                else throw new Exception("Message of invalid type received in stream.");
            }
        }

        private async void AcceptMessageUDP()
        {
            if (enabled)
            {
                //Await message
                object acceptedObject = await standardizedUDP.ReadObject();

                //Check for error
                if (acceptedObject == null)
                {
                    //Error has occured
                    Disable();
                    return;
                }
                else AcceptMessageUDP();

                if (acceptedObject is IClientMessage)
                {
                    lock (messageLock)
                    {
                        clientMessages.Add((IClientMessage)acceptedObject);
                    }
                }
                else throw new Exception("Message of invalid type received in stream.");
            }
        }

        public void SendGameData(GameSession gameSession)
        {
            if (gameSession.GameRunning && enabled)
            {
                if (gameSession.gameMap != null)
                {
                    //Send map data
                    Entity[] entities = gameSession.gameMap.Entities;
                    for (int i = 0; i < entities.Length; i++) if (entities[i] is LocalEntity b) b.SendNetworkEntityData(this);
                }
            }
        }

        public void SendNetworkEntityData(NetworkEntityData networkEntityData, bool server = false)
        {
            if (enabled)
            {
                //Encapsulate in message
                if (!server) SendMessage(new NetworkEntityDataMessage(networkEntityData), sendTCP: false);
                else SendMessage(new ServerNetworkEntityDataMessage(networkEntityData), sendTCP: false);
            }
        }

        public async void SendMessage(INetworkMessage networkMessage, bool sendTCP = true)
        {
            if (enabled)
            {
                if (sendTCP)
                {
                    if (!await standardizedTCP.SendMessage(networkMessage))
                    {
                        //Error while sending
                        Disable();
                    }
                }
                else
                {
                    if (!await standardizedUDP.SendMessage(networkMessage))
                    {
                        //Error while sending
                        Disable();
                    }
                }
            }
        }

        private List<IClientMessage> clientMessages = new List<IClientMessage>();
        private object messageLock = new object();
        public void ParseMessages(GameSession gameSession)
        {
            if (enabled)
            {
                lock (messageLock)
                {
                    //Gather messages
                    IClientMessage[] messagesToParse = clientMessages.ToArray();
                    clientMessages.Clear();

                    for (int i = 0; i < messagesToParse.Length; i++)
                    {
                        messagesToParse[i].ClientAction(gameSession);
                    }
                }
            }
        }

        private double lastTime = 0;
        private int clientIndex = -1;
        public int ClientIndex => clientIndex;

        private Random serverRandom;
        public Random ServerRandom => serverRandom;

        public void SetServerInfo(ServerInfo serverInfo)
        {
            //Unpack server information
            serverRandom = new Random(serverInfo.RandomSeed);
            clientIndex = serverInfo.ClientIndex;

            OnServerInfo();
        }

        private void OnServerInfo()
        {
            gameSession.LoadMap(serverRandom);
        }

        private bool enabled = false;
        public bool Enabled => enabled;
        public void Disable()
        {
            if (enabled)
            {
                //Close stream
                enabled = false;
                clientIndex = -1;
                standardizedTCP.Disable();
                standardizedUDP.Disable();
                tcpClient.Close();
                peerClientInfos.Clear();
                lock (messageLock) clientMessages.Clear();
                gameSession.EndCurrentGame(true);
            }
            else gameSession.EndCurrentGame(true);
        }

        public void Update(GameSession gameSession)
        {
            if (enabled)
            {
                //Parse messages
                ParseMessages(gameSession);

                //Send game data
                SendGameData(gameSession);
            }
        }

        //Temp
        public static string GetPublicIPAddress()
        {
            //return "192.168.1.158";
            return "192.168.1.158";
            //return "10.0.0.183";
            //return "108.36.236.169";
        }
    }
}

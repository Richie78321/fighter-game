using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using FighterGame.Network.Client;
using FighterGame.Network.Standardization;

namespace ServerTester
{
    class Program
    {
        static void Main(string[] args)
        {
            //Connect
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse(GetLocalIPAddress()), 7832);
            Console.WriteLine("Client connected. Enter to send client info.");

            //Wait
            Console.ReadLine();

            //Send client info
            StandardizedStream standardizedStream = new StandardizedStream(tcpClient.GetStream());
            standardizedStream.SendObject(new ClientInfo(new LobbyInfo(2, new LobbyAspect[] { })));
            Console.WriteLine("Client info object sent. Enter to disconnect.");

            //Wait
            Console.ReadLine();

            //Disconnect
            tcpClient.GetStream().Close();
            tcpClient.Close();
            Console.WriteLine("Client disconnected. Enter to close.");

            //Wait for close
            Console.ReadLine();
        }

        //Temp
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}

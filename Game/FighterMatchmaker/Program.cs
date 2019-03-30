using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using FighterGame.Runtime;

namespace FighterMatchmaker
{
    class Program
    {
        private const int SERVER_PORT = 7832;
        private static TcpListener tcpListener;
        private static Matchmaker matchmaker;

        static void Main(string[] args)
        {
            //Load networking content
            NetworkManager.LoadNetworkContent();

            bool started = false;
            while (!started)
            {
                //Get IP
                IPAddress address = null;
                while (address == null)
                {
                    Console.WriteLine("Local IPv4 Address?");
                    if (!IPAddress.TryParse(Console.ReadLine(), out address)) Console.WriteLine("Invalid address.");
                }

                //Initialize TCP Listener
                try
                {
                    tcpListener = new TcpListener(address, SERVER_PORT);
                    tcpListener.Start();
                    Console.WriteLine("Started Server.");
                    started = true;
                }
                catch
                {
                    Console.WriteLine("Failed to start the server on IP: " + address.ToString() + "!");
                }
            }

            //Start matchmaker
            matchmaker = new Matchmaker(tcpListener);
            matchmaker.StartClientListener();
            Console.WriteLine("Started Matchmaker.");

            //Start server dialogue
            ServerDialogue();
        }

        private static bool running = true;
        public static void ServerDialogue()
        {
            while (running)
            {
                //Await input
                string input = Console.ReadLine();
            }
        }

        //Temp
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.WriteLine("Attempting on " + ip.ToString());
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}

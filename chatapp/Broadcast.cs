using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace chatapp
{
    class Broadcast
    {
        private const int port = 54545;
        private const string broadcastAddress = "255.255.255.255";
        private IPAddress localIP;
        private User otherUser;

        private UdpClient receivingClient;
        private UdpClient sendingClient;

        private Thread receivingThread;


        public Broadcast(string userName)
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            localIP = ips[1];
            InitializeSender();
            InitializeReceiver(userName);
        }

        private void InitializeSender()
        {
            sendingClient = new UdpClient(broadcastAddress, port);
            sendingClient.EnableBroadcast = true;
        }

        public void send(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            sendingClient.Send(data, data.Length);
        }

        private void InitializeReceiver(string userName)
        {
            receivingClient = new UdpClient(port);
            receivingThread = new Thread(() => Receiver(userName));
            receivingThread.IsBackground = true;
            receivingThread.Start();
        }

        /* Сүлжээгээр ямарч хамаагүй хаягаас broadcast хийн ирсэн мессежийг хүлээн авч,
         * Хэрэв өөрийн username байвал буцаан өөрийнхөө нэр болон ip хаягийг буцаана.
         * Харин бусад үед ирсэн мессежийг задлан нэр, ipхаягийг нь авна.
         */
        private void Receiver(string userName)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            while(true)
            {
                byte[] data = receivingClient.Receive(ref endPoint);
                string message = Encoding.ASCII.GetString(data);
                Console.WriteLine("Message is : " + message);
                if (message.Equals(userName))
                {
                    send(userName + "," + localIP);
                } else
                {
                    string[] parts = message.Split(',');
                    if (parts.Length == 2)
                    {
                        getUserInfo(parts[0], parts[1]);
                    }
                }
            }
        }

        // User-тэй холбоотой функцууд
        private void getUserInfo(string username, string ipString)
        {
            otherUser = new User(username, ipString);
            Console.WriteLine(otherUser.name);
            Console.WriteLine(otherUser.ipString);
        }

        public User getOtherUser()
        {
            return otherUser;
        }
        public void clearUser()
        {
            otherUser = null;
        }
    }
}

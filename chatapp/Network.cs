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
    class Network
    {
        // IPHostEntry - Энэ нь internet host address-н мэдээллийг агуулдаг класс
        private IPHostEntry ipHost;
        private IPAddress ipAddr;
        private IPAddress sendingAddr;
        private IPEndPoint listeningEndPoint;
        private IPEndPoint sendingEndPoint;

        private Socket listener;
        private Socket sender;

        private Thread listeningThread;

        public string message = null;
        private int port = 11110;

        public Network()
        {
            // Host-ынхоо мэдээлийг авна
            ipHost = Dns.GetHostEntry(Dns.GetHostName());

            // Host дотор байх AddressList хүснэгтийн хоёрдох элемент нь Wiriless сүлжээн дэх IPv6 хаяг байна.
            ipAddr = ipHost.AddressList[1];

            // Тухайн ip хаяг болон 11110 гэсэн порт дээр EndPoint үүсгэнэ.
            listeningEndPoint = new IPEndPoint(ipAddr, port);

            InitializeListener();
        }

        private void Listener()
        {
            try
            {
                // Сонсож буй socket-оо listeningEndPoint-тойгоо холбоно.
                listener.Bind(listeningEndPoint);

                // Сонсож буй socket нь хамгийн ихдээ 10 хэрэглэгч холбогдох queue-нд байна
                listener.Listen(10);
                while (true)
                {
                    // Listen хийж байхад ирж буй хүсэлтүүдийн нэгэнтэй нь холбогдоно. (Client-тай холбогдоно.)
                    Socket clientSocket = listener.Accept();

                    // Socket-оор зөвхөн byte мэдээлэл ирэх учир 1024 хэмжээтэй byte хүснэгт үүсгэнэ.
                    byte[] bytes = new byte[1024];
                    string data = null;
                    while (true)
                    {   
                        // Socket-н Receive method-г ашиглан үүсгэсэн хүснэгтэндээ ирж буй мэдээллээ хуулна.
                        // numByte нь хичнээн хэмжээний byte мэдээллэл ирснийг илтгэнэ.
                        int numByte = clientSocket.Receive(bytes);

                        // Хүлээж авсан byte мэдээллээ string-лүү хөрвүүлнэ.
                        data += Encoding.ASCII.GetString(bytes, 0, numByte);

                        // Хэрэв хүлээж авсан мэдээлэлд "<EOF>" гэсэн хэсэг орж ирвэл хүлээж авах процессийг зогсооно.
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }
                    // Дараагаар нь бүх хүлээж авсан мэдээллээ message - рүү хуулна.
                    message = data;
                    // Client-тай үүсгэсэн socket-оо хаана.
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void InitializeListener()
        {
            // Ирж буй мессежийг сонсох socket үүсгэнэ.
            // - ipAddr.AddressFamily - ip хаягийнхаа family-г зааж өгнө. (IPv4, IPv6 аль нь болохыг)
            // - SocketType.Stream - Stream гэж зааж өгснөөр найдвартай, 
            // - ProtocolType.Tcp - TCP протокол ашиглахыг зааж өгнө.
            listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Дээр үүсгэсэн Listener функцээ нэг thread үүсгэн backgroun-д ажилуулна.
            ThreadStart start = new ThreadStart(Listener);
            listeningThread = new Thread(start);
            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

        // Тухайн зааж өгсөн ip хаяг руу message-г явуулна.
        public void SendMessage(string ipString, string message)
        {
            // Параметрээр орж ирсэн ipString-ээс ip хаягийг нь гарган авч
            // message явуулах end point-оо үүсгэнэ.
            sendingAddr = IPAddress.Parse(ipString);
            sendingEndPoint = new IPEndPoint(sendingAddr, port);
            try
            {
                // listener-тэй адил socket үүсгэнэ.
                sender = new Socket(sendingAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // үүсгэсэн socket-оо endpoint-тойгоо холбоно.
                sender.Connect(sendingEndPoint);

                // message-ээ byte-руу хөрвүүлэн илгээнэ.
                byte[] messageSent = Encoding.ASCII.GetBytes(message + "<EOF>");
                int byteSent = sender.Send(messageSent);

                // message-ээ илгээчээд socket-оо хаана.
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}

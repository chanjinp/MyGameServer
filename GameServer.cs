using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace MyGameServer
{
    public class GameServer
    {
        private Socket m_serverSocket;
        private bool isStarted;
        private int port;
        private int tryAcceptCount; //접속 시도
        private List<SocketInfo> m_clientList; //실제 접속했을 때 관리

        public IPAddress GetIPAddress()
        {
            IPHostEntry myHostInfo = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in myHostInfo.AddressList)
            {
                if (AddressFamily.InterNetwork.Equals(ip.AddressFamily)) //IPv4인 경우에만 return 하도록
                {
                    return ip;
                }
            }

            return IPAddress.Parse("127.0.0.1"); //없는 경우 로컬 통신 IP로 사용
        }

        public void Init()
        {
            Console.WriteLine("=========== Init ===========");
            m_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            isStarted = false;
            port = -1;
            tryAcceptCount = 0;
            m_clientList = new List<SocketInfo>();

        }
        public void Bind(int port)
        {
            this.port = port;
            IPEndPoint serverEP = new IPEndPoint(GetIPAddress(), port);

            Console.WriteLine("=========== Server Binding... [Port]: {0} ===========", port);
            m_serverSocket.Bind(serverEP); // 목적지 설정과 함께 설정

            Console.WriteLine("=========== Server Listening ===========");
            m_serverSocket.Listen(10); //동시 대기 최대 10명

            isStarted = true;
        }

        public void Close()
        {
            Console.WriteLine("=========== Server Closing ===========");
            m_serverSocket?.Close();
            m_serverSocket?.Dispose();

            isStarted = false;
        }

        public void AcceptMultiClients()
        {
            while (isStarted)
            {
                if (m_clientList.Count == tryAcceptCount)
                {
                    ThreadStart threadStart = new ThreadStart(AcceptClient);
                    tryAcceptCount++; //접속에 시도를 했으니 증가
                    Thread thread = new Thread(threadStart);
                    thread.Start();
                }
            }
        }
        public void AcceptClient()
        {
            Socket client = null;
            SocketInfo clientInfo = null;

            try
            {
                client = m_serverSocket.Accept(); //서버로부터 받은 소켓 사용
                clientInfo = new SocketInfo(client, true, 1024);

                m_clientList.Add(clientInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AcceptClient Error]: " + ex);
            }

        }

        //TODO 서버를 통한 브로드캐스트 전송 예제 간단히 테스트하기
        public void Send(string message)
        {
            byte[] msg = Encoding.UTF8.GetBytes(message);

            NetworkStream stream = new NetworkStream(m_serverSocket);

            for (int i = 0; i < m_clientList.Count; i++)
            {
                m_clientList[i].GetClientSocket().Send(msg);
            }
        }
    }
}

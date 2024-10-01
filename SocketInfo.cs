using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyGameServer
{
    public class SocketInfo
    {
        private byte[] bytes;
        public bool isConnected = false;
        private Socket m_clientSocket;

        public SocketInfo(Socket client, bool isConnected, int bufferSize)
        {
            m_clientSocket = client;
            this.isConnected = isConnected;
            bytes = new byte[bufferSize];
        }

        public Socket GetClientSocket()
        {
            return m_clientSocket;
        }

        public void ClearBuffer()
        {
            Array.Clear(bytes, 0, bytes.Length);
        }
    }
}

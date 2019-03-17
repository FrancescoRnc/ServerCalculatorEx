using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerCalculatorEx
{
    class TransportIPv4 : ITransport
    {
        Socket socket;

        public TransportIPv4()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //socket.Blocking = false;
        }

        public void Bind(string address, int port)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(address), port);
            socket.Bind(endPoint);
        }

        public EndPoint CreateEndPoint()
        {
            return new IPEndPoint(0, 0);
        }

        public byte[] Receive(int buffersize, ref EndPoint sender)
        {
            byte[] data = new byte[buffersize];
            int rlen = socket.ReceiveFrom(data, ref sender);
            byte[] trueData = new byte[rlen];
            Buffer.BlockCopy(data, 0, trueData, 0, rlen);
            return trueData;
        }

        public bool Send(byte[] data, EndPoint destination)
        {
            socket.SendTo(data, destination);

            return true;
        }
    }
}

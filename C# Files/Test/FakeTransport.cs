using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ServerCalculatorEx.Test
{
    public class FakeQueueEmpty : Exception
    {

    }

    public struct FakeData
    {
        public FakeEndPoint EndPoint;
        public byte[] Data;
    }

    class FakeTransport : ITransport
    {
        FakeEndPoint boundAddress;
        Queue<FakeData> recvQueue;
        Queue<FakeData> sendQueue;


        public FakeTransport()
        {
            recvQueue = new Queue<FakeData>();
            sendQueue = new Queue<FakeData>();
        }


        public void ClientEnqueue(FakeData fakeData)
        {
            recvQueue.Enqueue(fakeData);
        }

        public void ClientEnqueue(byte[] packet, string address, int port)
        {
            recvQueue.Enqueue(new FakeData() { Data = packet, EndPoint = new FakeEndPoint(address, port) });
        }

        public FakeData ClientDequeue()
        {
            if (sendQueue.Count <= 0)
                throw new FakeQueueEmpty();
            return sendQueue.Dequeue();
        }

        public void Bind(string address, int port)
        {
            boundAddress = new FakeEndPoint(address, port);
        }

        public EndPoint CreateEndPoint()
        {
            return new FakeEndPoint();
        }

        public byte[] Receive(int buffersize, ref EndPoint sender)
        {
            if (recvQueue.Count == 0)
                return null;
            byte[] data = new byte[buffersize];
            FakeData fakeData = recvQueue.Dequeue();
            if (fakeData.Data.Length > buffersize)
                return null;

            sender = fakeData.EndPoint;
            return fakeData.Data;
        }

        public bool Send(byte[] data, EndPoint destination)
        {
            FakeData fakeData = new FakeData();
            fakeData.Data = data;
            fakeData.EndPoint = destination as FakeEndPoint;
            sendQueue.Enqueue(fakeData);
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerCalculatorEx
{
    public class Client : IServerComponents
    {
        ITransport transport;

        byte[] dataToSend;

        byte[] lastDataReceived;
        public float LastResultReceived
        { get { return BitConverter.ToSingle(lastDataReceived, 0); } }

        int dataSendedCount;
        public int DataSendedCount { get { return dataSendedCount; } }

        public Client(ITransport transport)
        {
            this.transport = transport;
        }

        public void Run()
        {
            // This file is not minded to be a launchable program,
            // but only for testing

            Console.WriteLine("Client started");
            while (true)
            {
                Step();
            }
        }

        public void Step()
        {
            EndPoint serverEP = transport.CreateEndPoint();
            byte[] resultData = Receive(ref serverEP);
            if (resultData != null)
            {
                lastDataReceived = resultData;

                float result = BitConverter.ToSingle(resultData, 0);
                Console.WriteLine(result);
            }

            if (dataToSend != null)
            {
                Send(dataToSend, serverEP);

                Console.WriteLine("Sended data");
                dataSendedCount++;
                dataToSend = null;
            }
        }

        public byte[] Receive(ref EndPoint sender)
        {
            return transport.Receive(256, ref sender);
        }

        public void Send(byte[] data, EndPoint sender)
        {
            transport.Send(data, sender);
        }

        public void PrepareValuesToSend(uint command, float a, float b)
        {
            dataToSend = Server.TwoValuesPacked(command, a, b);
        }
    }
}

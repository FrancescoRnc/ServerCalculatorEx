using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ServerCalculatorEx
{
    public class Server : IServerComponents
    {
        ITransport transport;

        public int ExpectedPacketLength;


        public Server(ITransport transport)
        {
            this.transport = transport;

            ExpectedPacketLength = 12;
        }


        public void Run()
        {
            Console.WriteLine("Server started!");

            while (true)
            {
                Step();
            }
        }

        public void Step()
        {
            EndPoint sender = transport.CreateEndPoint();
            byte[] data = Receive(ref sender);

            if (data == null || data.Length != ExpectedPacketLength)
                return;

            uint command = data[0];
            float firstValue = BitConverter.ToSingle(data, 4);
            float secondValue = BitConverter.ToSingle(data, 8);
            if (command > 3u)
                return;

            float result = 0;
            if (command == 0)
            {
                result = firstValue + secondValue;
                Console.WriteLine(result);
            }
            else if (command == 1)
            {
                result = firstValue - secondValue;
                Console.WriteLine(result);
            }
            else if (command == 2)
            {
                result = firstValue * secondValue;
                Console.WriteLine(result);
            }
            else if (command == 3)
            {
                result = firstValue / secondValue;
                Console.WriteLine(result);
            }

            byte[] resultData = ResultPacked(result);

            Send(resultData, sender);
        }

        public byte[] Receive(ref EndPoint sender)
        {
            return transport.Receive(256, ref sender);
        }

        public void Send(byte[] data, EndPoint sender)
        {
            transport.Send(data, sender);
        }


        float Sum(float a, float b)
        {
            return a + b;
        }
        float Sub(float a, float b)
        {
            return a - b;
        }
        float Mult(float a, float b)
        {
            return a * b;
        }
        float Div(float a, float b)
        {
            return a / b;
        }

        byte[] ResultPacked(float result)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            
            writer.Write(result);

            return stream.ToArray();
        }

        static public byte[] TwoValuesPacked(uint command, float first, float second)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(command);
            writer.Write(first);
            writer.Write(second);

            return stream.ToArray();
        }
    }
}

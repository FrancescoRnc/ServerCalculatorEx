using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using NUnit.Framework;

namespace ServerCalculatorEx.Test
{
    class TestServer
    {
        Server server;
        FakeTransport serverTransport;

        Client client;
        FakeTransport clientTransport;

        [SetUp]
        public void SetUpTests()
        {
            serverTransport = new FakeTransport();
            serverTransport.Bind("testerClient", 0);
            server = new Server(serverTransport);

            clientTransport = new FakeTransport();
            client = new Client(clientTransport);
        }

        [Test]
        public void TestServerClearStep()
        {
            Assert.That(() => server.Step(), Throws.Nothing);
        }

        [Test]
        public void TestReceivedPacketNotVariated()
        {
            byte[] packet = Server.TwoValuesPacked(0, 3.5f, 7f);
            EndPoint sender = new FakeEndPoint("tester", 0);
            serverTransport.ClientEnqueue(new FakeData() { Data = packet, EndPoint = sender as FakeEndPoint });

            Assert.That(server.Receive(ref sender), Is.EqualTo(packet));
        }

        [Test]
        public void TestSendQueueNotEmpty()
        {
            byte[] packet = Server.TwoValuesPacked(0, 3.5f, 7f);
            EndPoint sender = new FakeEndPoint("tester", 0);
            serverTransport.ClientEnqueue(new FakeData() { Data = packet, EndPoint = sender as FakeEndPoint });

            server.Step();

            Assert.That(() => serverTransport.ClientDequeue(), Throws.Nothing);
        }

        [Test]
        public void TestCorrectPacketUse()
        {
            byte[] packet = Server.TwoValuesPacked(0, 3.5f, 7f);
            EndPoint sender = new FakeEndPoint("tester", 0);
            serverTransport.ClientEnqueue(new FakeData() { Data = packet, EndPoint = sender as FakeEndPoint });

            server.Step();

            FakeData lastPacket = serverTransport.ClientDequeue();
            byte[] result = lastPacket.Data;

            float currResult = BitConverter.ToSingle(result, 0);
            Assert.That(currResult, Is.EqualTo(10.5f));
        }

        [Test]
        public void TestClientClearStep()
        {
            Assert.That(() => client.Step(), Throws.Nothing);
        }

        [Test]
        public void TestClientEmptyQueueAfterFirstStep()
        {
            client.Step();
            Assert.That(() => clientTransport.ClientDequeue(), Throws.InstanceOf<FakeQueueEmpty>());
        }
        [Test]
        public void TestClientNotEmptyQueueAfterFirstStep()
        {
            client.PrepareValuesToSend(2, 5f, 7f);
            client.Step();
            Assert.That(() => clientTransport.ClientDequeue(), Throws.Nothing);
        }

        [Test]
        public void TestClientSendPacket()
        {
            client.PrepareValuesToSend(1, 9f, 5.5f);
            client.Step();

            Assert.That(client.DataSendedCount, Is.EqualTo(1));
        }

        [Test]
        public void TestServerTrueResult()
        {
            client.PrepareValuesToSend(0, 3.5f, 7f);
            client.Step();

            serverTransport.ClientEnqueue(clientTransport.ClientDequeue());
            server.Step();

            FakeData lastPacket = serverTransport.ClientDequeue();
            byte[] result = lastPacket.Data;

            float currResult = BitConverter.ToSingle(result, 0);
            Assert.That(currResult, Is.EqualTo(10.5f));
        }

        [Test]
        public void TestClientServerCheckCommandNotExpected()
        {            
            client.PrepareValuesToSend(9, 3.5f, 7f);
            client.Step();

            serverTransport.ClientEnqueue(clientTransport.ClientDequeue());
            server.Step();

            

            Assert.That(() => clientTransport.ClientDequeue(), Throws.InstanceOf<FakeQueueEmpty>());
        }

        [Test]
        public void TestAdditionCommandPacket()
        {
            client.PrepareValuesToSend(0, 5.4f, 7f);
            client.Step();

            serverTransport.ClientEnqueue(clientTransport.ClientDequeue());
            server.Step();

            clientTransport.ClientEnqueue(serverTransport.ClientDequeue());
            client.Step();
            
            Assert.That(client.LastResultReceived, Is.EqualTo(12.4f).Within(0.0001f));
        }

        [Test]
        public void TestSubtractionCommandPacket()
        {
            client.PrepareValuesToSend(1, 5.4f, 7);
            client.Step();

            serverTransport.ClientEnqueue(clientTransport.ClientDequeue());
            server.Step();

            clientTransport.ClientEnqueue(serverTransport.ClientDequeue());
            client.Step();
            
            Assert.That(client.LastResultReceived, Is.EqualTo(-1.6f).Within(0.0001f));
        }

        [Test]
        public void TestMultiplicationCommandPacket()
        {
            client.PrepareValuesToSend(2, 5.4f, 7f);
            client.Step();

            serverTransport.ClientEnqueue(clientTransport.ClientDequeue());
            server.Step();

            clientTransport.ClientEnqueue(serverTransport.ClientDequeue());
            client.Step();

            Assert.AreEqual(client.LastResultReceived, 37.8f);
        }

        [Test]
        public void TestDivisionCommandPacket()
        {
            client.PrepareValuesToSend(3, 3.5f, 7f);
            client.Step();

            serverTransport.ClientEnqueue(clientTransport.ClientDequeue());
            server.Step();

            clientTransport.ClientEnqueue(serverTransport.ClientDequeue());
            client.Step();
            
            Assert.AreEqual(client.LastResultReceived, 0.5f);
        }
    }
}

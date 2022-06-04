using LOG736_Labo1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Sockets;
using Moq;

namespace LOG736_Labo1_Client
{
    [TestClass]
    public class ClientTest
    {
        public const int PortNumber = 25000;

        [TestInitialize]
        public void InitClientTest()
        {
            
        }

        [TestCleanup]
        public void CleanClientTest()
        {
            
        }

        [TestMethod]
        public void GivenClient_WhenRequestingConnectionToServer_ShouldConnectToServer()
        {
            //Arrange
            var myClient = new MyClient();

            //Act
            myClient.RequestTime(1, 1, 1);
            //Assert
            Assert.IsTrue(myClient.MySocket.IsConnected);
        }

        [TestMethod]
        public void GivenClient_WhenRequestingConnectionToServer_ShouldSetCorrectTime()
        {
            //Arrange
            var myClient = new MyClient();

            //Act
            myClient.RequestTime(1, 1, 1);

            //Assert
            Assert.AreEqual(myClient.time, myClient.GetTime());
        }

        [TestMethod]
        public void GivenClient_WhenRequestingConnectionToServer_ShouldSetCorrectAccuracy()
        {
            //Arrange
            var myClient = new MyClient();

            //Act
            myClient.RequestTime(1, 1, 1);
            var actualTime = myClient.GetTime();
            var clientTime = myClient.time;

            //Assert
            Assert.AreEqual(actualTime, myClient.GetAccuracy() + clientTime);
        }
    }

    public class MyClient : IClient
    {
        public long time;
        public long accuracy;
        static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public MySocket? MySocket;

        public long GetAccuracy()
        {
            return accuracy;
        }

        public long GetTime()
        {
            return (long)(DateTime.Now - epoch).TotalMilliseconds;
        }

        public void RequestTime(int serverPort, long currentTime, int numberOftries)
        {
            MySocket = new MySocket();
            MySocket.Connect();

            if (MySocket.IsConnected)
            {
                ProcessTime(MySocket);
            }
        }

        private void ProcessTime(MySocket client)
        {
            var requestTime = DateTime.Now.Millisecond;
            var serverTime = client.Receive();
            var responseTime = DateTime.Now.Millisecond;
            var actualTime = GetTime();
            var processDelay = responseTime - requestTime;
            var clientTime = serverTime + TimeSpan.FromMilliseconds(processDelay / 2).Milliseconds;
            accuracy = actualTime - clientTime;
            time = clientTime;
        }

        public void SetTime(long newTime)
        {
            time = newTime;
        }
    }

    public class MySocket
    {
        static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public bool IsConnected = false;
        public void Connect()
        {
            IsConnected = true;
        }

        public void Close()
        {
            IsConnected = false;
        }

        public long Receive()
        {
            return (long)(DateTime.Now - epoch).TotalMilliseconds;
        }
    }
}

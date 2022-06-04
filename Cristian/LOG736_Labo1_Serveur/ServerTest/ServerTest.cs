using LOG736_Labo1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ServerTest
{
    [TestClass]
    public class ServerTest
    {
        [TestMethod]
        public void GivenServer_WhenStarting_ShouldConnectAndListen()
        {
            //Arrange
            var myServer = new MyServer();

            //Act
            myServer.StartServer(25000);

            //Assert
            Assert.IsTrue(myServer.IsConnected);
            Assert.IsTrue(myServer.IsListening);
        }

        [TestMethod]
        public void GivenServer_WhenStopping_ShouldDisconnectAndStopListening()
        {
            //Arrange
            var myServer = new MyServer();

            //Act
            myServer.StopServer();

            //Assert
            Assert.IsFalse(myServer.IsConnected);
            Assert.IsFalse(myServer.IsListening);
        }
     }

    public class MyServer : IServeur
    {
        static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public bool IsConnected = false;

        public bool IsListening = false;

        public long GetTime()
        {
            return (long)(DateTime.Now - epoch).TotalMilliseconds;
        }

        public void StartServer(int port)
        {
            Listen();
            if (IsConnected && IsListening)
            {
                GetTime();
            }
        }

        public void StopServer()
        {
            IsListening = false;
            IsConnected = false;
        }

        public void Listen()
        {
            IsConnected = true;
            IsListening = true;
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Linq;
using System.Text;

namespace BerkeleyTest
{
    [TestClass]
    public class ServerTest
    {
        private readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly IService _serviceMock = new ServiceMock();

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), 
        "Leader, and only leader, must be rank 0.")]
        public void StartNoeudPeerBadRankUniteTest()
        {
            Noeud noeud = new Noeud(_serviceMock, false);

            noeud.StartNoeud(1, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), 
        "Leader, and only leader, must be rank 0.")]
        public void StartNoeudLeaderBadRankUniteTest()
        {
            Noeud noeud = new Noeud(_serviceMock, false);

            noeud.StartNoeud(0, false);
        }

        [TestMethod]
        public void RequestTimePeerUniteTest()
        {
            // Arrange
            Noeud noeud = new Noeud(_serviceMock, false);
            LeaderMock leader = (LeaderMock)_serviceMock.GetLeader(0);
            leader.rank1TimeMock = NowTotalMilliseconds() - 10;

            // Act
            long[] times = noeud.RequestTime(leader);

            // Assert
            Assert.IsTrue(times[1] == NowTotalMilliseconds() - 10);
        }

        [TestMethod]
        public void RequestTimeLeaderTimeUniteTest()
        {
            // Arrange
            Noeud noeud = new Noeud(_serviceMock, false);
            LeaderMock leader = (LeaderMock)_serviceMock.GetLeader(0);

            // Act
            long[] times = noeud.RequestTime(leader);

            // Assert
            Assert.IsTrue(times[0] == NowTotalMilliseconds());
        }

        [TestMethod]
        public void GetTimeUniteTest()
        {
            // Arrange
            Noeud noeud = new Noeud(_serviceMock, false);

            // Act
            long time = noeud.GetTime();

            // Assert
            Assert.IsTrue(time == NowTotalMilliseconds());
        }

        [TestMethod]
        public void SetTimePostiveOffesetUniteTest()
        {
            // Arrange
            Noeud noeud = new Noeud(_serviceMock, false);

            // Act
            noeud.SetTime(10); // set offest of 10 ms

            // Assert
            Assert.IsTrue(noeud.GetTime() == NowTotalMilliseconds() + 10);
        }

        [TestMethod]
        public void SetTimeNegativeOffesetUniteTest()
        {
            // Arrange
            Noeud noeud = new Noeud(_serviceMock, false);

            // Act
            noeud.SetTime(-100); // set offest of 10 ms

            // Assert
            Assert.IsTrue(noeud.GetTime() == NowTotalMilliseconds() - 100);
        }

        [TestMethod]
        public void SetTimeZeroOffesetUniteTest()
        {
            // Arrange
            Noeud noeud = new Noeud(_serviceMock, false);

            // Act
            noeud.SetTime(0); // set offest of 10 ms

            // Assert
            Assert.IsTrue(noeud.GetTime() == NowTotalMilliseconds());
        }

        private long NowTotalMilliseconds()
        {
            return (long)(_serviceMock.UtcNow() - _epoch).TotalMilliseconds;
        }
    }

    public class ServiceMock : IService
    {
        public DateTime UtcNow()
        {
            return new DateTime(2022, 06, 01);
        }

        public IPeer GetPeer()
        {
            return new PeerMock();
        }

        public ILeader GetLeader(int comSize)
        {
            return new LeaderMock(comSize);
        }
    }

    public class LeaderMock : ILeader
    {
        public long rank1TimeMock;

        public LeaderMock(int comSize)  { }

        public void ConnectSockets(IPAddress ipAddress, Func<int, int> portRankFonction) { }
 
        public void Send(long[] data) { }

        public void Send(byte data) { }

        public void CloseConnections(IPAddress ipAddress, Func<int, int> portRankFonction) { }

        public void Receive(long[] readBuffer)
        {
            readBuffer[1] = rank1TimeMock;
        }
    }

    public class PeerMock : IPeer
    {
        public void OpenConnection(IPAddress ipAddress, int port) { }

        public void AcceptConnection() { }

        public int Receive(byte[] readBuffer)
        {
            return 1;
        }

        public void Close() { }


        public void Send(long time) { }
    }

}
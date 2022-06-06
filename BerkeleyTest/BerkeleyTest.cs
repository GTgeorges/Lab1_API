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
        private readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly IService serviceMock = new ServiceMock();

        // [TestMethod]
        // public void RequestTimeUniteTest()
        // {
        //     // Arrange
        //     Noeud noeud = new Noeud(serviceMock, false);

        //     Mock<IPAddress> ipAddress = new Mock<IPAddress>();
        //     Mock<IPAddress> ipAddress = new Mock<IPAddress>();
        //     // Act
        //     long time = noeud.RequestTime(ipAddress,  null);

        //     // Assert
        //     Assert.IsTrue(time == NowTotalMilliseconds());
        // }

        [TestMethod]
        public void GetTimeUniteTest()
        {
            // Arrange
            Noeud noeud = new Noeud(serviceMock, false);

            // Act
            long time = noeud.GetTime();

            // Assert
            Assert.IsTrue(time == NowTotalMilliseconds());
        }

        [TestMethod]
        public void SetTimePostiveOffesetUniteTest()
        {
            // Arrange
            Noeud noeud = new Noeud(serviceMock, false);

            // Act
            noeud.SetTime(10); // set offest of 10 ms

            // Assert
            Assert.IsTrue(noeud.GetTime() == NowTotalMilliseconds() + 10);
        }

        [TestMethod]
        public void SetTimeNegativeOffesetUniteTest()
        {
            // Arrange
            Noeud noeud = new Noeud(serviceMock, false);

            // Act
            noeud.SetTime(-100); // set offest of 10 ms

            // Assert
            Assert.IsTrue(noeud.GetTime() == NowTotalMilliseconds() - 100);
        }

        [TestMethod]
        public void SetTimeZeroOffesetUniteTest()
        {
            // Arrange
            Noeud noeud = new Noeud(serviceMock, false);

            // Act
            noeud.SetTime(0); // set offest of 10 ms

            // Assert
            Assert.IsTrue(noeud.GetTime() == NowTotalMilliseconds());
        }

        private long NowTotalMilliseconds()
        {
            return (long)(serviceMock.UtcNow() - epoch).TotalMilliseconds;
        }
    }

    public class ServiceMock : IService
    {
        public ServiceMock()
        {

        }

        public DateTime UtcNow()
        {
            return new DateTime(2022, 06, 01);
        }
    }
}
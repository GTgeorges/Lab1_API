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

        [TestMethod]
        public void SetTimeTest()
        {
            // Arrange
            Noeud noeud = new Noeud(serviceMock, false);

            // Act
            noeud.SetTime(10); // set offest of 10 ms

            // Assert
            Assert.IsTrue(noeud.GetTime() == NowTotalMilliseconds() + 10);
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
            return new DateTime(2022,06,01);
        }
    }
}
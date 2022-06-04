using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LOG736_Labo1
{
    public class Client : IClient
    {
        private long time;

        private long accuracy;

        static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public long GetAccuracy()
        {
            return accuracy;
        }

        public long GetTime()
        {
            return (long)(DateTime.UtcNow - epoch).TotalMilliseconds;
        }

        public void RequestTime(int serverPort, long currentTime, int numberOftries)
        {
            var host = Dns.GetHostEntry("localhost");
            var ipAddress = host.AddressList[0];
            var endpoint = new IPEndPoint(ipAddress, serverPort);
            var counter = 0;

            /*Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);*/
            try
            {
                while (counter < numberOftries)
                {
                    Console.WriteLine("Synchronisation #{0}", counter++);
                    Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    //connexion au server d'horloge sur l'ordinateur local
                    client.Connect(endpoint);
                    Console.WriteLine("Client connecté à: {0}", client?.RemoteEndPoint?.ToString());

                    var requestTime = DateTime.Now.Millisecond;

                    //recevoir donnees du serveur
                    byte[] messagedReceived = new byte[1024];
                    int byteRecv = client.Receive(messagedReceived);
                    var serverTime = Convert.ToInt64(Encoding.ASCII.GetString(messagedReceived, 0, byteRecv));
                    var responseTime = DateTime.Now.Millisecond;
                    //var actualTime = (long)DateTime.Now.Millisecond;
                    var actualTime = GetTime();

                    Console.WriteLine("Temps d'horloge retourné par le serveur: {0}", serverTime);

                    var processDelay = responseTime - requestTime;

                    Console.WriteLine("Délai de traitement: {0} millisecondes", processDelay);

                    var clientTime = serverTime + TimeSpan.FromMilliseconds(processDelay / 2).Milliseconds;
                    Console.WriteLine("Horloge synchronisé du client: {0}", clientTime);
                    accuracy = actualTime - clientTime;
                    Console.WriteLine("Erreur de synchronisation: {0} millisecondes", accuracy);
                    SetTime(clientTime);
                    client.Close();
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SetTime(long newTime)
        {
            time = newTime;
        }
    }
}

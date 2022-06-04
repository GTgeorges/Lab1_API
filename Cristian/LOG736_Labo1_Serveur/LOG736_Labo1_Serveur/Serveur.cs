using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LOG736_Labo1
{
    public class Serveur : IServeur
    {
        //private string receivedMessage;
        //private Socket listener;
        private List<Socket> sockets;

        static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public Serveur()
        {
            //listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sockets = new List<Socket>();
        }
        public long GetTime()
        {
            long ms = (long)(DateTime.UtcNow - epoch).TotalMilliseconds;
            return ms;
        }

        public void StartServer(int port)
        {
            var host = Dns.GetHostEntry("localhost");
            var ipAddress = host.AddressList[0];
            var endpoint = new IPEndPoint(ipAddress, port);
            Socket connection;

            try
            {
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sockets.Add(listener);
                listener.Bind(endpoint);
                listener.Listen();
                Console.WriteLine("En attente de connexion..."); //socket is listening

                while (true)
                {
                    connection = listener.Accept();
                    sockets.Add(connection);
                    
                    Console.WriteLine("Serveur connecté à {0}.", (connection.LocalEndPoint as IPEndPoint)?.Address);
                    connection.Send(Encoding.ASCII.GetBytes(GetTime().ToString()));
                    connection.Close();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            StopServer();
        }

        public void StopServer()
        {
            sockets.ForEach(x => x.Close());
        }
    }
}

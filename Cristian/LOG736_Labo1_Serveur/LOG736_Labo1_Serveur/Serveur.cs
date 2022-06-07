using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LOG736_Labo1
{
	public class Serveur : IServeur
	{
		private List<Socket> sockets;

		private DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public Serveur()
		{
			sockets = new List<Socket>();
		}
		public long GetTime()
		{
			long ms = (long)(DateTime.UtcNow - epoch).TotalMilliseconds;
			return ms;
		}

		public void StartServer(int port)
		{
			epoch = epoch.AddMilliseconds((new Random()).Next(-100, 100));
			var host = Dns.GetHostEntry("localhost");
			var ipAddress = host.AddressList[0];
			var endpoint = new IPEndPoint(ipAddress, port);
			Socket connection;
			Random random = new Random();

			try
			{
				//Creation du socket serveur et mise en écoute pour les connexions du client
				Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				sockets.Add(listener);
				listener.Bind(endpoint);
				listener.Listen();
				Console.WriteLine("En attente de connexion...");

				while (true)
				{
					//nouvelle connexion au serveur
					connection = listener.Accept();
					sockets.Add(connection);

					Console.WriteLine("Serveur connecté à {0}.", (connection.LocalEndPoint as IPEndPoint)?.Address);
					Console.WriteLine("Appuyez sur Enter pour continuer ou S pour arreter.");
					Thread.Sleep(random.Next(50));
					connection.Send(Encoding.ASCII.GetBytes(GetTime().ToString()));
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			StopServer();
		}

		public void StopServer()
		{
			Console.WriteLine("Arret du serveur...");
			sockets.ForEach(x => x.Close());
		}
	}
}

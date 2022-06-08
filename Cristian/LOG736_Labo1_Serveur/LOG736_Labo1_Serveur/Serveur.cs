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

		public void StartServer(int port, int numberOftries)
		{
			//epoch = epoch.AddMilliseconds((new Random()).Next(-100, 100));
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

				int counter = 0;
				while (counter < numberOftries)
				{
					Thread.Sleep(random.Next(200));

					//nouvelle connexion au serveur
					connection = listener.Accept();
					sockets.Add(connection);

					long serverTime = GetTime();
					Console.WriteLine("Serveur connecté à {0}.", (connection.LocalEndPoint as IPEndPoint)?.Address);
					connection.Send(Encoding.ASCII.GetBytes(serverTime.ToString()));
					counter++;
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
			sockets.ForEach(x =>
			{
				x.Close();
			});
		}
	}
}

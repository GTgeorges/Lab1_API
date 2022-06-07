using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LOG736_Labo1
{
	public class Client : IClient
	{
		private long time;

		private long erreur;

		private DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public long GetAccuracy()
		{
			return erreur;
		}

		public long GetTime()
		{
			return (long)(DateTime.UtcNow - epoch).TotalMilliseconds;
		}

		public void RequestTime(int serverPort, long currentTime, int numberOftries)
		{
			epoch = epoch.AddMilliseconds((new Random()).Next(-100, 100));
			var host = Dns.GetHostEntry("localhost");
			var ipAddress = host.AddressList[0];
			var endpoint = new IPEndPoint(ipAddress, serverPort);
			var counter = 0;
			Console.WriteLine("Heure courante du client: {0}\n", currentTime);

			try
			{
				while (counter < numberOftries)
				{
					//Connexion au server d'horloge sur l'ordinateur local
					Console.WriteLine("Synchronisation #{0}", ++counter);
					Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
					client.Connect(endpoint);
					Console.WriteLine("Client connecté à: {0}", client?.RemoteEndPoint?.ToString());

					//Reception des donnnes du serveur
					var requestTime = DateTime.Now;
					byte[] messagedReceived = new byte[1024];
					int byteRecv = client.Receive(messagedReceived);
					var serverTime = Convert.ToInt64(Encoding.ASCII.GetString(messagedReceived, 0, byteRecv));
					var responseTime = DateTime.Now;
					var realTime = GetTime();
					Console.WriteLine("Temps d'horloge retourné par le serveur: {0}", serverTime);

					//Calcul du delais de traitement
					var delay = (long)(responseTime - requestTime).TotalMilliseconds;
					Console.WriteLine("Délai de traitement: {0} millisecondes", delay);

					Console.WriteLine("Heure d'horloge reelle du client: {0}", realTime);

					//Ajustement de l'horloge du client
					var clientTime = serverTime + TimeSpan.FromMilliseconds(delay / 2).Milliseconds;
					Console.WriteLine("Horloge synchronisé du client: {0}", clientTime);

					//Calcul de l'erreur
					erreur = realTime - clientTime;
					Console.WriteLine("Erreur de synchronisation: {0} millisecondes", erreur);
					SetTime(erreur);
					client.Close();
					Console.WriteLine();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		public void SetTime(long offset)
		{
			epoch = epoch.AddMilliseconds(offset);
		}
	}
}

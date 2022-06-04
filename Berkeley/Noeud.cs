using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;

public class Noeud : INoeud
{

	private DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	const int _numberOfPeer = 3;

	const int _basePort = 25000;

	public void StartNoeud(int n, bool isLeader)
	{
		epoch = epoch.AddMilliseconds((new Random()).Next(-100, 100));

		IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
		IPAddress ipAddress = ipHostInfo.AddressList[0];


		if (!isLeader)
		{
			IPEndPoint EndPoint = new IPEndPoint(ipAddress, _basePort + n * 100);
			Socket s = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			s.Bind(EndPoint);
			s.Listen();

			while (true)
			{
				// Program is suspended while waiting for an incoming connection.  
				Socket handler = s.Accept();

				// Incoming connection from leader needs to be processed. 
				byte[] bytes = new Byte[1];

				bool needToExit = false;
				while (true)
				{
					int bytesRec = handler.Receive(bytes);
					if (bytes[0] == 1)
						break;

					else if (bytes[0] == 2)
					{
						handler.Shutdown(SocketShutdown.Both);
						handler.Close();
						needToExit = true;
						break;
					}
				}
				if (needToExit)
					break;


				// Thread.Sleep(1000);

				long time = GetTime();

				// Print the time of the node
				Console.WriteLine($"[{n}] My time is {time}");

				// Send local time to leader
				handler.Send(BitConverter.GetBytes(time));

				handler.Shutdown(SocketShutdown.Both);
				handler.Close();
			}
		}
		else
		{
			while (true)
			{
				// Get time from other nodes 
				ConsoleKey key = Console.ReadKey().Key;
				if (key == ConsoleKey.Enter)
				{
					long[] times = RequestTime(ipAddress);

					long mean = times.Sum() / times.Length;

					// Some print to see what's going on
					for (int i = 0; i < _numberOfPeer; i++)
						Console.WriteLine("noeud" + (int)(i + 1) + ": " + times[i + 1]);

					Console.WriteLine("leader: " + times[0]);
					Console.WriteLine("Moyenne: " + mean);
				}
				else if (key == ConsoleKey.X)
				{
					CloseConnections(ipAddress);
					break;
				}
			}
		}
	}

	public void CloseConnections(IPAddress ipAddress)
	{
		Socket[] noeuds = new Socket[_numberOfPeer];
		for (int i = 0; i < _numberOfPeer; i++)
		{
			noeuds[i] = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			noeuds[i].Connect(new IPEndPoint(ipAddress, _basePort + (i + 1) * 100));
		}

		// Send byte = 2 to indicate that the leader wants the time from each node
		for (int i = 0; i < _numberOfPeer; i++)
			noeuds[i].Send(new byte[] { 2 });
	}

	public long[] RequestTime(IPAddress ipAddress)
	{
		// Makes Sockets for the different endpoints of the different nodes and Connects
		Socket[] noeuds = new Socket[_numberOfPeer];
		for (int i = 0; i < _numberOfPeer; i++)
		{
			noeuds[i] = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			noeuds[i].Connect(new IPEndPoint(ipAddress, _basePort + (i + 1) * 100));
		}

		long[] times = new long[_numberOfPeer + 1];

		// Get time leader
		times[0] = GetTime();

		// Send byte = 1 to indicate that the leader wants the time from each node
		for (int i = 0; i < _numberOfPeer; i++)
			noeuds[i].Send(new byte[] { 1 });

		// Receive the different time values for each node
		for (int i = 0; i < _numberOfPeer; i++)
		{
			byte[] bytes = new Byte[sizeof(long)];
			noeuds[i].Receive(bytes);
			times[i + 1] = BitConverter.ToInt64(bytes);
		}

		return times;
	}


	public long GetTime()
	{
		long ms = (long)(DateTime.UtcNow - epoch).TotalMilliseconds;
		return ms;
	}

	public void SetTime(long offset)
	{
		epoch = epoch.AddMilliseconds(offset);
	}
}

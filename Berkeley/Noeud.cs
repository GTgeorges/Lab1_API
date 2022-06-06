using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;

public class Noeud : INoeud
{
	public const int _numberOfPeer = 3;
	public const int _basePort = 25000;

	private readonly IService _service;

	// Time zero for the node
	private DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	private int rank;
	private IPAddress ipAddress;

	public Noeud(IService service, bool randomOffset=true)
	{
		_service = service;
		this.ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0]; // Get localhost ipAdress
		if (randomOffset)
			epoch = epoch.AddMilliseconds((new Random()).Next(-100, 100));

	}

	public void StartNoeud(int rank, bool isLeader)
	{
		this.rank = rank;
		
		if (!isLeader)
		{
            StartPeer();
		}
		else
		{
			StartLeader();
		}
	}

	private void StartPeer()
	{
		IPEndPoint endPoint = new IPEndPoint(ipAddress, GetPort(rank));
		Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

		socket.Bind(endPoint);
		socket.Listen();

		while (true)
		{
			// Program is suspended while waiting for an incoming connection.  
			Socket handler = socket.Accept();

			// Incoming connection from leader needs to be processed. 
			byte[] bytes = new Byte[sizeof(long)];
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

			// Send local time to leader
			long time = GetTime();
			handler.Send(BitConverter.GetBytes(time));

			while (true)
			{
				int bytesRec = handler.Receive(bytes);
				long offset = -BitConverter.ToInt64(bytes);

				// Print the time of the node before the correction
				time = GetTime();
				Console.WriteLine($"[{this.rank}] My time is now {time}.");

				SetTime(offset);

				time = GetTime();

				// Print the time of the node after changing the time
				Console.WriteLine($"[{this.rank}] My time is now {time}. Correction of ({offset}).");

				break;
			}

			// Close connection
			handler.Shutdown(SocketShutdown.Both);
			handler.Close();
		}
	}

	private void StartLeader()
	{
		while (true)
		{
			// Get time from other nodes 
			ConsoleKey key = Console.ReadKey().Key;
			if (key == ConsoleKey.Enter)
			{
				Console.WriteLine();
				Socket[] noeuds = new Socket[_numberOfPeer];
				long[] times = RequestTime(noeuds);

				long mean = times.Sum() / times.Length;

				// Print the time of the node when requesting time and the mean
				// Console.WriteLine($"[{n}] My time is {times[0]}");
				// Console.WriteLine("Moyenne: " + mean);

				SendOffset(noeuds, times, mean);

				// Print the time of the node before the correction
				long time = GetTime();
				Console.WriteLine($"[{this.rank}] My time is now {time}.");

				// Adjust leader's time with its offset value
				SetTime(mean - times[0]);

				// Print the time of the node after changing the time
				time = GetTime();
				Console.WriteLine($"[{this.rank}] My time is now {time}. Correction of ({mean - times[0]}).");
			}
			else if (key == ConsoleKey.X)
			{
				CloseConnections();
				break;
			}
		}
	}

	private void SendOffset(Socket[] noeuds, long[] times, long mean)
	{
		// Calculate the offset for each node
		long[] offsets = new long[_numberOfPeer + 1];
		for (int i = 0; i < _numberOfPeer + 1; i++)
			offsets[i] = -(mean - times[i]);

		// Send the offset value to each node. First offset is the leader's so we skip it 
		for (int i = 0; i < _numberOfPeer; i++)
		{
			byte[] offset = BitConverter.GetBytes(offsets[i + 1]);
			noeuds[i].Send(offset);
		}
	}

	public void CloseConnections()
	{
		Socket[] noeuds = new Socket[_numberOfPeer];
		ConnectSockets(noeuds);

		// Send byte = 2 to indicate that the leader wants the time from each node
		for (int i = 0; i < _numberOfPeer; i++)
			noeuds[i].Send(new byte[] { 2 });
	}

	public long[] RequestTime(Socket[] noeuds)
	{
		ConnectSockets(noeuds);

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

	/* Makes Sockets for the different endpoints of the different nodes and Connect */
	private void ConnectSockets(Socket[] noeuds)
	{
		for (int i = 0; i < _numberOfPeer; i++)
		{
			noeuds[i] = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			noeuds[i].Connect(new IPEndPoint(ipAddress, GetPort(i + 1)));
		}
	}

	/* Get the current local time */
	public long GetTime()
	{
		return (long)(_service.UtcNow() - epoch).TotalMilliseconds;
	}

	/* Set a new time from an offset value */
	public void SetTime(long offset)
	{
		epoch = epoch.AddMilliseconds(-offset);
	}

	private static int GetPort(int rank)
	{
		return _basePort + (rank + 1) * 100;
	}
}

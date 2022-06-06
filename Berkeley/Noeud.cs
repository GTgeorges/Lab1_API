using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;

public class Noeud
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
		
		if (isLeader)
		{
            StartLeader();
		}
		else
		{
			StartPeer();
		}
	}

	private void StartPeer()
	{
		IPeer peer = _service.GetPeer();
		peer.OpenConnection(ipAddress, GetPort(rank));

		while (true)
		{
			// Program is suspended while waiting for an incoming connection.  
			peer.AcceptConnection();

			// Incoming connection from leader needs to be processed. 
			byte[] bytes = new Byte[sizeof(long)];
			bool needToExit = false;
			while (true)
			{
				peer.Receive(bytes);
				if (bytes[0] == 1)
					break;
				else if (bytes[0] == 2)
				{
					peer.Close();
					needToExit = true;
					break;
				}
			}
			if (needToExit)
				break;

			// Send local time to leader
			long time = GetTime();
			peer.Send(time);

			peer.Receive(bytes);
			long offset = BitConverter.ToInt64(bytes);

			// Print the time of the node before the correction
			time = GetTime();
			Console.WriteLine($"[{this.rank}] My time is now {time}.");

			SetTime(-offset);

			time = GetTime();

			// Print the time of the node after changing the time
			Console.WriteLine($"[{this.rank}] My time is now {time}. Correction of ({-offset}).");

			// Close connection
			peer.Close();
		}
	}

	private void StartLeader()
	{
		ILeader leader = _service.GetLeader(_numberOfPeer);
		while (true)
		{
			// Get time from other nodes 
			ConsoleKey key = Console.ReadKey().Key;
			if (key == ConsoleKey.Enter)
			{
				Console.WriteLine();
				long[] times = RequestTime(leader);

				long mean = times.Sum() / times.Length;

				SendOffset(leader, times, mean);

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
				leader.CloseConnections(ipAddress, GetPort);
				break;
			}
		}
	}

	private void SendOffset(ILeader leader, long[] times, long mean)
	{
		// Calculate the offset for each node
		long[] offsets = new long[_numberOfPeer + 1];
		for (int i = 0; i < _numberOfPeer + 1; i++)
			offsets[i] = -(mean - times[i]);

		// Send the offset value to each node. First offset is the leader's so we skip it 
		leader.Send(offsets);
	}

	public long[] RequestTime(ILeader leader)
	{
		/* Makes Sockets for the different endpoints of the different nodes and Connect */
		leader.ConnectSockets(ipAddress, GetPort);

		long[] times = new long[_numberOfPeer + 1];

		// Get time leader
		times[0] = GetTime();

		// Send byte = 1 to indicate that the leader wants the time from each node
		leader.Send(1);

		// Receive the different time values for each node
		leader.Receive(times);

		return times;
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

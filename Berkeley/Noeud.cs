using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;

public class Noeud : INoeud
{
	const int _numberOfPeer = 3;
	const int _basePort = 25000;

	// Is time zero for the node
	private DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	// Is the rank of the node
	private int n;

	public void StartNoeud(int n, bool isLeader)
	{
		this.n = n;
		epoch = epoch.AddMilliseconds((new Random()).Next(-100, 100));

		// Setup ipAdress
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
					long offset = BitConverter.ToInt64(bytes);

					// Print the time of the node before the correction
					time = GetTime();
					Console.WriteLine($"[{n}] My time is now {time}.");

					SetTime(offset);

					time = GetTime();

					// Print the time of the node after changing the time
					Console.WriteLine($"[{n}] My time is now {time}. Correction of ({-offset}).");

					break;
				}

				// Close connection
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
					Console.WriteLine();
					Socket[] noeuds = new Socket[_numberOfPeer];
					long[] times = RequestTime(ipAddress, noeuds);

					long mean = times.Sum() / times.Length;

					// Print the time of the node when requesting time and the mean
					// Console.WriteLine($"[{n}] My time is {times[0]}");
					// Console.WriteLine("Moyenne: " + mean);

					SendOffset(noeuds, times, mean);

					// Print the time of the node before the correction
					long time = GetTime();
					Console.WriteLine($"[{n}] My time is now {time}.");

					// Adjust leader's time with its offset value
					SetTime(-(mean - times[0]));

					// Print the time of the node after changing the time
					time = GetTime();
					Console.WriteLine($"[{n}] My time is now {time}. Correction of ({mean - times[0]}).");
				}
				else if (key == ConsoleKey.X)
				{
					CloseConnections(ipAddress);
					break;
				}
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

	public void CloseConnections(IPAddress ipAddress)
	{
		Socket[] noeuds = new Socket[_numberOfPeer];
		ConnectSockets(ipAddress, noeuds);

		// Send byte = 2 to indicate that the leader wants the time from each node
		for (int i = 0; i < _numberOfPeer; i++)
			noeuds[i].Send(new byte[] { 2 });
	}

	public long[] RequestTime(IPAddress ipAddress, Socket[] noeuds)
	{
		ConnectSockets(ipAddress, noeuds);

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
	private void ConnectSockets(IPAddress ipAddress, Socket[] noeuds)
	{
		for (int i = 0; i < _numberOfPeer; i++)
		{
			noeuds[i] = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			noeuds[i].Connect(new IPEndPoint(ipAddress, _basePort + (i + 1) * 100));
		}
	}

	/* Get the current time */
	public long GetTime()
	{
		return (long)(DateTime.UtcNow - epoch).TotalMilliseconds;
	}

	/* Set a new time from an offset value */
	public void SetTime(long offset)
	{
		epoch = epoch.AddMilliseconds(offset);
	}
}

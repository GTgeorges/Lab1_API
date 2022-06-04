using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;

public class Noeud : INoeud
{

	static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	const int _numberOfPeer = 1;

	const int _basePort = 25000;

	public void StartNoeud(int n, bool isLeader)
	{
		try
		{
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress ipAddress = ipHostInfo.AddressList[0];


			if (!isLeader)
			{
				IPEndPoint EndPoint = new IPEndPoint(ipAddress, _basePort + n * 100);
				Socket s = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

				s.Bind(EndPoint);
				s.Listen();

				// Program is suspended while waiting for an incoming connection.  
				Console.WriteLine("Waiting for a connection...");
				Socket handler = s.Accept();

				// Incoming connection from leader needs to be processed. 
				byte[] bytes = new Byte[1];
				while (true)
				{
					int bytesRec = handler.Receive(bytes);
					if (bytes[0] == 1)
					{
						break;
					}
				}

				Thread.Sleep(1000);

				long time = GetTime();

				// Print the time of the node
				Console.WriteLine("My time is " + time);

				// Send local time to leader
				handler.Send(BitConverter.GetBytes(time));
				handler.Shutdown(SocketShutdown.Both);
				handler.Close();
			}
			else
			{
				// Get time leader
				long time = GetTime();

				// Get time from other nodes 
				long[] times = RequestTime(ipAddress);

				long mean = (times.Sum() + time) / (_numberOfPeer + 1);

				// Some print to see what's going on
				for (int i = 0; i < _numberOfPeer; i++)
					Console.WriteLine("noeud" + (int)(i + 1) + ": " + times[i]);

				Console.WriteLine("leader: " + time);
				Console.WriteLine("Moyenne: " + mean);
			}

		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}
	}

	public long[] RequestTime(IPAddress ipAddress)
	{
		// Makes Sockets for the different endpoints of the different nodes
		Socket[] noeuds = new Socket[_numberOfPeer];
		for (int i = 0; i < _numberOfPeer; i++)
			noeuds[i] = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

		// Connect to each node
		for (int i = 0; i < _numberOfPeer; i++)
			noeuds[i].Connect(new IPEndPoint(ipAddress, _basePort + (i + 1) * 100));

		// Send one byte to indicate that the leader wants the time from each node
		for (int i = 0; i < _numberOfPeer; i++)
			noeuds[i].Send(new byte[] { 1 });

		long[] times = new long[_numberOfPeer];

		// Receive the different time values for each node
		for (int i = 0; i < _numberOfPeer; i++)
		{
			byte[] bytes = new Byte[sizeof(long)];
			noeuds[i].Receive(bytes);
			times[i] = BitConverter.ToInt64(bytes);
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
		// time += offset;
	}
}

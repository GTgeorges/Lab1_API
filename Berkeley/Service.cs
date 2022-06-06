using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;

public interface IService
{
	public DateTime UtcNow();
}


public class Service : IService
{
	public Service()
	{

	}

	public DateTime UtcNow()
	{
		return DateTime.UtcNow;
	}
}

public class Peer
{
	private Socket handler;
	private Socket socket;

	public Peer()
	{

	}

	public void OpenPeerConnection(IPAddress ipAddress, int port)
	{
		socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
		socket.Bind(new IPEndPoint(ipAddress, port));
		socket.Listen();
	}

	public void AcceptConnexion()
	{
		handler = socket.Accept();
	}

	public int Receive(byte[] readBuffer)
	{
		return handler.Receive(readBuffer);
	}

	public void Close()
	{
		handler.Shutdown(SocketShutdown.Both);
		handler.Close();
	}

	public void Send(long time)
	{
		handler.Send(BitConverter.GetBytes(time));
	}
}

public class Leader
{
	// Rank 0 is set to be leader

	private int comSize;
	Socket[] noeuds; 

	public Leader(int comSize)
	{
		this.comSize = comSize;
		noeuds = new Socket[comSize];
	}

	/* Makes Sockets for the different endpoints of the different nodes and Connect */
	public void ConnectSockets(IPAddress ipAddress, Func<int, int> portRankFonction)
	{
		for (int i = 0; i < comSize; i++)
		{
			noeuds[i] = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			noeuds[i].Connect(new IPEndPoint(ipAddress, portRankFonction(i + 1)));
		}
	}

	// Send the offset value to each node. First offset is the leader's so we skip it 
	public void Send(long[] data)
	{
		for (int i = 0; i < comSize; i++)
		{
			byte[] offset = BitConverter.GetBytes(data[i + 1]);
			noeuds[i].Send(offset);
		}
	}

	public void Send(byte data)
	{
		for (int i = 0; i < comSize; i++)
		{
			noeuds[i].Send(new byte[] { data });
		}
	}

	public void CloseConnections(IPAddress ipAddress, Func<int, int> portRankFonction)
	{
		ConnectSockets(ipAddress, portRankFonction);
		// Send byte = 2 to indicate that the leader wants the time from each node
		Send(2);
	}

	// Receive the different time values for each node
	public void Receive(long[] readBuffer)
	{
		for (int i = 0; i < comSize; i++)
		{
			byte[] bytes = new Byte[sizeof(long)];
			noeuds[i].Receive(bytes);
			readBuffer[i + 1] = BitConverter.ToInt64(bytes);
		}
	}
}

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

	Socket handler;
	Socket socket;

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

	public void Accept()
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

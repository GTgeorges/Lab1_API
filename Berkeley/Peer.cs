using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;

public interface IPeer
{
	public void OpenConnection(IPAddress ipAddress, int port);

	public void AcceptConnection();

	public int Receive(byte[] readBuffer);

	public void Close();

	public void Send(long time);
}

public class Peer : IPeer
{
	private Socket handler;
	private Socket socket;

	public void OpenConnection(IPAddress ipAddress, int port)
	{
		socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
		socket.Bind(new IPEndPoint(ipAddress, port));
		socket.Listen();
	}

	public void AcceptConnection()
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

// Ancient import pour Java

// import javax.xml.bind.SchemaOutputResolver;
// import java.io.*;
// import java.net.InetAddress;
// import java.net.ServerSocket;
// import java.net.Socket;

using System;
using System.Net;
using System.Net.Sockets;

public class Noeud : INoeud
{

	private long time;
	private Boolean isLeader = false;
	private Socket s;

	public Socket StartNoeud(int port, int n, Boolean isLeader)
	{
		try
		{
			port = 25100 + 100 * n;
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

			Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			try
			{
				s.Connect(remoteEP);

				if (isLeader)
				{
					int[] ports = new int[] { 25200, 25300 };
					RequestTime(ports, 1);
				}
				else
				{

				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}
	}

	public long[] RequestTime(int[] ports, long seuil)
	{

	}


	public long GetTime()
	{

		return time;
	}

	public void SetTime(long offset)
	{
		time += offset;
	}
}

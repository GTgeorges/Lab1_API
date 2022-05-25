// package classes;

// import interfaces.INoeud;

// import javax.xml.bind.SchemaOutputResolver;
// import java.io.*;
// import java.net.InetAddress;
// import java.net.ServerSocket;
// import java.net.Socket;

using INoeud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Net.Mail;



public class Noeud : INoeud<>
{

	private long time;


	public long[] requestTime(int[] ports, long seuil) throws IOException
	{

	}


	public Socket startNoeud(int port) throws IOException
	{

	}

	public long getTime()
	{
		return time;
	}

	public void setTime(long offset)
	{

	}
}

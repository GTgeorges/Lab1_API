using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;

public interface IService
{
	public DateTime UtcNow();
	public IPeer GetPeer();
	public ILeader GetLeader(int comSize);
}

public class Service : IService
{
	public DateTime UtcNow()
	{
		return DateTime.UtcNow;
	}

	public IPeer GetPeer()
	{
		return new Peer();
	}

	public ILeader GetLeader(int comSize)
	{
		return new Leader(comSize);
	}
}

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

	public void OpenConnection()
	{
		
	}
}

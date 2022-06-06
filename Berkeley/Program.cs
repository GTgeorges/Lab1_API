using System;

internal class Program
{
	static void Main(string[] args)
	{
		Noeud noeud = new Noeud(new Service());

		int n = int.Parse(args[0]);
		bool isLeader = bool.Parse(args[1]);

		noeud.StartNoeud(n, isLeader);
	}
}

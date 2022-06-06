using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

interface INoeud
{
	long[] RequestTime(ILeader leader);
	void StartNoeud(int rank, bool isLeader);
	long GetTime();
	void SetTime(long offset);
}

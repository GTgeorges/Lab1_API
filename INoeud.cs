using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Net.Mail;

interface INoeud
{
	long[] RequestTime(int[] ports, long seuil);
	void StartNoeud(int n, bool isLeader);
	long GetTime();
	void SetTime(long offset);
}

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
	long[] RequestTime(Socket[] noeuds);
	void StartNoeud(int rank, bool isLeader);
	long GetTime();
	void SetTime(long offset);
}

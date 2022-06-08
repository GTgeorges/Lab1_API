namespace LOG736_Labo1
{
	public interface IServeur
	{
		public void StartServer(int port, int numberOftries);
		public void StopServer();
		public long GetTime();
	}
}

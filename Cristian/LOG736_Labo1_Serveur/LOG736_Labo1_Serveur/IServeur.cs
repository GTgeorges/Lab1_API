namespace LOG736_Labo1
{
    public interface IServeur
    {
        public void StartServer(int port);
        public void StopServer();
        public long GetTime();
    }
}

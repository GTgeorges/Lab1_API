namespace LOG736_Labo1
{
    public interface IClient
    {
        public void RequestTime(int serverPort, long currentTime, int numberOftries);
        public long GetTime();
        public void SetTime(long newTime);
        public long GetAccuracy();
    }
}

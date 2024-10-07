namespace MyGameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            GameServer gameServer = new GameServer();

            gameServer.Init();
            gameServer.Bind(9999);

            ThreadStart threadStart = new ThreadStart(gameServer.AcceptMultiClients);
            Thread thread = new Thread(threadStart);

            thread.Start();

        }
    }
}
using System.Net;
using System.Net.Sockets;

namespace SocketsApp
{
    class Program
    {
        static void Main()
        {
            ClientSocket clientSocket = new ClientSocket();
            clientSocket.ClientMessage += GetMessage; 
            clientSocket.StartClient("127.0.0.1");
            //Thread.Sleep(10000);
        }

        public static string GetMessage(string message)
        {
            Console.WriteLine(message);
            return "";
        }
        
    }
}
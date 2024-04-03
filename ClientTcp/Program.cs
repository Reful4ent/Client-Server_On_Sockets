using System.Net;
using System.Net.Sockets;

namespace SocketsApp
{
    class Program
    {
        static void Main()
        {
            ClientSocket clientSocket = new ClientSocket();
            clientSocket.StartClient("dsad");
        }
    }
}
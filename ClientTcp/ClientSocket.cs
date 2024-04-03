using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketsApp;

public class ClientSocket
{
    string ip = "127.0.0.1";
    const int port = 8080;
    public void StartClient(string ip)
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(this.ip), port);
        Socket tcpSocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        tcpSocketClient.Connect(endPoint);
        
        while (true)
        {
            byte[] buffer = new byte[256];
            int size = 0;
            StringBuilder answer = new StringBuilder();

            do
            {
                size = tcpSocketClient.Receive(buffer);
                answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
            } while (tcpSocketClient.Available>0);
        
            Console.WriteLine($"Клиент получил {DateTime.Now} {answer.ToString()}");
            
            string message = Console.ReadLine();
            var data = Encoding.UTF8.GetBytes(message);
            if (message == "exit")
            {
                tcpSocketClient.Send(Encoding.UTF8.GetBytes("exit"));
                tcpSocketClient.Shutdown(SocketShutdown.Both);
                tcpSocketClient.Close();
                break;
            }
            tcpSocketClient.Send(data);
            Array.Clear(data);
        }
    }
}
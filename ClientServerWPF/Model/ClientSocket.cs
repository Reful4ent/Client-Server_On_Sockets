using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketsApp;

public class ClientSocket
{
    string ip = string.Empty;
    const int port = 8080;
    public Func<string, string>? ClientMessage;
    private IPEndPoint endPoint;
    private Socket tcpSocketClient;
    
    public async Task StartClient(string ip)
    {
        try
        {
            this.ip = ip;
            endPoint = new IPEndPoint(IPAddress.Parse(this.ip), port);
            tcpSocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpSocketClient.Connect(endPoint);
        }
        catch (Exception e)
        {
            ClientMessage?.Invoke("Неверный адрес сервера!");
            return;
        }
        await Task.Run(async () =>
        {
            while (true)
            {
                byte[] buffer = new byte[256];
                int size = 0;
                StringBuilder answer = new StringBuilder();
                do
                {
                    size = await tcpSocketClient.ReceiveAsync(buffer);
                    answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
                } while (tcpSocketClient.Available > 0);

                if (answer.ToString() == "exit")
                {
                    tcpSocketClient.Shutdown(SocketShutdown.Both);
                    tcpSocketClient.Close();
                    break;
                }
                ClientMessage?.Invoke($"Клиент получил {DateTime.Now} {answer.ToString()}");
            }
        });
        return;
    }

    public async Task SendMessageAsync(string message)
    {
        try
        {
            await Task.Run(async () =>
            {
                if (String.IsNullOrEmpty(message))
                    message = " ";

                await tcpSocketClient.SendAsync(Encoding.UTF8.GetBytes(message));
            });
        }
        catch (Exception e)
        {
            ClientMessage?.Invoke($"Клиент отсутствует или сервер отстутствует");
            return;
        }
    }
}


using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketsApp;

public class ClientSocket
{
    string ip = string.Empty;
    const int port = 8080;
    private bool messageIsSend = false;
    private string clientSend = string.Empty;
    public Func<string, string>? ClientMessage;
    
    public async Task StartClient(string ip)
    {
        await Task.Run(() =>
        {
            try
            {
                this.ip = ip;
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
                    } while (tcpSocketClient.Available > 0);

                    ClientMessage?.Invoke($"Клиент получил {DateTime.Now} {answer.ToString()}");

                    while (messageIsSend != true)
                    {
                        
                    }

                    string message = clientSend;

                    if (String.IsNullOrEmpty(message))
                        message = " ";

                    var data = Encoding.UTF8.GetBytes(message);
                    if (message == "exit")
                    {
                        tcpSocketClient.Send(Encoding.UTF8.GetBytes("exit"));
                        tcpSocketClient.Shutdown(SocketShutdown.Both);
                        tcpSocketClient.Close();
                        break;
                    }

                    messageIsSend = false;
                    tcpSocketClient.Send(data);
                    Array.Clear(data);
                }
            }
            catch (Exception e)
            {
                ClientMessage?.Invoke("Неверный адрес сервера!");
                return;
            }
        });
    }

    public void Clients(string m)
    {
        clientSend = m;
        messageIsSend = true;
    }
}
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Documents;


namespace SocketsApp;

public class ClientSocket
{
    string ip = string.Empty;
    const int port = 8080;
    private IPEndPoint endPoint;
    private Socket tcpSocketClient;

    private List<string> drives;
    private List<string> directories;
    
    public Action<bool>? IsConnected;
    
    public Func<string, string>? ClientMessage;
    public Action<List<string>>? ClientGetDrives;
    public Action<List<string>>? СlientGetDirectory;
    
    /// <summary>
    /// Start the client.
    /// Запуск клиента.
    /// </summary>
    /// <param name="ip"></param>
    public async Task StartClient(string ip)
    {
        try
        {
            if (tcpSocketClient != null && tcpSocketClient.Connected) 
                return;
            this.ip = ip;
            endPoint = new IPEndPoint(IPAddress.Parse(this.ip), port);
            tcpSocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await tcpSocketClient.ConnectAsync(endPoint);
            IsConnected?.Invoke(true);
        }
        catch (Exception e)
        {
            //If the server was turned off.
            //Если сервер был выключен.
            if (e.HResult == -2147467259)
            {
                ClientMessage?.Invoke("Не удалось получить ответ с удаленного сервера");
                IsConnected?.Invoke(false);
                return;
            }
            ClientMessage?.Invoke("Неверный адрес сервера!"); 
            IsConnected?.Invoke(false);
            return;
        }
        
        
        await Task.Run(async () =>
        {
            while (true)
            {
                byte[] buffer = new byte[256];
                int size = 0;
                StringBuilder answer = new StringBuilder();

                try
                {
                    do
                    {
                        size = await tcpSocketClient.ReceiveAsync(buffer);
                        answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
                    } while (tcpSocketClient.Available > 0);
                    
                    string[] tempPathElements = answer
                        .ToString()
                        .Split("\n")
                        .Take(answer.ToString().Split("\n").Length)
                        .ToArray();
                    

                    switch (tempPathElements[tempPathElements.Length - 1])
                    {
                        case "drives":
                            drives = new List<string>();
                            for (int i = 1; i < tempPathElements.Length-1; i++)
                                drives.Add(tempPathElements[i]);
                            
                            tempPathElements = answer
                                .ToString()
                                .Split("\n")
                                .Take(answer.ToString().Split("\n").Length-1)
                                .ToArray();
                            
                            answer = new StringBuilder(String.Join("\n",tempPathElements)+"\n");
                            ClientGetDrives?.Invoke(drives);
                            break;
                        
                        case "directories":
                            directories = new List<string>();
                            for (int i = 1; i < tempPathElements.Length-1; i++)
                                directories.Add(tempPathElements[i]);
                            
                            tempPathElements = answer
                                .ToString()
                                .Split("\n")
                                .Take(answer.ToString().Split("\n").Length-1)
                                .ToArray();
                            
                            if (tempPathElements[1] == ".")
                            {
                                tempPathElements = tempPathElements
                                    .Reverse()
                                    .Take(tempPathElements.Length - 3)
                                    .Append("\n")
                                    .Reverse()
                                    .ToArray();
                            }
                            answer = new StringBuilder(String.Join("\n",tempPathElements)+"\n");
                            СlientGetDirectory?.Invoke(directories);
                            break;
                        default: break;
                    }
                    
                    
                }
                catch (Exception e)
                {
                    return;
                }

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
    
    /// <summary>
    /// Send a message to the server.
    /// Отправляет сообщение для сервера.
    /// </summary>
    /// <param name="message"></param>
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
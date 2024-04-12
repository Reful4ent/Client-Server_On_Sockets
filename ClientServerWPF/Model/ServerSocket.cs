using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.IO;
using System.Printing;


namespace SocketsApp;

public class ServerSocket
{
    
    const string ip = "127.0.0.1";
    const int port = 8080;
    Socket listener = null;
    private IPEndPoint endPoint;
    private Socket tcpSocketServer;
    private bool isWork = true;
    public Func<string, string>? ServerMessage;
    
    // List of the file extensions.
    // Список текстовых расширений для чтения.
    private readonly List<string> extensions = new List<string>()
    {
        ".txt",
        ".bin",
        ".html",
        ".doc",
        ".rtf",
    };
    
    /// <summary>
    /// Start the server.
    /// Запуск сервера.
    /// </summary>
    public async Task StartServer()
    {
        
        endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        tcpSocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        tcpSocketServer.Bind(endPoint);
        tcpSocketServer.Listen(10);
        ServerMessage?.Invoke($"Сервер включен: {DateTime.Now}\n");

        await Task.Run(async () =>
        {
            while (isWork)
            {
                if (listener == null || !(listener.Connected))
                {
                    try
                    {
                        listener = await tcpSocketServer.AcceptAsync();
                        ServerMessage?.Invoke($"Клиент присоединился {DateTime.Now} \n c адреса: {listener.RemoteEndPoint}\n");
                        await listener.SendAsync(Encoding.UTF8.GetBytes(ShowDriversInfo()));
                    }
                    catch (Exception e)
                    {
                        return;
                    }
                }

                byte[] buffer = new byte[256];
                int size = 0;
                StringBuilder data = new StringBuilder();


                try
                {
                    size = await listener.ReceiveAsync(buffer);
                    data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                catch (Exception e)
                {
                    size = -1;
                }
                
                
                
                if (size > 0)
                {
                    ServerMessage?.Invoke($"Сервер получил {DateTime.Now} \n {data.ToString()}\n");
                    await listener.SendAsync(Encoding.UTF8.GetBytes(ResponseRequest(data)));
                }
                
                
                if (data.ToString() == "exit")
                    DisconnectClient();
            }
        });
    }

    #region Response_Messages
    /// <summary>
    /// Creates a response on the client`s request.
    /// Создает ответ на запрос клиента.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private string ResponseRequest(StringBuilder data)
    {
        
        string message = data.ToString();
        
        
        if (message == "exit")
            return "exit";

        if (message == "ServerExit")
            return "Сервер разорвал соединение";
        
        if (String.IsNullOrEmpty(message) || String.IsNullOrWhiteSpace(message))
            return "Пустое сообщение";
        
        string response = string.Empty;
        var directory = new DirectoryInfo(message);
        
        //If path contains elements from "extensions", then check the existence of the file.
        //If the file exists,then read it.
        //Если путь содержит элементы из списка разрешений,то проверяем файл на существование по указанному пути.
        //Если файл существует, то считываем его.
        if (extensions.Exists(x=>message.Contains(x)))
        {
            if (!File.Exists(message))
                return "Не является файлом!";
            return "\n" + File.ReadAllText(message);
        }
        
        //If directory exists, then create info about child elements of this directory.
        //Если директория существует, то создаем информацию о дочерних элементах директории.
        if (directory.Exists)
        {
            DirectoryInfo[] dirs = directory.GetDirectories();
            FileInfo[] files = directory.GetFiles();
            
            foreach (DirectoryInfo item in dirs)
                response += "\n" + item.FullName;

            foreach (FileInfo item in files)
                response += "\n" + item.FullName;
            
            return response + "\n";
        }
        return "Полученноe сообщение не является директорией или файлом";
    }
    
    
    /// <summary>
    /// Send the information about available logical drives on the PC.
    /// Выводит информацию о имеющихся логических устройствах на ПК.
    /// </summary>
    /// <returns></returns>
    private string ShowDriversInfo()
    {
        DriveInfo[] allDrives = DriveInfo.GetDrives();
        string driversInfo = string.Empty;
        
        if (allDrives == null || allDrives.Length == 0)
            return "Логические устройства отсутствуют";
        
        foreach (DriveInfo drive in allDrives)
            driversInfo += "\n" + drive.Name + "\n";
        
        return driversInfo;
    }
    
    #endregion

    #region Instruments_for_Server

    
    /// <summary>
    /// Send a message to the client.
    /// Отправляет сообщение для клиента.
    /// </summary>
    /// <param name="message"></param>
    public async Task SendMessageAsync(string message)
    {
        await Task.Run(async () =>
        {
            if (String.IsNullOrEmpty(message))
                message = " ";
            StringBuilder data = new StringBuilder(message);
            await listener.SendAsync(Encoding.UTF8.GetBytes(ResponseRequest(data)));
        });
    }
    
    
    /// <summary>
    /// Disconnect the client from the server.
    /// Отключает пользователя от сервера.
    /// </summary>
    private void DisconnectClient()
    {
        ServerMessage?.Invoke($"Клиент с адресом {listener.RemoteEndPoint} отключился {DateTime.Now}");
        listener.Dispose();
    }
    
    
    /// <summary>
    /// Turn off the server and client if it connected.
    /// Выключает сервер и клиента, если он подключен.
    /// </summary>
    public async Task DisposeServer()
    {
        isWork = false;
        if (listener !=null && listener.Connected)
        {
            await listener.SendAsync(Encoding.UTF8.GetBytes(ResponseRequest(new StringBuilder("ServerExit"))));
            DisconnectClient();
        }
        ServerMessage?.Invoke($"Сервер отключился {DateTime.Now}");
        tcpSocketServer.Dispose();
    }
    #endregion
   
}
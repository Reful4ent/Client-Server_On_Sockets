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
    private bool flag = true;
    public Func<string, string>? ServerMessage;
    
    // Список текстовых расширений для чтения
    private readonly List<string> extensions = new List<string>()
    {
        ".txt",
        ".bin",
        ".html",
        ".doc",
        ".rtf",
    };
    
    
    public async Task StartServer()
    {
        
        endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        tcpSocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        tcpSocketServer.Bind(endPoint);
        tcpSocketServer.Listen(10);
        ServerMessage?.Invoke($"Сервер включен: {DateTime.Now}");

        await Task.Run(async () =>
        {
            while (flag)
            {
                if (listener == null || !(listener.Connected))
                {
                    try
                    {
                        listener = await tcpSocketServer.AcceptAsync();
                        ServerMessage?.Invoke($"Клиент присоединился {DateTime.Now} \n c адреса: {listener.RemoteEndPoint}");
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

                
                size = await listener.ReceiveAsync(buffer);
                data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                
                
                
                if (size > 0)
                {
                    ServerMessage?.Invoke($"Сервер получил {DateTime.Now} \n {data.ToString()}");
                    await listener.SendAsync(Encoding.UTF8.GetBytes(ResponseRequest(data)));
                }
                
                
                if (data.ToString() == "exit")
                    DisconnectClient();
            }
        });
    }
    
    /// <summary>
    /// Создает ответ на запрос клиента
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
        
        //Проверка на то что строка имеет данные подстроки, а дальше на существование файла
        if (extensions.Exists(x=>message.Contains(x)))
        {
            if (!File.Exists(message))
                return "Не является файлом!";
            return File.ReadAllText(message);
        }
        
        if (directory.Exists)
        {
            DirectoryInfo[] dirs = directory.GetDirectories();
            FileInfo[] files = directory.GetFiles();
            
            foreach (DirectoryInfo item in dirs)
                response += "\n" + item.FullName;

            foreach (FileInfo item in files)
                response += "\n" + item.FullName;
            
            return response;
        }
        return "Полученноe сообщение не являеется директорией или файлом";
    }
    
    
    /// <summary>
    /// Выводит информацию о имеющихся логических устройствах на ПК
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
    
    
    public async Task CloseServer()
    {
        try
        {
            await listener.SendAsync(Encoding.UTF8.GetBytes(ResponseRequest(new StringBuilder("exit"))));
            DisconnectClient();
            ServerMessage?.Invoke($"Сервер отключился {DateTime.Now}");
            tcpSocketServer.Shutdown(SocketShutdown.Both);
            tcpSocketServer.Close();
        }
        catch (Exception e)
        {
            return;
        }
    }
    
    private void DisconnectClient()
    {
        ServerMessage?.Invoke($"Клиент с адресом {listener.RemoteEndPoint} отключился {DateTime.Now}");
        listener.Shutdown(SocketShutdown.Both);
        listener.Close();
    }


    public void Dispose()
    {
        flag = false;
        if (listener !=null && listener.Connected)
        {
            listener.Send(Encoding.UTF8.GetBytes(ResponseRequest(new StringBuilder("exit"))));
            DisconnectClient();
        }
        ServerMessage?.Invoke($"Сервер отключился {DateTime.Now}");
        tcpSocketServer.Dispose();
    }
}
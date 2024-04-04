using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketsApp;

public class ServerSocket
{
    
    const string ip = "127.0.0.1";
    const int port = 8080;
    Socket listener = null;
    
    // Список текстовых расширений для чтения
    private readonly List<string> extensions = new List<string>()
    {
        ".txt",
        ".bin",
        ".html",
        ".doc",
        ".rtf",
    };
    
    public void StartServer()
    {
        
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        Socket tcpSocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        tcpSocketServer.Bind(endPoint);
        tcpSocketServer.Listen(10);
        Console.WriteLine($"Сервер включен: {DateTime.Now}");
        
        while (true)
        {
            if (listener == null || !(listener.Connected))
            {
                listener = tcpSocketServer.Accept();
                Console.WriteLine($"Клиент присоединился {DateTime.Now} \n c адреса: {listener.RemoteEndPoint}");
                listener.Send(Encoding.UTF8.GetBytes(ShowDriversInfo()));
            }
            
            byte[] buffer = new byte[256];
            int size = 0;
            StringBuilder data = new StringBuilder();
            
            do
            {
                size = listener.Receive(buffer);
                data.Append(Encoding.UTF8.GetString(buffer, 0, size));
            } while (listener.Available>0);
            
            Console.WriteLine(ServerGetMessage(data));
            listener.Send(Encoding.UTF8.GetBytes(ResponseRequest(data)));
            
            if (data.ToString() == "exit")
            {
                Console.WriteLine($"Клиент с адресом {listener.RemoteEndPoint} отключился {DateTime.Now}");
                listener.Shutdown(SocketShutdown.Both);
                listener.Close();
            }
        }
    }
    
    /// <summary>
    /// Создает сообщение о том что и когда передал клиент на сервер
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private string ServerGetMessage(StringBuilder data) => $"Сервер получил {DateTime.Now} \n {data.ToString()}";

    
    /// <summary>
    /// Создает ответ на запрос клиента
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private string ResponseRequest(StringBuilder data)
    {
        
        string message = data.ToString();
        
        if (String.IsNullOrEmpty(message) || String.IsNullOrWhiteSpace(message))
            return "Пустое сообщение";
        
        string response = string.Empty;
        var directory = new DirectoryInfo(message);
        
        
        if (directory.Exists)
        {
            DirectoryInfo[] dirs = directory.GetDirectories();
            FileInfo[] files = directory.GetFiles();
            
            foreach (DirectoryInfo item in dirs)
                response += item.FullName + "\n";

            foreach (FileInfo item in files)
                response += item.FullName + "\n";
            
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
}
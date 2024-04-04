using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketsApp;

public class ServerSocket
{
    const string ip = "127.0.0.1";
    const int port = 8080;
    Socket listener = null;
    
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


    private string ServerGetMessage(StringBuilder data) => $"Сервер получил {DateTime.Now} \n {data.ToString()}";

    private string ResponseRequest(StringBuilder data)
    {
        string message = data.ToString();
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
        if (message == null || String.IsNullOrEmpty(message))
            return "Пустое сообщение";
        return "Goida";
    }
    
    
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
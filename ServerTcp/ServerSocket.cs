using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketsApp;

public class ServerSocket
{
    const string ip = "127.0.0.1";
    const int port = 8080;
    
    public void StartServer()
    {
        
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        Socket tcpSocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        tcpSocketServer.Bind(endPoint);
        tcpSocketServer.Listen(10);
        Console.WriteLine($"Сервер включен: {DateTime.Now}");
        
        while (true)
        {
            Socket listener = tcpSocketServer.Accept();
            Console.WriteLine();
            byte[] buffer = new byte[256];
            int size = 0;
            StringBuilder data = new StringBuilder();
            listener.Send(Encoding.UTF8.GetBytes(ShowDriversInfo()));
            
            do
            {
                size = listener.Receive(buffer);
                data.Append(Encoding.UTF8.GetString(buffer, 0, size));
            } while (listener.Available>0);
            Console.WriteLine(data);
            listener.Send(Encoding.UTF8.GetBytes("Deliches"));
            if (data.ToString() == "close")
            {
                listener.Shutdown(SocketShutdown.Both);
                listener.Close();
            }
        }
    }

    private string ShowDriversInfo()
    {
        DriveInfo[] allDrives = DriveInfo.GetDrives();
        string driversInfo = string.Empty;
        if (allDrives == null || allDrives.Length == 0)
            return "Логические устройства отсутствуют";
        foreach (DriveInfo drive in allDrives)
        {
            driversInfo += drive.Name + "\n";
        }
        return driversInfo;
    }
}
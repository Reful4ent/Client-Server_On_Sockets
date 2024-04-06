using SocketsApp;
static string GetMessage(string message)
{
    Console.WriteLine(message);
    return "";
}

ServerSocket socket = new ServerSocket();
socket.ServerMessage += GetMessage;
socket.StartServer();
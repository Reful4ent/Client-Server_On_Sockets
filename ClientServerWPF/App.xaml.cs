using System.Configuration;
using System.Data;
using System.Windows;
using SocketsApp;
using WpfApp1.View;

namespace WpfApp1;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServerSocket _serverSocket;
    private ClientSocket _clientSocket;

    public App() : base()
    {
        _serverSocket = new ServerSocket();
        _clientSocket = new ClientSocket();
        
    }
    
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ClientServerWindow clientServerWindow = new ClientServerWindow(_serverSocket, _clientSocket);
        clientServerWindow.Show();
    }
}


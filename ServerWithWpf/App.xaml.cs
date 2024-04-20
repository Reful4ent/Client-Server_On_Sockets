using System.Configuration;
using System.Data;
using System.Windows;
using ServerWithWpf.Model;
using ServerWithWpf.View;
using ServerWithWpf.ViewModel;

namespace ServerWithWpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServerSocket _serverSocket;
    

    public App() : base()
    {
        _serverSocket = new ServerSocket();
    }
    
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ServerWindow clientServerWindow = new ServerWindow(_serverSocket);
        clientServerWindow.Show();
    }
}
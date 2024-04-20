using System.Configuration;
using System.Data;
using System.Windows;
using ClientWithWpf.Model;
using ClientWithWpf.View;

namespace ClientWithWpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ClientSocket _clientSocket;
    
    public App() : base()
    {
        _clientSocket = new ClientSocket();
    }
    
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ClientWindow clientServerWindow = new ClientWindow( _clientSocket);
        clientServerWindow.Show();
    }
}
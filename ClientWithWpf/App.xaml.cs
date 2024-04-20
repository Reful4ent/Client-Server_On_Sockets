using System.Configuration;
using System.Data;
using System.Windows;
<<<<<<<< HEAD:ServerWithWpf/App.xaml.cs
using ServerWithWpf.Model;
using ServerWithWpf.View;
using ServerWithWpf.ViewModel;

namespace ServerWithWpf;
========
using ClientWithWpf.Model;
using ClientWithWpf.View;

namespace ClientWithWpf;
>>>>>>>> ClientWpf:ClientWithWpf/App.xaml.cs

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
<<<<<<<< HEAD:ServerWithWpf/App.xaml.cs
    private ServerSocket _serverSocket;
    

    public App() : base()
    {
        _serverSocket = new ServerSocket();
========
    private ClientSocket _clientSocket;

    public App() : base()
    {
        _clientSocket = new ClientSocket();
>>>>>>>> ClientWpf:ClientWithWpf/App.xaml.cs
    }
    
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
<<<<<<<< HEAD:ServerWithWpf/App.xaml.cs
        ServerWindow clientServerWindow = new ServerWindow(_serverSocket);
========
        ClientWindow clientServerWindow = new ClientWindow( _clientSocket);
>>>>>>>> ClientWpf:ClientWithWpf/App.xaml.cs
        clientServerWindow.Show();
    }
}
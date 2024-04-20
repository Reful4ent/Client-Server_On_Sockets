using System.Windows;
using ServerWithWpf.Model;
using ServerWithWpf.ViewModel;
namespace ServerWithWpf.View;

public partial class ServerWindow : Window
{
    public ServerWindow(ServerSocket serverSocket)
    {
        InitializeComponent();
        DataContext = new MainVM(serverSocket);
    }
}
using System.Windows;
using SocketsApp;
using WpfApp1.ViewModel;

namespace WpfApp1.View;

public partial class ClientServerWindow : Window
{
    public ClientServerWindow(ServerSocket serverSocket,ClientSocket clientSocket)
    {
        InitializeComponent();
        DataContext = new MainVM(serverSocket,clientSocket);
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
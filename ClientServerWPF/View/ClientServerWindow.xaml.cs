using System.Windows;
using System.Windows.Media;
using SocketsApp;
using WpfApp1.ViewModel;

namespace WpfApp1.View;

public partial class ClientServerWindow : Window
{
    public ClientServerWindow(ServerSocket serverSocket,ClientSocket clientSocket)
    {
        InitializeComponent();
        DataContext = new MainVM(serverSocket,clientSocket);
       /* if (DataContext is MainVM mainVm)
        {
            mainVm.IsClientConnectedAction += ButtonConnected_State;
        }*/
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    /*(private void ButtonConnected_State(bool isConnected)
    {
        if (isConnected)
        {
            StartClientButton.IsEnabled = false;
            StartClientButton.Foreground = Brushes.DarkSeaGreen;
            return;
        }
        StartClientButton.IsEnabled = true;
        StartClientButton.Foreground = Brushes.Black;
        return;
    }*/
}
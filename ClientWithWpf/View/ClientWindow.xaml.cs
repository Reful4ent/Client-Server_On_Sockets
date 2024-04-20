using System.Windows;
using ClientWithWpf.Model;
using ClientWithWpf.ViewModel;
using System.Windows.Media;

namespace ClientWithWpf.View;

public partial class ClientWindow : Window
{
    public ClientWindow(ClientSocket clientSocket)
    {
        InitializeComponent();
        DataContext = new MainVM(clientSocket);
        if (DataContext is MainVM mainVm)
        {
            mainVm.IsClientConnectedAction += ButtonConnected_State;
        }
        
    }
    
    private void ButtonConnected_State(bool isConnected)
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
    }
}
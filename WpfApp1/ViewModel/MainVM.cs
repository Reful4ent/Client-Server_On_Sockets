using System.Windows.Input;
using SocketsApp;
using WpfApp1.ViewModel.Commands;

namespace WpfApp1.ViewModel;

public class MainVM : BaseVM
{
   private ServerSocket _serverSocket;
   private ClientSocket _clientSocket;
   private string serverText;
   private string clientText;
   public MainVM(ServerSocket serverSocket, ClientSocket clientSocket)
   {
      _serverSocket = serverSocket;
      _clientSocket = clientSocket;
      _serverSocket.ServerMessage += GetServerText;
      _clientSocket.ClientMessage += GetClientText;
      StartServer();
   }

   public async void StartServer()
   {
      await _serverSocket.StartServer();
   }

   public async void StartClient()
   {
      await _clientSocket.StartClient("127.0.0.1");
   }

   public async void CloseClient()
   {
      await _clientSocket.SendMessageAsync("exit");
      clientText = string.Empty;
   }


   public string ServerText
   {
      get => serverText;
      set => Set(ref serverText, serverText+"\n"+value);
   }

   public string ClientText
   {
      get => clientText;
      set => Set(ref clientText, clientText + "\n" + value);
   }
   private string GetServerText(string message) => ServerText = message;
   private string GetClientText(string message) => ClientText = message;

   public Command StartServerCommand => Command.Create(StartClient);
   public Command EndServerCommand => Command.Create(CloseClient);

}
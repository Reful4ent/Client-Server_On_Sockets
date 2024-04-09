using System.Windows.Input;
using SocketsApp;
using WpfApp1.ViewModel.Commands;

namespace WpfApp1.ViewModel;

public class MainVM : BaseVM
{
   private string ipAdress = string.Empty;
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

   public string IpAdress
   {
      get => ipAdress;
      set => Set(ref ipAdress, value);
   }
   
   private string GetServerText(string message) => ServerText = message;
   private string GetClientText(string message) => ClientText = message;

   public Command StartClientCommand => Command.Create(StartClient);
   public Command EndClientCommand => Command.Create(CloseClient);
   public Command EndServerCommand => Command.Create(CloseServer);

   public Command SendToServerCommand => Command.Create(async () =>
   {
      await _clientSocket.SendMessageAsync(@"C:\Users\dima2\Documents\aboba.txt");
   });
   
   public async void StartClient()
   {
      await _clientSocket.StartClient(IpAdress);
   }

   public async void CloseClient()
   {
      await _clientSocket.SendMessageAsync("exit");
      clientText = string.Empty;
   }

   public async void StartServer()
   {
      await _serverSocket.StartServer();
   }

   public async void CloseServer()
   {
      await _serverSocket.DisposeServer();
   }
   
   


}
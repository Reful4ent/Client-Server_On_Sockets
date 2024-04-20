using System.Collections.ObjectModel;
using SocketsApp;
using System.IO;
using WpfApp1.ViewModel.Commands;

namespace WpfApp1.ViewModel;

public class MainVM : BaseVM
{
   private string ipAdress = string.Empty;
   private ServerSocket serverSocket;
   private ClientSocket clientSocket;
   private string serverText;
   private string clientText;
   private int indexDrive;
   private int indexPath;
   private ObservableCollection<string> drives;
   private ObservableCollection<string> directoryInfo;

   private bool isFirstDrive = true;
   private string currentDrive = string.Empty;
   private string driveItem = string.Empty; 
   private string fullPath = String.Empty;
   
   public Action<bool>? IsClientConnectedAction;
   
   public MainVM(ServerSocket _serverSocket, ClientSocket _clientSocket)
   {
      serverSocket = _serverSocket;
      clientSocket = _clientSocket;
      serverSocket.ServerMessage += GetServerText;
      clientSocket.ClientGetDrives += ShowDriversInfo;
      clientSocket.СlientGetDirectory += ShowDirectoryInfo;
      clientSocket.ClientMessage += GetClientText;
      clientSocket.IsConnected += CheckClientConnected;
      StartServer();
   }


   #region Bindings_for_Server_Client
   public string ServerText
   {
      get => serverText;
      set => Set(ref serverText, serverText+"\n"+value);
   }
   public string ClientText
   {
      get => clientText;
      set
      {
         if (value == string.Empty)
         {
            Set(ref clientText, "");
            return;
         }
         Set(ref clientText, clientText + "\n" + value);
      }
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
   public Command SendToServerCommand => Command.Create(SendToServer);
   public Command SendToClientCommand => Command.Create(SendToClient);
   
   private async void StartClient()
   {
      await clientSocket.StartClient(IpAdress);
   }
   private async void CloseClient()
   {
      await clientSocket.SendMessageAsync("exit");
      Drives.Clear();
      if (DirectoryInfo != null)
      {
         DirectoryInfo.Clear();
         DirectoryInfo = null;
      }
      IsClientConnectedAction?.Invoke(false);
      ClientText = String.Empty;
   }

   private async void SendToServer()
   {
      if(DirectoryInfo != null)
         FullPath = DirectoryInfo[IndexPath];
      
      Drives[IndexDrive] = FullPath;
      DriveItem = Drives[IndexDrive];
      await clientSocket.SendMessageAsync(FullPath);
    }

   private async void StartServer()
   {
      await serverSocket.StartServer();
   }
   private async void CloseServer()
   {
      await serverSocket.DisposeServer();
      IsClientConnectedAction?.Invoke(false);
   }
   private async void SendToClient()
   {
      await serverSocket.SendMessageAsync(FullPath);
   }
   private void CheckClientConnected(bool isConnected)
   {
      IsClientConnectedAction?.Invoke(isConnected);
   }
   #endregion
   
   
   
   #region Bindings_Drives_and_Path
   
   /// <summary>
   /// Index of the selected drive in the List of drives.
   /// Индекс выбранного диска в списке.
   /// </summary>
   public int IndexDrive
   {
      get => indexDrive;
      set
      {
         Set(ref indexDrive, value);
         if (IndexDrive == -1)
         {
            IndexDrive = 0;
            return;
         }

         if (!isFirstDrive)
         {
            FullPath = drives[IndexDrive];
            currentDrive = drives[IndexDrive];
            isFirstDrive = true;
         }
      }
   }
   
   /// <summary>
   /// List of the drives.
   /// Список дисков.
   /// </summary>
   public ObservableCollection<string> Drives
   {
      get => drives;
      set
      {
         Set(ref drives, value);
         FullPath = Drives[IndexDrive];
         if(isFirstDrive)
            currentDrive = Drives[IndexDrive];
         isFirstDrive = false;
      }
   }

   public string DriveItem
   {
      get => driveItem;
      set => Set(ref driveItem, value);
   }
   
   /// <summary>
   /// List of child elements in the directory.
   /// Список дочерних элементов в директории.
   /// </summary>
   public ObservableCollection<string> DirectoryInfo
   {
      get => directoryInfo;
      set => Set(ref directoryInfo, value);
   }
   
   /// <summary>
   /// Index of the directory path.
   /// Индекс пути к каталогу.
   /// </summary>
   public int IndexPath
   {
      get => indexPath;
      set => Set(ref indexPath, value);
      
   }
   
   public string FullPath
   {
      get => fullPath;
      set
      {
         if (value == ".")
         {
            Console.WriteLine("currentDrive" + currentDrive);
            Set(ref fullPath, currentDrive);
            return;
         }
         if (value == "..")
         {
            string[] tempPathElements = FullPath.Split("\\").Take(FullPath.Split("\\").Length - 1).ToArray();
            
            if (tempPathElements.Length == 1)
               Set(ref fullPath, tempPathElements[0] + "\\");
            else Set(ref fullPath, String.Join("\\", tempPathElements));
            Console.WriteLine(FullPath);
         }
         else Set(ref fullPath, String.Join("\\", value));
      }
   }
   
   public Command OpenDirectoryCommand => Command.Create(ChangeDirectory);
   public Command PreviousDirectoryCommand => Command.Create(ReturnDirectory);
   
   /// <summary>
   /// Send the information about available logical drives on the PC.
   /// Выводит информацию о имеющихся логических устройствах на ПК.
   /// </summary>
   /// <returns></returns>
   private void ShowDriversInfo(List<string> drivesFromServer)
   {
      Drives = new ObservableCollection<string>(drivesFromServer);
   }
   
   /// <summary>
   /// If directory exists, then create info about child elements of this directory.
   /// Если директория существует, то создаем информацию о дочерних элементах директории.
   /// </summary>
   /// <param name="path"></param>
   
   private void ShowDirectoryInfo(List<string> information)
   {
      DirectoryInfo = new ObservableCollection<string>(information);
   }
   
   
   /// <summary>
   /// Change the directory.
   /// Меняет директорию, переходя по ней.
   /// </summary>
   private void ChangeDirectory()
   {
      SendToServer();
   }
   
   
   
   /// <summary>
   /// Returns to the previous directory.
   /// Возвращается в предыдущюю папкую.
   /// </summary>
   private async void ReturnDirectory()
   {
      string[] tempPathElements = FullPath.Split("\\").Take(FullPath.Split("\\").Length - 1).ToArray();
      
      if (tempPathElements.Length == 1)
         fullPath = tempPathElements[0]+"\\";
      else FullPath = String.Join("\\", tempPathElements);
      Drives[IndexDrive] = FullPath;
      DriveItem = Drives[IndexDrive];
      await clientSocket.SendMessageAsync(FullPath);
   }
   #endregion


}
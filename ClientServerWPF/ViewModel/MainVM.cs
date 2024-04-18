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
   private List<string> drives = new();
   private List<string> directoryInfo;
   private string fullPath = String.Empty;

   
   public Action<bool>? IsClientConnectedAction;
   
   
   
   
   
   public MainVM(ServerSocket _serverSocket, ClientSocket _clientSocket)
   {
      serverSocket = _serverSocket;
      clientSocket = _clientSocket;
      serverSocket.ServerMessage += GetServerText;
      clientSocket.ClientMessage += GetClientText;
      clientSocket.IsConnected += CheckClientConnected;
      ShowDriversInfo();
      ShowDirectoryInfo(drives[0].ToString());
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
   public Command SendToServerCommand => Command.Create(SendToServer);
   public Command SendToClientCommand => Command.Create(SendToClient);
   
   private async void StartClient()
   {
      await clientSocket.StartClient(IpAdress);
   }
   private async void CloseClient()
   {
      await clientSocket.SendMessageAsync("exit");
      IsClientConnectedAction?.Invoke(false);
      clientText = string.Empty;
   }

   private async void SendToServer()
   {
      await clientSocket.SendMessageAsync(fullPath);
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
      await serverSocket.SendMessageAsync(fullPath);
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
         
         ShowDirectoryInfo(drives[IndexDrive].ToString());
      }
   }
   
   /// <summary>
   /// List of the drives.
   /// Список дисков.
   /// </summary>
   public List<string> Drives
   {
      get => drives;
      set
      {
         Set(ref drives, value);

         FullPath = drives[indexDrive].ToString();
      }
   }
   
   
   /// <summary>
   /// List of child elements in the directory.
   /// Список дочерних элементов в директории.
   /// </summary>
   public List<string> DirectoryInfo
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
      set
      {
         Set(ref indexPath, value);
         FullPath = DirectoryInfo[IndexPath].ToString();
      }
   }
   
   public string FullPath
   {
      get => fullPath;
      set => Set(ref fullPath, value);
   }
   
   public Command OpenDirectoryCommand => Command.Create(ChangeDirectory);
   public Command PreviousDirectoryCommand => Command.Create(ReturnDirectory);
   
   /// <summary>
   /// Send the information about available logical drives on the PC.
   /// Выводит информацию о имеющихся логических устройствах на ПК.
   /// </summary>
   /// <returns></returns>
   private void ShowDriversInfo()
   {
      DriveInfo[] allDrives = DriveInfo.GetDrives();
      List<string> drives = new();
      foreach (DriveInfo drive in allDrives)
         drives.Add(drive.Name);
      Drives = drives;
   }
   
   /// <summary>
   /// If directory exists, then create info about child elements of this directory.
   /// Если директория существует, то создаем информацию о дочерних элементах директории.
   /// </summary>
   /// <param name="path"></param>
   private void ShowDirectoryInfo(string path)
   {
      List<string> information = new();
      var directory = new DirectoryInfo(path);
      
      if (directory.Exists)
      {
         DirectoryInfo[] dirs = directory.GetDirectories();
         FileInfo[] files = directory.GetFiles();
            
         foreach (DirectoryInfo item in dirs)
            if(Directory.Exists(item.FullName)) 
               information.Add(item.FullName);

         foreach (FileInfo item in files)
            information.Add(item.FullName);
         
      }
      DirectoryInfo = information;
   }
   
   
   /// <summary>
   /// Change the directory.
   /// Меняет директорию, переходя по ней.
   /// </summary>
   private void ChangeDirectory()
   {
      var directory = new DirectoryInfo(FullPath);
      List<string> information = new();
      if (directory.Exists)
      {
         DirectoryInfo[] dirs = directory.GetDirectories();
         FileInfo[] files = directory.GetFiles();
            
         foreach (DirectoryInfo item in dirs)
            if(Directory.Exists(item.FullName)) 
               information.Add(item.FullName);

         foreach (FileInfo item in files)
            information.Add(item.FullName);
         
      }
      DirectoryInfo = information;
   }
   
   
   
   /// <summary>
   /// Returns to the previous directory.
   /// Возвращается в предыдущюю папкую.
   /// </summary>
   private void ReturnDirectory()
   {
      string[] tempPathElements = FullPath.Split("\\").Take(FullPath.Split("\\").Length - 1).ToArray();
      
      if (tempPathElements.Length == 1)
         fullPath = tempPathElements[0]+"\\";
      else FullPath = String.Join("\\", tempPathElements);
      
      ChangeDirectory();
   }
   #endregion


}
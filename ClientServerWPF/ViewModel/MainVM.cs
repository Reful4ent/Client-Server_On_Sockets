using System.Data;
using System.Windows.Input;
using SocketsApp;
using System.IO;
using WpfApp1.ViewModel.Commands;

namespace WpfApp1.ViewModel;

public class MainVM : BaseVM
{
   private string ipAdress = string.Empty;
   private ServerSocket _serverSocket;
   private ClientSocket _clientSocket;
   private string serverText;
   private string clientText;
   private int indexDrive;
   private int indexPath;
   private List<string> drives = new();
   private List<string> directoryInfo;
   private string fullPath = String.Empty;
   
   
   
   
   public MainVM(ServerSocket serverSocket, ClientSocket clientSocket)
   {
      _serverSocket = serverSocket;
      _clientSocket = clientSocket;
      _serverSocket.ServerMessage += GetServerText;
      _clientSocket.ClientMessage += GetClientText;
      ShowDriversInfo();
      ShowDiryctoryInfo(drives[0].ToString());
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

   public Command SendToServerCommand => Command.Create(async () =>
   {
      await _clientSocket.SendMessageAsync(FullPath);
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
   #endregion


   #region Bindings_Drives_and_Path

   public int IndexDrive
   {
      get => indexDrive;
      set
      {
         Set(ref indexDrive, value);
         
         ShowDiryctoryInfo(drives[IndexDrive].ToString());
      }
   }

   public List<string> Drives
   {
      get => drives;
      set
      {
         Set(ref drives, value);

         FullPath = drives[indexDrive].ToString();
      }
   }
   

   public List<string> DirectoryInfo
   {
      get => directoryInfo;
      set => Set(ref directoryInfo, value);
   }

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
   
   private void ShowDriversInfo()
   {
      DriveInfo[] allDrives = DriveInfo.GetDrives();
      List<string> drives = new();
      foreach (DriveInfo drive in allDrives)
         drives.Add(drive.Name);
      Drives = drives;
   }

   private void ShowDiryctoryInfo(string path)
   {
      List<string> information = new();
      var directory = new DirectoryInfo(path);
      
      if (directory.Exists)
      {
         DirectoryInfo[] dirs = directory.GetDirectories();
         FileInfo[] files = directory.GetFiles();
            
         foreach (DirectoryInfo item in dirs)
            information.Add(item.FullName);

         foreach (FileInfo item in files)
            information.Add(item.FullName);
         
      }
      DirectoryInfo = information;
   }

   public Command OpenDirectoryCommand => Command.Create(ChangeDirectory);

   public void ChangeDirectory()
   {
      var directory = new DirectoryInfo(fullPath);
      List<string> information = new();
      if (directory.Exists)
      {
         DirectoryInfo[] dirs = directory.GetDirectories();
         FileInfo[] files = directory.GetFiles();
            
         foreach (DirectoryInfo item in dirs)
            information.Add(item.FullName);

         foreach (FileInfo item in files)
            information.Add(item.FullName);
         
      }
      DirectoryInfo = information;
   }

   #endregion


}
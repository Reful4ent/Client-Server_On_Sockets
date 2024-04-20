
using System.Collections.ObjectModel;

using ServerWithWpf.Model;
using ServerWithWpf.ViewModel.Commands;
using ServerWithWpf.ViewModel;
using System.IO;
using System.Windows.Documents;

namespace ServerWithWpf.ViewModel;

public class MainVM : BaseVM
{
    private ServerSocket serverSocket;
    private string serverText;
    private int indexDrive;
    private int indexPath;
    private ObservableCollection<string> drives;
    private ObservableCollection<string> directoryInfo;
    private bool isFirstDrive = true;
    private int prevIndex;
    private string currentDrive = string.Empty;
    private string driveItem = string.Empty; 
    private string fullPath = String.Empty;
   
    public Action<bool>? IsClientConnectedAction;
    
    public MainVM(ServerSocket _serverSocket)
    {
        serverSocket = _serverSocket;
        serverSocket.ServerMessage += GetServerText;
        StartServer();
        ShowDriversInfo();
    }
    
   #region Bindings_for_Server-Work
   public string ServerText
   {
      get => serverText;
      set => Set(ref serverText, serverText+"\n"+value);
   }
   private string GetServerText(string message) => ServerText = message;
   
   public Command EndServerCommand => Command.Create(CloseServer);
   public Command SendToClientCommand => Command.Create(SendToClient);
   

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

   #endregion

   #region Bindings_for_Migrations
   public string DriveItem
   {
      get => driveItem;
      set => Set(ref driveItem, value);
   }
   
   public int IndexDrive
   {
      get => indexDrive;
      set
      {
         Set(ref indexDrive, value);

         if (IndexDrive == -1)
         {
            IndexDrive = prevIndex;
            return;
         }
         
         if (IndexDrive != prevIndex)
         {
            int tempIndex = prevIndex;
            string tempPath = string.Empty;
            
            ClearPath(ref fullPath);
            tempPath = FullPath;
            
            prevIndex = IndexDrive;
            FullPath = Drives[IndexDrive];

            
            if (DirectoryInfo != null)
            {
               DirectoryInfo.Clear();
               DirectoryInfo = null;
            }
            
            DriveItem = Drives[IndexDrive];
            currentDrive = Drives[IndexDrive];
            Drives[tempIndex] = tempPath;
            ShowDirectoryInfo(Drives[IndexDrive]);
            isFirstDrive = true;
         }
      }
   }
   private void ClearPath(ref string thisPath)
   {
      string[] tempPathElements = thisPath.Split("\\").Take(1).ToArray();
      
      if (tempPathElements.Length == 1)
         thisPath = tempPathElements[0]+"\\";
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
      set
      {
         Set(ref indexPath, value);
         fullPath = DirectoryInfo[IndexPath];
      }
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
   private void ShowDriversInfo()
   {
      DriveInfo[] allDrives = DriveInfo.GetDrives();
      List<string> drives = new();
      foreach (DriveInfo drive in allDrives)
         drives.Add(drive.Name);
      Drives = new ObservableCollection<string>(drives);
      ShowDirectoryInfo(Drives[0]);
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
      DirectoryInfo = new ObservableCollection<string>(information);
   }
   
   /// <summary>
   /// Change the directory.
   /// Меняет директорию, переходя по ней.
   /// </summary>
   private void ChangeDirectory()
   {
      if (DirectoryInfo != null)
      {
         if(indexPath != -1)
            FullPath = DirectoryInfo[IndexPath];
      }
      
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
      DirectoryInfo = new ObservableCollection<string>(information);
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
      Drives[IndexDrive] = FullPath;
      DriveItem = Drives[IndexDrive];
      ShowDirectoryInfo(FullPath);
   }
   

   #endregion
}
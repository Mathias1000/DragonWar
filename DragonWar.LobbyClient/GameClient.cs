
using DragonWar.LobbyClient.Config;
using DragonWar.LobbyClient.Game;
using DragonWar.LobbyClient.Network;
using DragonWar.LobbyClient.Utils;
using DragonWar.Utils.ServerTask;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

public class GameClient
{

    public delegate void HandshakeHandler(ushort EncryptKey);

    public event HandshakeHandler HandShakeRecv;

    public void InvokeHandshake(ushort Key) => HandShakeRecv?.Invoke(Key);

    private LobbySession NetworkSession { get; set; }

    private bool IsOnline { get { return (MSession != null); } }

    public static GameClient Instance { get; set; }

    public string StartDirectory { get; internal set; }
    public string StartExecutable { get; internal set; }

    public GameTime CurrentTime { get; internal set; }
    public TimeSpan TotalUpTime { get; internal set; }

    private int ClientWorkerThreads = 2;

    private LobbySession MSession { get; set; }


    public LobbyPlayer PlayerInfo { get; set; }

    public TaskPool ThreadPool { get; private set; }


    public GameClient()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        HandShakeRecv += GameClient_HandShakeRecv;
    }

    private void GameClient_HandShakeRecv(ushort EncryptKey)
    {
        if(IsOnline)
        {
            MessageBox.Show("HandShake Recv");
        }
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        //TODO HANDLE CLIENT ERRORS
    }


    public void LogWarning(string Title, TextBlock Text, bool ShowWindows)
    {
        //TODO WArning Handle
    }

    public void Shutdown()
    {
        ThreadPool?.Dispose();
    }

    public bool LoadThreadPool()
    {

        ThreadPool = new TaskPool(ClientWorkerThreads);

        return AddRunTimeTasks();
    }

    private bool AddRunTimeTasks()
    {

        var TaskList = Reflector.GiveServerTasks();

        foreach (var mTask in TaskList)
        {
            try
            {
                var mU = (IServerTask)Activator.CreateInstance(mTask.Second);
                mU.Intervall = mTask.First;
                AddTask(mU);
            }
            catch
            {

                return false;
            }
        }
        return true;

    }

    public void AddTask(IServerTask mTask)
    {
        ThreadPool.QueueTask(mTask);
    }

   
    public bool ConnectToServer()
    {
        try
        {
            if (!IsOnline)
            {
                MSession = new LobbySession(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));

               

                MSession.TryConnectToLogin(LobbyClientConfiguration.Instance.ConnectInfo.ConnectIP, LobbyClientConfiguration.Instance.ConnectInfo.ConnectPort);
                MSession.StartRecv();

                if (MSession.IsConnected)
                {

                    return true;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }

    }

    [DllImport("Kernel32")]
    public static extern void AllocConsole();

    [DllImport("Kernel32")]
    public static extern void FreeConsole();

    public void ActivateConsole()
    {
        AllocConsole();

        Console.WriteLine("KOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
    }
}


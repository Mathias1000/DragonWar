using DragonWar.LobbyClient.Config;
using DragonWar.LobbyClient.GUI.Logic;
using DragonWar.Networking.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DragonWar.LobbyClient.GUI.Windows
{
    /// <summary>
    /// Interaktionslogik für StartForm.xaml
    /// </summary>
    public partial class StartForm : Window
    {
  
        public StartForm()
        {
            InitializeComponent();

            LoadClientDataContent();

        }

        private void LoadClientDataContent()
        {

            if (GameClient.Instance != null)
                throw new Exception("Canot Initlize GameClient Instance Effect");


            if(!LobbyClientConfiguration.Initialize())
            {
                MessageBox.Show("Failed to Load Config!");
                Environment.Exit(0);
            }

       
         
            DataContentLabel.Content = "Client Components";

            GameClient.Instance = new GameClient()
            {

                StartDirectory = AppDomain.CurrentDomain.BaseDirectory.ToEscapedString(),
                StartExecutable = (Assembly.GetEntryAssembly().CodeBase.Replace("file:///", "").Replace("/", "\\")),
                CurrentTime = (GameTime)DateTime.Now,
            };

            LoadExsternAssemblys();

            if (LobbyClientConfiguration.Instance.Debug)
                GameClient.Instance.ActivateConsole();

                if (!LoadModules(LobbyModuleType.Client) || ! GameClient.Instance.LoadThreadPool())
            {
                MessageBox.Show("Failed to Load Client Modules!");
                Environment.Exit(0);
            }

            DataContentLabel.Content = "Network Components";

            LobbyHandlerStore.Initialize();

            if(!LoadModules(LobbyModuleType.Network))
            {
                MessageBox.Show("Failed to Load Network Modules!");
                Environment.Exit(0);
            }
            
            //here contentloading.

            FinishLoadContent();//tmp

        }

        private void LoadExsternAssemblys()
        {
            Assembly.Load(@"DragonWar.Utils");
            Assembly.Load(@"DragonWar.Game");
            Assembly.Load(@"DragonWar.Networking");
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FinishLoadContent()
        {
            DataContentLabel.Content = "Login";
            new PageSwitcher().Show();
            this.Close();
        }



        public bool LoadModules(LobbyModuleType Type)
        {
            try
            {
                if (!ClientReflector.GetInitializerLobbyModulesMethods(Type).Any(method => !method.Invoke()))
                {
                    return true;
                }

            }
            catch
            {
                return false;
            }

            return true;
        }
    
    }
}

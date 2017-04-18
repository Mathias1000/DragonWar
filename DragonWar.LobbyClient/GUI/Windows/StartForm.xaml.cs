using DragonWar.LobbyClient.GUI.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //here contentloading.
 
            FinishLoadContent();//tmp
       
        }

        private void FinishLoadContent()
        {
            new PageSwitcher().Show();
            this.Close();
        }
 
    }
}

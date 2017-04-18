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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DragonWar.LobbyClient.GUI.Controls
{
    /// <summary>
    /// Interaktionslogik für LoginPanel.xaml
    /// </summary>
    public partial class LoginPanel : UserControl,ISwitchable
    {
        public LoginPanel()
        {
            InitializeComponent();
        }

        public void UtilizeState(object state)
        {
            MessageBox.Show("test util");
        }
    }
}

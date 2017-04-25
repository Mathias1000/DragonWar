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

        private void AccountInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoginButtonLogic();
        }

    
        private void PasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            LoginButtonLogic();
        }

        private void LoginButtonLogic()
        {
            if(PasswordInput.Password.Length > 0 
                && AccountInput.Text.Length > 0 
                && !Login.IsEnabled)
            {
                Login.IsEnabled = true;
            }
            else if(PasswordInput.Password.Length == 0 || AccountInput.Text.Length == 0)
            {
                Login.IsEnabled = false;
            }
        }

        private void AccountInput_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && Login.IsEnabled)
            {
                LoginAck();
            }
        }


        private void PasswordInput_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && Login.IsEnabled)
            {
                LoginAck();
            }
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginAck();
        }
        private void LoginAck()
        {
            GameClient.Instance.ConnectToServer();
       
           
        }

  
    }
}

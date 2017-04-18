using DragonWar.LobbyClient.GUI.Logic;
using System.Windows.Controls;

public static class Switcher
{
    public static PageSwitcher pageSwitcher;

    public static void Switch(UserControl newPage)
    {
        pageSwitcher.Navigate(newPage);
    }

    public static void Switch(UserControl newPage, object state)
    {
        pageSwitcher.Navigate(newPage, state);
    }
}
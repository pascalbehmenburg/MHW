
using System;
using System.Data;
using System.Windows;

namespace MHW_Save_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        
        public void EditSteamLabel(object sender, EventArgs e)
        {
            if (saveFile == null) return;
            string steamstring;
            InputBox inputDialog = new InputBox("Enter replacement steam ID:", saveFile.ReadSteamID().ToString());
            if (inputDialog.ShowDialog() == true)
            {
                steamstring = inputDialog.Answer;
                try
                {
                    long steamid = Convert.ToInt64(steamstring);
                    saveFile.setSteamID(steamid);
                    GeneralTabControl.SteamId = "Steam ID: "+ steamid;
                }
                catch
                {
                    MessageBox.Show("Invalid Steam ID", "Invalid Steam ID", MessageBoxButton.OK);
                }
            }
        }
    }
}
        
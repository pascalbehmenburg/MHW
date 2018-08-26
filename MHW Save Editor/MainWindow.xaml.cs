using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace MHW_Save_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SaveFile saveFile;
        private MemoryStream data;

        public MainWindow()
        {
            InitializeComponent();
            data = new MemoryStream();
        }

        private void BackupButton_Click(object sender, RoutedEventArgs e)
        {
            string steamPath = Utility.getSteamPath();
            string backupPath = steamPath + "\\backups";

            if (!Directory.Exists(backupPath))
                Directory.CreateDirectory(steamPath + "\\backups");

            string date_and_time = DateTime.Now.ToString("MM\\_dd\\_yyyy\\_h\\_mm");

            SaveFile tempFile = new SaveFile(File.ReadAllBytes(Utility.getSteamPath() + "\\SAVEDATA1000"));
            tempFile.Save(backupPath + "\\SAVEDATA1000_" + date_and_time);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            string steamPath = Utility.getSteamPath();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            saveFile = new SaveFile(File.ReadAllBytes(openFileDialog.FileName));

            StatusLabel.Content = "Decrypted: " + saveFile.isDecrypted();
            SizeLabel.Content = "Size: " + saveFile.FileSize().ToString() + " byte";
            SteamIDLabel.Content = "Steam ID: " + saveFile.getSteamID();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.ShowDialog();
            saveFileDialog.InitialDirectory = Utility.getSteamPath();
            
            if (MessageBox.Show("Encrypt the file?", "Save", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                saveFile.Encrypt();

            saveFile.Save(saveFileDialog.FileName);

            MessageBox.Show("File saved.", "Save", MessageBoxButton.OK);
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using MHW_Save_Editor.src.FileFormat;
using Microsoft.Win32;

namespace MHW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SaveFile saveFile;
        private GenericFile genericFile;
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
            tempFile.Encrypt();
            tempFile.Save(backupPath + "\\SAVEDATA1000_" + date_and_time, false);
            MessageBox.Show("File backup created.", "Backup", MessageBoxButton.OK);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            string steamPath = Utility.getSteamPath();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            
            //to-do cleanup
            switch (Path.GetExtension(openFileDialog.FileName))
            {
                case "":
                    saveFile = new SaveFile(File.ReadAllBytes(openFileDialog.FileName));
                    StatusLabel.Content = "Decrypted: " + saveFile.isDecrypted();
                    SizeLabel.Content = "Size: " + saveFile.FileSize().ToString() + " byte";
                    SteamIDLabel.Content = "Steam ID: " + saveFile.ReadSteamID();
                    ChecksumLabel.Content = "Checksum: " + saveFile.GetChecksum();
                    break;
                case ".mib":
                    genericFile = new GenericFile(File.ReadAllBytes(openFileDialog.FileName), "TZNgJfzyD2WKiuV4SglmI6oN5jP2hhRJcBwzUooyfIUTM4ptDYGjuRTP");
                    genericFile.Decrypt();
                    StatusLabel.Content = "Decrypted: " + "unsupported file format";
                    SizeLabel.Content = "Size: " + File.ReadAllBytes(openFileDialog.FileName).Length + " byte";
                    SteamIDLabel.Content = "Steam ID: " + "unsupported file format";
                    ChecksumLabel.Content = "Checksum: " + "unsupported file format";
                    break;
                case ".itlot":
                    genericFile = new GenericFile(File.ReadAllBytes(openFileDialog.FileName), "D7N88VEGEnRl0HEHTO0xMQkbeMb37arJF488lREp90WYojAONkLoxfMt");
                    genericFile.Decrypt();
                    StatusLabel.Content = "Decrypted: " + "unsupported file format";
                    SizeLabel.Content = "Size: " + File.ReadAllBytes(openFileDialog.FileName).Length + " byte";
                    SteamIDLabel.Content = "Steam ID: " + "unsupported file format";
                    ChecksumLabel.Content = "Checksum: " + "unsupported file format";
                    break;
            }

            
            FilePathLabel.Content = openFileDialog.FileName;
            MessageBox.Show("File loaded.", "Load", MessageBoxButton.OK);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.ShowDialog();
            saveFileDialog.InitialDirectory = Utility.getSteamPath();

            //to-do clean that up + move messageboxes to encrypt/decrypt functions -.-
            if (saveFile != null && !saveFile.validChecksum)
            {
                if (MessageBox.Show("The checksum is invalid. Should it be fixed?", "Checksum", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (MessageBox.Show("Encrypt the file?", "Save", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        saveFile.Encrypt();
                    saveFile.Save(saveFileDialog.FileName, true);
                } else
                {
                    if (MessageBox.Show("Encrypt the file?", "Save", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        saveFile.Encrypt();
                    saveFile.Save(saveFileDialog.FileName, false);
                }
            } else if (saveFile != null)
            {
                if (MessageBox.Show("Encrypt the file?", "Save", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    saveFile.Encrypt();
                saveFile.Save(saveFileDialog.FileName, false);
            } else
            {
                genericFile.Encrypt();
                genericFile.Save(saveFileDialog.FileName);
            }
           
            MessageBox.Show("File saved.", "Save", MessageBoxButton.OK);
        }
    }
}

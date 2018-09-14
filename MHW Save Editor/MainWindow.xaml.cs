using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
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

        private void BackupFunction(object sender, RoutedEventArgs e)
        {
            string steamPath = Utility.getSteamPath();
            string backupPath = steamPath + "\\backups";

            if (!Directory.Exists(backupPath))
                Directory.CreateDirectory(steamPath + "\\backups");

            string date_and_time = DateTime.Now.ToString("MM\\_dd\\_yyyy\\_h\\_mm");

            SaveFile tempFile = new SaveFile(File.ReadAllBytes(Utility.getSteamPath() + "\\SAVEDATA1000"));
            tempFile.Encrypt();
            tempFile.Save(backupPath + "\\SAVEDATA1000_" + date_and_time);
            MessageBox.Show("File backup created.", "Backup", MessageBoxButton.OK);
        }

        private void OpenFunction(object sender, RoutedEventArgs e)
        {
            string steamPath = Utility.getSteamPath();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            
            //to-do cleanup
            switch (Path.GetExtension(openFileDialog.FileName))
            {
                case ".bin":
                case "":
                    saveFile = new SaveFile(File.ReadAllBytes(openFileDialog.FileName));
                    SizeLabel.Content = "Size: " + saveFile.FileSize() + " byte";
                    SteamIdLabel.Content = saveFile.ReadSteamID();
                    ChecksumLabel.Content = "Checksum: " + saveFile.GetChecksum();
                    break;
                case ".mib":
                    genericFile = new GenericFile(File.ReadAllBytes(openFileDialog.FileName), "TZNgJfzyD2WKiuV4SglmI6oN5jP2hhRJcBwzUooyfIUTM4ptDYGjuRTP");
                    genericFile.Decrypt();
                    SizeLabel.Content = "Size: " + File.ReadAllBytes(openFileDialog.FileName).Length + " byte";
                    ChecksumLabel.Content = "Checksum: " + "unsupported file format";
                    break;
                case ".itlot":
                    genericFile = new GenericFile(File.ReadAllBytes(openFileDialog.FileName), "D7N88VEGEnRl0HEHTO0xMQkbeMb37arJF488lREp90WYojAONkLoxfMt");
                    genericFile.Decrypt();
                    SizeLabel.Content = "Size: " + File.ReadAllBytes(openFileDialog.FileName).Length + " byte";
                    ChecksumLabel.Content = "Checksum: " + "unsupported file format";
                    break;
            }

            
            FilePathLabel.Content = openFileDialog.FileName;
        }

        private void SaveFunction(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Encrypted|*|Unencrypted (*.bin)|*.bin";
            saveFileDialog.InitialDirectory = Utility.getSteamPath();
            saveFileDialog.AddExtension = true;
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result==true){
                if (saveFile != null)
                {
                    saveFile.Save(saveFileDialog.FileName,!(Path.GetExtension(saveFileDialog.FileName)==".bin"));
                }
                else
                {
                    genericFile.Encrypt();
                    genericFile.Save(saveFileDialog.FileName);
                }
            }
            MessageBox.Show("File saved.", "Save", MessageBoxButton.OK);
        }

        public void EditSteamLabel(object sender, RoutedEventArgs e)
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
                    SteamIdLabel.Content = steamid;
                }
                catch
                {
                    MessageBox.Show("Invalid Steam ID", "Invalid Steam ID", MessageBoxButton.OK);
                }
            }
        }
    }
    


    
}

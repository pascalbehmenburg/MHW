using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MHW_Save_Editor.FileFormat;
using MHW_Save_Editor.InvestigationEditing;
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
        private bool open;
        
        public MainWindow()
        {
            data = new MemoryStream();
            InitializeComponent();
            DataContext = this;
        }


        private void OpenFunction(object sender, RoutedEventArgs e)
        {
            open = true;
            string steamPath = Utility.getSteamPath();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName == "") return;
            saveFile = new SaveFile(File.ReadAllBytes(openFileDialog.FileName));
            GeneralTabControl.Size = "Size: " + saveFile.FileSize() + " byte";
            GeneralTabControl.SteamId = "Steam ID: " + saveFile.ReadSteamID();
            GeneralTabControl.Checksum = "Checksum: " + saveFile.GetChecksum();
            GeneralTabControl.OnFileChecksum = "ChecksumGenerated: " + BitConverter.ToString(saveFile.GenerateChecksum()).Replace("-","");
            GeneralTabControl.FilePath = openFileDialog.FileName;
            //investigations.Populate(saveFile.data);
            
            
        }

        private void SaveFunction(object sender, RoutedEventArgs e)
        {
            if (!open) return;
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Encrypted|*|Unencrypted (*.bin)|*.bin";
            saveFileDialog.InitialDirectory = Utility.getSteamPath();
            saveFileDialog.AddExtension = true;
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result==true){
                if (saveFile != null)
                {
                    saveFile.Save(saveFileDialog.FileName,!(Path.GetExtension(saveFileDialog.FileName)==".bin"));
                    MessageBox.Show("File saved.", "Save", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("No File to Save.", "Save", MessageBoxButton.OK);
                }
                
            }
        }
                
        private void BackupFunction(object sender, RoutedEventArgs e)
        {
            string steamPath = Utility.getSteamPath();
            string backupPath = steamPath + "\\backups";

            if (!Directory.Exists(backupPath))
                Directory.CreateDirectory(steamPath + "\\backups");

            string date_and_time = DateTime.Now.ToString("MM\\_dd\\_yyyy\\_h\\_mm");

            File.Copy((Utility.getSteamPath() + "\\SAVEDATA1000"), backupPath + "\\SAVEDATA1000_" + date_and_time, true);
            MessageBox.Show("File backup created.", "Backup", MessageBoxButton.OK);
        }

        void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                RaisePropertyChanged("EditOptions");
                RaisePropertyChanged("ToolsOptions");
            }
        }

        public ObservableCollection<string> EditOptions
        {
            get
            {
                switch(TabControl.SelectedIndex)
                {
                    case 1:
                        return new ObservableCollection<string>(InvestigationList_EditMenu);
                    default:
                        return new ObservableCollection<string>();
                }
            }
        }

        public ObservableCollection<string> ToolsOptions
        {
            get
            {
                switch (TabControl.SelectedIndex)
                {
                    case 1:
                        return new ObservableCollection<string>(InvestigationList_ToolsMenu);
                    default:
                        return new ObservableCollection<string>();
                }
            }
        }

        private void GeneralEditHandler(string any)
        {}
        private void GeneralToolsHandler(string any)
        {}
        
        private void EditHandlers(int switchvar, string command)
        {
            switch (switchvar)
            {
                case (0):
                    GeneralEditHandler(command);
                    break;
                case(1):
                    InvestigationsEditHandler(command);
                    break;
            }
        }

        private void ToolsHandlers(int switchvar, string command)
        {
            switch (switchvar)
            {
                case (0):
                    GeneralToolsHandler(command);
                    break;
                case (1):
                    InvestigationsToolsHandler(command);
                    break;
            }
        }


        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem  obMenuItem = e.OriginalSource as MenuItem ;
            EditHandlers(TabControl.SelectedIndex,obMenuItem.Header.ToString());
        }
        private void ToolsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem  obMenuItem = e.OriginalSource as MenuItem ;
            ToolsHandlers(TabControl.SelectedIndex,obMenuItem.Header.ToString());
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
}

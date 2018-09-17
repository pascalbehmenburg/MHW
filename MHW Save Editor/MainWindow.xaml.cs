using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MHW.InvestigationEditing;
using MHW_Save_Editor.src.FileFormat;
using Microsoft.Win32;

namespace MHW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private SaveFile saveFile;
        private GenericFile genericFile;
        private MemoryStream data;
        InvestigationList investigations;
        InvestigationViewModel currentInvestigation;
        public string testString = "Test";
        
        public MainWindow()
        {
            data = new MemoryStream();
            investigations = new InvestigationList();
            currentInvestigation = investigations.Expose();
            InitializeComponent();
            //DataContext = this;
            SetBindings();
        }

        private void SetBindings()
        {
            InvestigationVisibleList.ItemsSource  = investigations.InvestigationCollection;
            CurrentInvestigationVisible.DataContext = currentInvestigation;
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
            
            switch (Path.GetExtension(openFileDialog.FileName))
            {
                case ".bin":
                case "":
                    if (openFileDialog.FileName == "") return;
                    saveFile = new SaveFile(File.ReadAllBytes(openFileDialog.FileName));
                    SizeLabel.Content = "Size: " + saveFile.FileSize() + " byte";
                    SteamIdLabel.Content = "Steam ID: " + saveFile.ReadSteamID();
                    ChecksumLabel.Content = "Checksum: " + saveFile.GetChecksum();
                    investigations.Populate(saveFile.data);
                    SetBindings();
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
                MessageBox.Show("File saved.", "Save", MessageBoxButton.OK);
            }
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

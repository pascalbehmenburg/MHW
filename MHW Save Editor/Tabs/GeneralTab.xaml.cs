using System;
using System.Windows;
using System.Windows.Controls;

namespace MHW_Save_Editor
{
    public partial class GeneralTab : UserControl
    {
        public GeneralTab()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event EventHandler SteamLabelDoubleClicked;
        public void EditSteamLabel(object sender, RoutedEventArgs args)
        {
            if (SteamLabelDoubleClicked != null)
                SteamLabelDoubleClicked(this, args);
        }
        
        public string Checksum
        {
            get { return (string)GetValue(ChecksumProperty); }
            set { SetValue(ChecksumProperty, value); }
        }

        public static readonly DependencyProperty ChecksumProperty =
            DependencyProperty.Register("Checksum", typeof(string), typeof(GeneralTab), new UIPropertyMetadata(""));
        
        public string OnFileChecksum
        {
            get { return (string)GetValue(OnFileChecksumProperty); }
            set { SetValue(OnFileChecksumProperty, value); }
        }

        public static readonly DependencyProperty OnFileChecksumProperty =
            DependencyProperty.Register("OnFileChecksum", typeof(string), typeof(GeneralTab), new UIPropertyMetadata(""));
        public string SteamId
        {
            get { return (string)GetValue(SteamIdProperty); }
            set { SetValue(SteamIdProperty, value); }
        }

        public static readonly DependencyProperty SteamIdProperty =
            DependencyProperty.Register("SteamId", typeof(string), typeof(GeneralTab), new UIPropertyMetadata(""));
        public string Size
        {
            get { return (string)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(string), typeof(GeneralTab), new UIPropertyMetadata(""));
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(GeneralTab), new UIPropertyMetadata(""));
        
    }
}

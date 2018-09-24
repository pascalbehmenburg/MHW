using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace MHW_Save_Editor
{
    public partial class Boxstore : UserControl
    {
        public Boxstore()
        {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.DataContext = new BoxstoreViewModel();
            }
        }
    }
}

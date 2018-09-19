using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
namespace MHW_Save_Editor
{
    public partial class InvestigationView : UserControl
    {
        public InvestigationView()
        {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.DataContext = new InvestigationViewModel();
            }
        }
    }
}
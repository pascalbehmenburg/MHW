using System.Windows;
using System.Windows.Data;
using MHW_Save_Editor.Data;
using MHW_Save_Editor.InvestigationEditing;

namespace MHW_Save_Editor
{
    public class BoxstoreViewModel : NotifyUIBase
    {
        public ListCollectionView InvestigationCollectionView {get; set;}
        private Investigation CurrentInvestigation
        {
            get { return InvestigationCollectionView.CurrentItem as Investigation; }
            set
            {
                InvestigationCollectionView.MoveCurrentTo(value);
                RaisePropertyChanged();

            }
        }
        public BoxstoreViewModel()
        {
            InvestigationCollectionView = Application.Current.Resources["InvestigationCollectionView"] as ListCollectionView;
            InvestigationCollectionView.MoveCurrentToPosition(1);
        }
    }
}
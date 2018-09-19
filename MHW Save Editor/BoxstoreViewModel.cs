using System.Windows;
using System.Windows.Data;
using MHW_Save_Editor.Data;
using MHW_Save_Editor.InvestigationEditing;

namespace MHW_Save_Editor
{
    public class BoxstoreViewModel : NotifyUIBase
    {
        public ListCollectionView BoxstoreCollectionView {get; set;}
        private Investigation CurrentInvestigation
        {
            get { return BoxstoreCollectionView.CurrentItem as Investigation; }
            set
            {
                BoxstoreCollectionView.MoveCurrentTo(value);
                RaisePropertyChanged();

            }
        }
        public BoxstoreViewModel()
        {
            BoxstoreCollectionView = Application.Current.Resources["PeopleCollectionView"] as ListCollectionView;
            BoxstoreCollectionView.MoveCurrentToPosition(1);
        }
    }
}
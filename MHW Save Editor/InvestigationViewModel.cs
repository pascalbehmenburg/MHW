using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using MHW_Save_Editor.Data;
using MHW_Save_Editor.InvestigationEditing;

namespace MHW_Save_Editor
{
    public class InvestigationViewModel : NotifyUIBase
    {
        public ListCollectionView BoxstoreCollectionView { get; set; }

        private Investigation CurrentInvestigation
        {
            get { return BoxstoreCollectionView.CurrentItem as Investigation; }
            set
            {
                BoxstoreCollectionView.MoveCurrentTo(value);
                RaisePropertyChanged();

            }
        }
        public InvestigationViewModel()
        {
            BoxstoreCollectionView = Application.Current.Resources["BoxstoreCollectionView"] as ListCollectionView;
            FirstRecord = new RelayCommand(() => FirstRecordExecute());
            NextRecord = new RelayCommand(() => NextRecordExecute());
            NextRecord = new RelayCommand(() => ToggleRecordExecute());
            PreviousRecord = new RelayCommand(() => PreviousRecordExecute());
            LastRecord = new RelayCommand(() => LastRecordExecute());
        }
        public RelayCommand FirstRecord { get; set; }
        public RelayCommand NextRecord { get; set; }
        public RelayCommand ToggleRecord { get; set; }
        public RelayCommand PreviousRecord { get; set; }
        public RelayCommand LastRecord { get; set; }
        public void FirstRecordExecute()
        {
            BoxstoreCollectionView.MoveCurrentToFirst();
        }
        public void PreviousRecordExecute()
        {
            BoxstoreCollectionView.MoveCurrentToPrevious();
        }
        public void ToggleRecordExecute()
        {
            //TODO - Actually write the method
        }
        public void NextRecordExecute()
        {
            BoxstoreCollectionView.MoveCurrentToNext();
        }
        public void LastRecordExecute()
        {
            BoxstoreCollectionView.MoveCurrentToLast();
        }
    }
}
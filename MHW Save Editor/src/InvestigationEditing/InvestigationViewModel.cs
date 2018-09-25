using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using MHW_Save_Editor.Data;
using MHW_Save_Editor.InvestigationEditing;

namespace MHW_Save_Editor
{
    public class InvestigationViewModel : NotifyUIBase
    {
        public ListCollectionView InvestigationCollectionView { get; set; }

        private Investigation CurrentInvestigation
        {
            get { return InvestigationCollectionView.CurrentItem as Investigation; }
            set
            {
                InvestigationCollectionView.MoveCurrentTo(value);
                RaisePropertyChanged();

            }
        }
        public InvestigationViewModel()
        {
            InvestigationCollectionView = Application.Current.Resources["InvestigationCollectionView"] as ListCollectionView;
            FirstRecord = new RelayCommand(() => FirstRecordExecute());
            NextRecord = new RelayCommand(() => NextRecordExecute());
            ToggleRecord = new RelayCommand(() => ToggleRecordExecute());
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
            InvestigationCollectionView?.MoveCurrentToFirst();
        }
        public void PreviousRecordExecute()
        {
            if (InvestigationCollectionView?.CurrentPosition>0)
                InvestigationCollectionView.MoveCurrentToPrevious();
        }
        public void ToggleRecordExecute()
        {
            var col = (ListCollectionView) Application.Current.Resources["InvestigationCollectionView"];
            var listing = ((IList<Investigation>) col.SourceCollection);
            int index = col.CurrentPosition;
            listing[index] = listing[index].Filled?new Investigation(InvestigationThinLayer.nullinvestigation):new Investigation(InvestigationThinLayer.defaultinvestigation);
        }
        
        public void NextRecordExecute()
        {
            if (InvestigationCollectionView?.CurrentPosition+1<Investigation.inv_number)InvestigationCollectionView.MoveCurrentToNext();
        }
        public void LastRecordExecute()
        {
            InvestigationCollectionView?.MoveCurrentToLast();
        }
    }
}
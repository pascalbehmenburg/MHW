using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MHW.InvestigationEditing;

namespace MHW
{
    public partial class MainWindow
    {
        public void InvestigationList_First(object sender, RoutedEventArgs e)
        {
            investigations.First();
            currentInvestigation = investigations.Expose();
            InvestigationVisibleList.SelectedIndex = investigations.CurrentIndex;
        }
        public void InvestigationList_Prev(object sender, RoutedEventArgs e)
        {
            investigations.Prev();
            currentInvestigation = investigations.Expose();
            InvestigationVisibleList.SelectedIndex = investigations.CurrentIndex;
        }

        public void InvestigationList_ToggleCurrent(object sender, RoutedEventArgs e)
        {
            investigations.Toggle();
            currentInvestigation = investigations.Expose();
            InvestigationVisibleList.SelectedIndex = investigations.CurrentIndex;
        }
        public void InvestigationList_Next(object sender, RoutedEventArgs e)
        {
            investigations.Next();
            currentInvestigation = investigations.Expose();
            InvestigationVisibleList.SelectedIndex = investigations.CurrentIndex;
        }
        public void InvestigationList_Last(object sender, RoutedEventArgs e)
        {
            investigations.Last();
            currentInvestigation = investigations.Expose();
            InvestigationVisibleList.SelectedIndex = investigations.CurrentIndex;

        }
        public void InvestigationList_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ListBox listbox = sender as ListBox;
            investigations.Seek(listbox.SelectedIndex);
            currentInvestigation = investigations.Expose();
        }        
        
        
    }

    
}
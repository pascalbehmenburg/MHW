using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
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
            CurrentInvestigationVisible.DataContext = currentInvestigation;
        }

        public void InvestigationList_Prev(object sender, RoutedEventArgs e)
        {
            investigations.Prev();
            currentInvestigation = investigations.Expose();
            InvestigationVisibleList.SelectedIndex = investigations.CurrentIndex;
            CurrentInvestigationVisible.DataContext = currentInvestigation;
        }

        public void InvestigationList_ToggleCurrent(object sender, RoutedEventArgs e)
        {
            investigations.Toggle();
            currentInvestigation = investigations.Expose();
            InvestigationVisibleList.SelectedIndex = investigations.CurrentIndex;
            CurrentInvestigationVisible.DataContext = currentInvestigation;
        }

        public void InvestigationList_Next(object sender, RoutedEventArgs e)
        {
            investigations.Next();
            currentInvestigation = investigations.Expose();
            InvestigationVisibleList.SelectedIndex = investigations.CurrentIndex;
            CurrentInvestigationVisible.DataContext = currentInvestigation;
        }

        public void InvestigationList_Last(object sender, RoutedEventArgs e)
        {
            investigations.Last();
            currentInvestigation = investigations.Expose();
            InvestigationVisibleList.SelectedIndex = investigations.CurrentIndex;
            CurrentInvestigationVisible.DataContext = currentInvestigation;

        }

        public void InvestigationList_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ListBox listbox = sender as ListBox;
            investigations.Seek(listbox.SelectedIndex);
            currentInvestigation = investigations.Expose();
            CurrentInvestigationVisible.DataContext = currentInvestigation;
        }
      
        private static string[] InvestigationList_EditMenu = new[]
        {
            "Copy", "Paste", "Paste At", "Commit All", "Sort", "Filter"
        };

        private void InvestigationsEditHandler(string command)
        {
            switch (command)
            {
                case("Copy"):
                    investigations.Copy();
                    break;
                case("Paste"):
                    investigations.Paste();
                    break;
                case("Paste At"):
                    int[] positions = PromptPositions();
                    investigations.PasteAt(positions);
                    break;
                case("Commit All"):
                    investigations.Commit();
                    break;
                case("Sort"):
                    Func<InvestigationViewModel,int > sorter = PromptInvestigationsSort();
                    investigations.ReSort(sorter);
                    break;
                case("Filter"):
                    Func<InvestigationViewModel,bool > filterer = PromptInvestigationsFilter();
                    investigations.ClearAll(filterer);
                    break;
                default:
                    return;
            }
        }

        private static string[] InvestigationList_ToolsMenu = new[]
        {
            "Import", "Export", "Import At", "Export At", "Prepend", "Generate Log File"
        };
        
        private void InvestigationsToolsHandler(string command)
        {
            string inputfile = "";
            bool accepted;
            switch (command)
            {
                case "Import":
                case "Import At":
                case "Prepend":
                    accepted=PromptInvestigationsInputFile(ref inputfile);
                    break;
                default:
                    accepted=PromptInvestigationsOutputFile(ref inputfile);
                    break;
            }
            if (accepted)
            switch (command)
            {
                case "Import":
                    investigations.Import(inputfile);
                    break;
                case "Export":
                    investigations.Export(inputfile);
                    break;
                case "Import At":
                {
                    int[] positions = PromptPositions();
                    investigations.ImportAt(inputfile, positions);
                    break;
                }
                case "Export At":
                {
                    int[] positions = PromptPositions();
                    investigations.ExportAt(inputfile, positions);
                    break;
                }
                case "Prepend":
                {
                    investigations.Prepend(inputfile);
                    break;
                }
                case "Generate Log File":
                    investigations.GenerateLog(inputfile);
                    break;
                default:
                    return;
            }
        }
    }
}
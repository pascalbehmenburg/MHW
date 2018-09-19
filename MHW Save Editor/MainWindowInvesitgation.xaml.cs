using System;
using MHW_Save_Editor.InvestigationEditing;

namespace MHW_Save_Editor
{
    public partial class MainWindow
    {

      
        private static string[] InvestigationList_EditMenu = new[]
        {
            "Copy", "Paste", "Paste At", "Commit All", "Sort", "Filter"
        };

        private void InvestigationsEditHandler(string command)
        {
            switch (command)
            {
                case("Copy"):
                    //investigations.Copy();
                    break;
                case("Paste"):
                    //investigations.Paste();
                    break;
                case("Paste At"):
                    int[] positions = PromptPositions();
                    //investigations.PasteAt(positions);
                    break;
                case("Commit All"):
                    //investigations.Commit();
                    break;
                case("Sort"):
                    Func<Investigation,int > sorter = PromptInvestigationsSort();
                    //investigations.ReSort(sorter);
                    break;
                case("Filter"):
                    Func<Investigation,bool > filterer = PromptInvestigationsFilter();
                    //investigations.ClearAll(filterer);
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
                    //investigations.Import(inputfile);
                    break;
                case "Export":
                    //investigations.Export(inputfile);
                    break;
                case "Import At":
                {
                    int[] positions = PromptPositions();
                    //investigations.ImportAt(inputfile, positions);
                    break;
                }
                case "Export At":
                {
                    int[] positions = PromptPositions();
                    //investigations.ExportAt(inputfile, positions);
                    break;
                }
                case "Prepend":
                {
                    //investigations.Prepend(inputfile);
                    break;
                }
                case "Generate Log File":
                    //investigations.GenerateLog(inputfile);
                    break;
                default:
                    return;
            }
        }
    }
}
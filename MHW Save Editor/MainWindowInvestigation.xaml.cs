using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using MHW_Save_Editor.InvestigationEditing;
using MHW_Save_Editor.Tabs;

namespace MHW_Save_Editor
{
    public partial class MainWindow
    {
        private Investigation _clipboard;
        private object PopulateInvestigations(byte[] newdata)
        {
            List<Investigation> inv = GetInvestigations(
                newdata.Slice(Investigation.inv_offsets[0], Investigation.inv_offsets[0]+Investigation.inv_size*Investigation.inv_number));
            ObservableCollection<Investigation> boxstore = new ObservableCollection<Investigation>(inv);
            ListCollectionView investigationCollectionView = (ListCollectionView)new CollectionViewSource { Source = boxstore }.View;
            Application.Current.Resources["InvestigationCollectionView"] = investigationCollectionView;
            return new InvestigationsTab();
        }

        private List<Investigation> GetInvestigations(byte[] newdata)
        {
            List<Investigation> inv = new List<Investigation>();
            int invcount = newdata.Length / Investigation.inv_size;
            for (int i = 0; i < invcount; i++)
            {
                inv.Add(new Investigation(newdata.Slice(i*Investigation.inv_size, (i+1)*Investigation.inv_size)));
            }
            return inv;
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
                    Copy();
                    break;
                case("Paste"):
                    Paste();
                    break;
                case("Paste At"):
                    List<int> positions = PromptPositions();
                    PasteAt(positions);
                    break;
                case("Commit All"):
                    Commit();
                    break;
                case("Sort"):
                    Func<Investigation,IEnumerable<int> > sorter = PromptInvestigationsSorter();
                    ReSort(sorter);//sorter);
                    break;
                case("Filter"):
                    Func<Investigation,bool > filterer = PromptInvestigationsFilter();
                    ClearAll(filterer);
                    break;
                default:
                    return;
            }
        }

        private static string[] InvestigationList_ToolsMenu = new[]
        {
            "Import", "Export", "Import At", "Export At", "Prepend", "Generate Log File", "Generate CSV File"
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
                    Import(inputfile);
                    break;
                case "Export":
                    Export(inputfile);
                    break;
                case "Import At":
                {
                    List<int> positions = PromptPositions();
                    ImportAt(inputfile, positions);
                    break;
                }
                case "Export At":
                {
                    List<int> positions = PromptPositions();
                    ExportAt(inputfile, positions);
                    break;
                }
                case "Prepend":
                    Prepend(inputfile);
                    break;
                case "Generate Log File":
                    GenerateLog(inputfile);
                    break;
                case "Generate CSV File":
                    GenerateCSV(inputfile);
                    break;
                default:
                    return;
            }
        }

        public int CurrentIndex()
        {
            return ((ListCollectionView) Application.Current.Resources["InvestigationCollectionView"]).CurrentPosition;
        }

        private void Copy()
        {
            _clipboard = (Investigation) ((ListCollectionView) Application.Current.Resources["InvestigationCollectionView"]).CurrentItem;
        }

        private void Paste()
        {
            PasteAt(new List<int>(new []
                {((ListCollectionView) Application.Current.Resources["InvestigationCollectionView"]).CurrentPosition+1}
            ));

        }
    
        private void PasteAt(List<int> positions)
        {
            foreach (int j in positions){
                ((IList<Investigation>)((ListCollectionView) Application.Current.Resources["InvestigationCollectionView"]).SourceCollection)
                [j-1]
                = new Investigation(_clipboard.Serialize());
            }
        }
    
        private void Commit()
        {
            for (int i = 0; i < Investigation.inv_number; i++)
            {
                ((IList<Investigation>) ((ListCollectionView) Application.Current.Resources[
                        "InvestigationCollectionView"]).SourceCollection)
                    [i].Serialize().CopyTo(saveFile.data, Investigation.inv_offsets[0]+i*Investigation.inv_size);
            }
        }

        private void ReSort(Func<Investigation, IEnumerable<int>> sorterer = null)
        {
            IList<Investigation> list = (IList<Investigation>) ((ListCollectionView) Application.Current.Resources[
                "InvestigationCollectionView"]).SourceCollection;
            if (sorterer == null)
            {
                int j = 0;
                for (int i = 0; i < Investigation.inv_number - j;)
                {
                    if (list[i].Filled) i++;
                    else
                    {
                        list.RemoveAt(i);
                        list.Add(new Investigation(InvestigationThinLayer.nullinvestigation));
                        j++;
                    }
                }
            }
            else
            {
                try
                {
                    List<int> valuation = Enumerable.Range(0, Investigation.inv_number)
                        .OrderByDescending(x=>sorterer(list[x]), new SequentialCompararer()).ToList();
                    list.ParallelSort(valuation);
                }
                catch
                {
                    MessageBox.Show("Supplied expression failed.", "Error", MessageBoxButton.OK);
                }
            }
        }

        private void ClearAll(Func<Investigation, bool> filter)
        {
            IList<Investigation> list = (IList<Investigation>) ((ListCollectionView) Application.Current.Resources[
                "InvestigationCollectionView"]).SourceCollection;
            if (!(filter == null))
            {
                try
                {
                    for (int i = 0; i < Investigation.inv_number; i++)
                    {
                        Investigation inv = list[i];
                        if (inv.Filled && filter(inv))
                        {
                            list[i] = new Investigation(InvestigationThinLayer.nullinvestigation);
                        }
                    }

                    ReSort();
                }
                catch
                {
                    MessageBox.Show("Supplied expression failed.", "Error", MessageBoxButton.OK);
                }
                
            }
        }

        public void Import(string filepath)
        {
            ImportAt(filepath, new List<int>(new [] {CurrentIndex()+1} ) );
        }
        public void Export(string filepath)
        {
            ExportAt(filepath, new List<int>(new [] {CurrentIndex()+1} ) );
        }
        public void ImportAt(string filepath, List<int> positions)//Positions are in 1 index, need to convert to 0-index
        {
            byte[] import = File.ReadAllBytes(filepath);
            if ((import.Length % Investigation.inv_size)!=0){MessageBox.Show("File is not a collection of 42-byte investigations", "Error", MessageBoxButton.OK);return;}
            if ((import.Length / Investigation.inv_size)!=positions.Count){MessageBox.Show("Number of investigations does not match number of replacements", "Error", MessageBoxButton.OK);return;}
            for (int i = 0; i < import.Length / Investigation.inv_size; i++)
            {
                ((IList<Investigation>) ((ListCollectionView) Application.Current.Resources["InvestigationCollectionView"]).SourceCollection)
                    [positions[i]-1]= new Investigation(import.Slice(i * Investigation.inv_size, i * Investigation.inv_size+Investigation.inv_size));
            }
        }
        
        public void ExportAt(string filepath, List<int> positions)
        {
            byte[] content = new byte[Investigation.inv_size*positions.Count];
            for (int i =0; i<positions.Count; i++)
            {
                ((IList<Investigation>) ((ListCollectionView) Application.Current.Resources["InvestigationCollectionView"]).SourceCollection)
                    [positions[i]-1].Serialize().CopyTo(content, i * Investigation.inv_size);
            }
            File.WriteAllBytes(filepath,content);
        }
        
        public void Prepend(string filepath)
        {
            
            List<Investigation> moreItems = GetInvestigations(File.ReadAllBytes(filepath));
            moreItems.Reverse();
            var list =
                (IList<Investigation>) ((ListCollectionView) Application.Current.Resources[
                    "InvestigationCollectionView"]).SourceCollection;
            foreach (Investigation item in moreItems)
            {
                list.Insert(0,item);
                list.RemoveAt(Investigation.inv_number);
            }
        }
        
        public void GenerateLog(string filepath)
        {
            var builder = new StringBuilder();
            foreach (Investigation inv in (IList<Investigation>) ((ListCollectionView) Application.Current.Resources["InvestigationCollectionView"]).SourceCollection)
            {
                builder.AppendLine(inv.Log());
            }

            File.WriteAllText(filepath, builder.ToString());
        }
        
        public void GenerateCSV(string filepath)
        {
            var builder = new StringBuilder();
            
            builder.AppendLine("Index"+","+Investigation.CSVHeader);
            for (int i = 0; i < Investigation.inv_number; i++)
            {
                Investigation inv =
                    ((IList<Investigation>) ((ListCollectionView) Application.Current.Resources[
                        "InvestigationCollectionView"]).SourceCollection)[i];
                builder.AppendLine(i+","+inv.LogCSV());
            }
            if(string.IsNullOrEmpty(Path.GetExtension(filepath)))filepath += ".csv";
            File.WriteAllText(filepath, builder.ToString());
        }
        
    }
}
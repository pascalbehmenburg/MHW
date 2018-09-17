using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Windows;
using MHW.InvestigationEditing;
using Microsoft.Win32;

namespace MHW
{
    public partial class MainWindow
    {
        private int[] PromptPositions()
        {
            string steamstring;
            InputBox inputDialog = new InputBox("Enter indexes to excecute the operation separated by commas:", "");
            int[] positions = new int [0];
            if (inputDialog.ShowDialog() == true)
            {
                steamstring = inputDialog.Answer;
                try
                {
                    positions = Regex.Replace(steamstring, @"\s+", "").Split(',').ToList<string>().Select(x => Convert.ToInt32(x)).ToArray();
                }
                catch
                {
                    MessageBox.Show("Invalid Position List", "Invalid Position List", MessageBoxButton.OK);
                }
                
            }
            return positions;
        }

        
        private Func<InvestigationViewModel, int> PromptInvestigationsSort()
        {
            InputBox inputDialog = new InputBox("EXTREMELY Experimental, this will evaled directly by C#,\n " +
                                                "DO NOT USE IF YOU DON'T KNOW EXACTLY WHAT YOU ARE DOING:\n" +
                                                "Func<InvestigationViewModel, int>", "");
            inputDialog.Show();
            Func<InvestigationViewModel, int> filter = new Func<InvestigationViewModel, int>((x => 1));
            DataBinder.Eval(filter,inputDialog.Answer);
            return filter;
        }

        private Func<InvestigationViewModel, bool> PromptInvestigationsFilter()
        {
            InputBox inputDialog = new InputBox("EXTREMELY Experimental, this will evaled directly by C#,\n " +
                                                "DO NOT USE IF YOU DON'T KNOW EXACTLY WHAT YOU ARE DOING:\n" +
                                                "Func<InvestigationViewModel, bool>", "");
            inputDialog.Show();
            Func<InvestigationViewModel, bool> filter = new Func<InvestigationViewModel, bool>((x => true));
            DataBinder.Eval(filter,inputDialog.Answer);
            return filter;
        }
      
        private bool PromptInvestigationsInputFile(ref string filepath)
        {
            string steamPath = Utility.getSteamPath();
            var openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            filepath = openFileDialog.FileName;
            return filepath != "";
        }
        private bool PromptInvestigationsOutputFile(ref string filepath)
        {
            string steamPath = Utility.getSteamPath();
            var openFileDialog = new SaveFileDialog();
            openFileDialog.ShowDialog();
            filepath = openFileDialog.FileName;
            if (filepath!=""){
                Stream fStream = File.Create(filepath);
                fStream.Close();
            }
            return filepath != "";
        }
        
    }
}
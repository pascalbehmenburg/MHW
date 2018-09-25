using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Windows;
using MHW_Save_Editor.InvestigationEditing;
using Microsoft.Win32;

namespace MHW_Save_Editor
{
    public partial class MainWindow
    {
        private List<int> PromptPositions()
        {
            string steamstring;
            InputBox inputDialog = new InputBox("Enter indexes to execute the operation separated by commas\nRanges can be input with a hyphen e.g.(1,4,5-9):", "");
            List<int> positions = new List<int>();
            List<string> intermediates = new List<string>();
            if (inputDialog.ShowDialog() == true)
            {
                steamstring = inputDialog.Answer;
                try
                {
                    intermediates = Regex.Replace(steamstring, @"\s+", "").Split(',').ToList();
                    foreach (string argument in intermediates)
                    {
                        if (argument.Contains('-'))positions.AddRange(ParseRange(argument));
                        else positions.Add(Convert.ToInt32(argument));
                    }
                }
                catch{MessageBox.Show("Invalid Position List", "Invalid Position List", MessageBoxButton.OK);} 
            }
            return positions;
        }

        private List<int> ParseRange(string argument)
        {
            List<string>range = Regex.Replace(argument, @"\s+", "").Split('-').ToList();
            return Enumerable.Range(Convert.ToInt32(range[0]), Convert.ToInt32(range[1])-Convert.ToInt32(range[0])+1).ToList();
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
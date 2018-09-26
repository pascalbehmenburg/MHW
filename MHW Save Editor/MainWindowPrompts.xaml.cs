using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Windows;
using MHW_Save_Editor.InvestigationEditing;
using Microsoft.Win32;
using MultiParse;
using Expression = MultiParse.Expression;

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
        
        private Func<Investigation, T> PromptInvestigationsFunction<T>()
        {
            Func<Investigation, int > attempts = (x => x.Attempts);
            Func<Investigation, int > hp = (x => x.HP);
            Func<Investigation, int > faints = (x => Investigation.FaintValues[x.FaintIndex]);
            Func<Investigation, int > attack = (x => x.Attack);
            Func<Investigation, int > def = (x => x.Defense);
            Func<Investigation, int > rank = (x => x.Rank);
            Func<Investigation, int > goal = (x => x.Goal<6?0:(x.Goal<8?2:1));
            Func<Investigation, int > count = (x => Investigation._TimeAmountCount[x.Goal]);
            Func<Investigation, int > time = (x => Investigation._TimeAmountObjective[x.Goal]);
            Func<Investigation, int > locale = (x => x.LocaleIndex);
            Expression e = new Expression();
            string type = typeof(T).ToString();
            InputBox inputDialog = new InputBox($"Insert a fully formed expression with {type} result. Investigations satisfying the rule will be cleared.\n" +
                                                "Allowed variables are: " +
                                                "[A]ttempts: 0-10\n" +
                                                "[f]aints: 1,2,3,5\n"+
                                                "[h]p: 0-5\n" +
                                                "[a]ttack: 0-5\n" +
                                                "[d]efense: 0-5\n" +
                                                "[r]ank: 0-LR, 1-HR, 2-T\n" +
                                                "[g]oal: 0-Hunt, 1-Capture, 2-Wildlife\n" +
                                                "[c]ount(#ofMon): 0-3\n" +
                                                "[t]ime: 15 30 50\n" +
                                                "[l]ocale: 0-AF, 1-WW, 2-CH, 3-RV, 4-ER\n" +
                                                "Example of a Filter: (g == 2)|((t <= 50) & (f > 3)) will remove Wildlife and Hunts with less than 50min timer which have less than 3 faints.\n" +
                                                "Example of a Sorter: (t+c*50) will sort investigations by number of monsters and then by time.\n" +
                                                "Example of a Sorter: (r) will sort investigations by rank.", "");
            if (inputDialog.ShowDialog() == true)
            {

                string response = inputDialog.Answer;
                return (x => (T) e.Evaluate(response.Replace("A", attempts(x).ToString())
                                                        .Replace("f", faints(x).ToString())
                                                        .Replace("h", hp(x).ToString())
                                                        .Replace("a", attack(x).ToString())
                                                        .Replace("d", def(x).ToString())
                                                        .Replace("r", rank(x).ToString())
                                                        .Replace("g", goal(x).ToString())
                                                        .Replace("c", count(x).ToString())
                                                        .Replace("t", time(x).ToString())
                                                        .Replace("l", locale(x).ToString())
                                                        .Replace(" ", String.Empty)
                                                )
                        );
            }else return null;
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
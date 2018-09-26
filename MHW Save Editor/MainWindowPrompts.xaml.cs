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

        private static readonly Func<Investigation, int > _aux_func_attempts = (x => x.Attempts);
        private static readonly Func<Investigation, int > _aux_func_hp = (x => x.HP);
        private static readonly Func<Investigation, int > _aux_func_faints = (x => Investigation.FaintValues[x.FaintIndex]);
        private static readonly Func<Investigation, int > _aux_func_attack = (x => x.Attack);
        private static readonly Func<Investigation, int > _aux_func_def = (x => x.Defense);
        private static readonly Func<Investigation, int > _aux_func_rank = (x => x.Rank);
        private static readonly Func<Investigation, int > _aux_func_goal = (x => x.Goal<6?0:(x.Goal<8?2:1));
        private static readonly Func<Investigation, int > _aux_func_count = (x => Investigation._TimeAmountCount[x.Goal]);
        private static readonly Func<Investigation, int > _aux_func_time = (x => Investigation._TimeAmountObjective[x.Goal]);
        private static readonly Func<Investigation, int > _aux_func_locale = (x => x.LocaleIndex);

        private static readonly string _aux_allowed_variables = "Allowed variables are: \n" +
                                                                "[A]ttempts: 0-10\n" +
                                                                "[f]aints: 1,2,3,5\n" +
                                                                "[h]p: 0-5\n" +
                                                                "[a]ttack: 0-5\n" +
                                                                "[d]efense: 0-5\n" +
                                                                "[r]ank: 0-LR, 1-HR, 2-T\n" +
                                                                "[g]oal: 0-Hunt, 1-Capture, 2-Wildlife\n" +
                                                                "[c]ount(#ofMon): 0-3\n" +
                                                                "[t]ime: 15 30 50\n" +
                                                                "[l]ocale: 0-AF, 1-WW, 2-CH, 3-RV, 4-ER\n";
        
        
        private Func<Investigation, IEnumerable<int>> PromptInvestigationsSorter()
        {
            InputBox inputDialog = new InputBox("List the sorting criteria separated by commas:\n" +
                                                _aux_allowed_variables +
                                                "\nExample of a Sort: r,-g,a \n" +
                                                "Will sort first by rank descending, then within the same rank by goal ascending \n" +
                                                "and finally within goals by attempts left descending.\n", "");
            if (inputDialog.ShowDialog() == true)
            {
                try
                {
                    string response = inputDialog.Answer;
                    if (response.Length == 0) return null;
                    List<Func<Investigation, int>> mapping = new List<Func<Investigation, int>>();
                    List<string> intermediates = response.Replace(" ", String.Empty).Split(',').ToList();
                    foreach (string step in intermediates)
                        mapping.Add(step.Contains('-')?(x=>-_aux_func[step[1]](x)):_aux_func[step[0]]);
                    return (x => mapping.Apply(x));
                }
                catch{MessageBox.Show("Invalid Ordering String", "Invalid Ordering String", MessageBoxButton.OK);} 
            }
            return null;
        }

        private static readonly Dictionary<char, Func<Investigation, int>> _aux_func =
            new Dictionary<char, Func<Investigation, int>>
            {
                {'A', _aux_func_attempts},
                {'f', _aux_func_faints},
                {'a', _aux_func_attack},
                {'h', _aux_func_hp},
                {'d', _aux_func_def},
                {'r', _aux_func_rank},
                {'g', _aux_func_goal},
                {'c', _aux_func_count},
                {'t', _aux_func_time},
                {'l', _aux_func_locale}
            };
        
        private Func<Investigation, bool> PromptInvestigationsFilter()
        {
            Expression e = new Expression();
            InputBox inputDialog = new InputBox("Insert a fully formed expression with boolean result. Investigations satisfying the rule will be cleared.\n" +
                                                _aux_allowed_variables +
                                                "\nExample of a Filter: (g == 2)|((t <= 50) & (f > 3)) will remove Wildlife and Hunts with less than 50min timer which have less than 3 faints.\n", "");
            if (inputDialog.ShowDialog() == true)
            {
                string response = inputDialog.Answer;
                return (x => (bool) e.Evaluate(response.Replace("A", _aux_func['A'](x).ToString())
                            .Replace("f", _aux_func['f'](x).ToString())
                            .Replace("h", _aux_func['h'](x).ToString())
                            .Replace("a", _aux_func['a'](x).ToString())
                            .Replace("d", _aux_func['d'](x).ToString())
                            .Replace("r", _aux_func['r'](x).ToString())
                            .Replace("g", _aux_func['g'](x).ToString())
                            .Replace("c", _aux_func['c'](x).ToString())
                            .Replace("t", _aux_func['t'](x).ToString())
                            .Replace("l", _aux_func['l'](x).ToString())
                            .Replace(" ", String.Empty)));
            }
            return null;
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MHW_Save_Editor.Data;


namespace MHW_Save_Editor.InvestigationEditing
{
    public class Investigation : NotifyUIBase
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        public Investigation(byte[] newdata)
        {
            if (newdata.Length != 0) _underlyingInvestigationThinLayer = new InvestigationThinLayer(newdata);
            else
            {
                byte[] newish = new byte[Investigation.inv_size];
                Array.Copy(InvestigationThinLayer.nullinvestigation, 0, newish, 0, Investigation.inv_size);
                _underlyingInvestigationThinLayer = new InvestigationThinLayer(newish);
            }
        }

        private InvestigationThinLayer _underlyingInvestigationThinLayer;

        public string InvestigationTitle
        {
            get
            {
                if (!_underlyingInvestigationThinLayer.Filled) return "Empty Slot";
                string objective = _TimeAmountGoal[Goal];
                int count = _TimeAmountCount[Goal];
                string mainmon = count != 0 ? (MonsterNames[Mon1] + (count > 1 ? ", ..." : "")) : "Wildlife";
                return objective + " " + mainmon;
            }
        }

        public string LocaleTitle
        {
            get => LocalesNames[LocaleIndex];
        }
        
        public string Legality
        {
            get
            {
                StringBuilder Bob = new StringBuilder();
                UntemperableCondition(Bob);
                RankTemperCondition(Bob);
                ElderCondition(Bob);
                MonsterLocaleCondition(Bob);
                PickleBagelCondition(Bob);
                return Bob.ToString();
            }
        }

        public static readonly int inv_size = 42;
        public static readonly int inv_number = 250;
        public static readonly int[] inv_offsets = {0x003DADB1, 0x004D0EC1, 0x005C6FD1};
        

    #region Members
        
        public bool Filled
        {
            get => _underlyingInvestigationThinLayer.Filled;
        }
        
        public string ToggleState
        {
            get => _underlyingInvestigationThinLayer.Filled?"Clear":"Initialize";
        }
        
        public int Goal
        {
            get => _underlyingInvestigationThinLayer.TimeAmountIndex;
            set{
                _underlyingInvestigationThinLayer.TimeAmountIndex = value;
                RaisePropertyChanged();
                RaisePropertyChanged("InvestigationTitle");
                RaisePropertyChanged("Legality");
            }
        }
        public bool Seen
        {
            get => _underlyingInvestigationThinLayer.Seen;
            set{
                _underlyingInvestigationThinLayer.Seen = value;
                RaisePropertyChanged();
            }
        }
        public bool Selected
        {
            get => _underlyingInvestigationThinLayer.Selected;
            set{
                _underlyingInvestigationThinLayer.Selected = value;
                RaisePropertyChanged();
            }
        }
        public int Rank
        {
            get => _underlyingInvestigationThinLayer.Rank;
            set
            {
                _underlyingInvestigationThinLayer.Rank = value;
                RaisePropertyChanged();
                RaisePropertyChanged("Legality");
            }
        }
        public int Mon1
        {
            get => _CodeToMonsterIndex[_underlyingInvestigationThinLayer.Mon1];
            set{
                _underlyingInvestigationThinLayer.Mon1 = _MonstersCodeList[value];
                RaisePropertyChanged();
                RaisePropertyChanged("InvestigationTitle");
                RaisePropertyChanged("Legality");
            }
        }
        public int Mon2
        {
            get => _CodeToMonsterIndex[_underlyingInvestigationThinLayer.Mon2];
            set{
                _underlyingInvestigationThinLayer.Mon2 = _MonstersCodeList[value];
                RaisePropertyChanged();
                RaisePropertyChanged("InvestigationTitle");
                RaisePropertyChanged("Legality");
            }
        }
        public int Mon3
        {
            get => _CodeToMonsterIndex[_underlyingInvestigationThinLayer.Mon3];
            set{
                _underlyingInvestigationThinLayer.Mon3 = _MonstersCodeList[value];
                RaisePropertyChanged();
                RaisePropertyChanged("InvestigationTitle");
                RaisePropertyChanged("Legality");
            }
        }
        public bool M1Temper
        {
            get => _underlyingInvestigationThinLayer.M1Temper;
            set
            {
                _underlyingInvestigationThinLayer.M1Temper = value;
                RaisePropertyChanged();
                RaisePropertyChanged("Legality");
            }
        }
        public bool M2Temper
        {
            get => _underlyingInvestigationThinLayer.M2Temper;
            set
            {
                _underlyingInvestigationThinLayer.M2Temper = value;
                RaisePropertyChanged();
                RaisePropertyChanged("Legality");
            }
        }
        public bool M3Temper
        {
            get => _underlyingInvestigationThinLayer.M3Temper;
            set
            {
                _underlyingInvestigationThinLayer.M3Temper = value;
                RaisePropertyChanged();
                RaisePropertyChanged("Legality");
            }
        }

        public int HP
        {
            get => _underlyingInvestigationThinLayer.HP;
            set
            {
                _underlyingInvestigationThinLayer.HP = value;
                RaisePropertyChanged();
            }
        }
        public int Attack
        {
            get => _underlyingInvestigationThinLayer.Attack;
            set
            {
                _underlyingInvestigationThinLayer.Attack = value;
                RaisePropertyChanged();
            }
        }
        public int Defense
        {
            get => _underlyingInvestigationThinLayer.Defense;
            set
            {
                _underlyingInvestigationThinLayer.Defense = value;
                RaisePropertyChanged();
            }
        }
        public int X3
        {
            get => _underlyingInvestigationThinLayer.X3;
            set
            {
                _underlyingInvestigationThinLayer.X3 = value;
                RaisePropertyChanged();
            }
        }
        public int FaintIndex
        {
            get => _underlyingInvestigationThinLayer.FaintIndex;
            set
            {
                _underlyingInvestigationThinLayer.FaintIndex = value;
                RaisePropertyChanged();
            }
        }
        public int PlayerCountIndex
        {
            get => _underlyingInvestigationThinLayer.PlayerCountIndex;
            set
            {
                _underlyingInvestigationThinLayer.PlayerCountIndex = value;
                RaisePropertyChanged();
            }
        }   
        public int ZennyBonus
        {
            get => _underlyingInvestigationThinLayer.ZennyMultiplier;
            set
            {
                _underlyingInvestigationThinLayer.ZennyMultiplier = value;
                RaisePropertyChanged();
            }
        }  
        public int LocaleIndex
        {
            get => _underlyingInvestigationThinLayer.LocaleIndex;
            set
            {
                _underlyingInvestigationThinLayer.LocaleIndex = value;
                RaisePropertyChanged();
                RaisePropertyChanged("CurrentFlourishes");
                RaisePropertyChanged("FlourishIndex");
                RaisePropertyChanged("Legality");
            }
        }

        public int FlourishIndex
        {
            get => _underlyingInvestigationThinLayer.FlourishIndex;
            set
            {
                _underlyingInvestigationThinLayer.FlourishIndex = value;
                RaisePropertyChanged();
            }
        }
        public int Y0
        {
            get => _underlyingInvestigationThinLayer.Y0;
            set
            {
                _underlyingInvestigationThinLayer.Y0 = value;
                RaisePropertyChanged();
            }
        }
        public int Y3
        {
            get => _underlyingInvestigationThinLayer.Y3;
            set
            {
                _underlyingInvestigationThinLayer.Y3 = value;
                RaisePropertyChanged();
            }
        }
        public int MonsterRewards
        {
            get => _underlyingInvestigationThinLayer.MonsterRewards;
            set
            {
                _underlyingInvestigationThinLayer.MonsterRewards = value;
                RaisePropertyChanged();
            }
        }

        public int Attempts
        {
            get => _underlyingInvestigationThinLayer.Attempts;
            set
            {
                _underlyingInvestigationThinLayer.Attempts = value;
                RaisePropertyChanged();
            }
        }
        #endregion

    #region Methods

        public byte[] Serialize()
        {
            return _underlyingInvestigationThinLayer.Serialize();
        }

        public static readonly string CSVHeader =
            "Seen,Filled,Attempts,Rank,Goal,Mon#,Time,Locale,Flourish,Monster1,Temper1,Monster2,Temper2,Monster3,Temper3,HP,Attack,Defense,X3,Y0,Y3,Faints,PlayerCount,MonRewards,ZennyBonus";
        
        public string LogCSV()
        {
            return (Seen ? 1 : 0) + "," + (Filled ? 1 : 0) + "," + Attempts + "," + _RankChoices[Rank] + "," +
                   _TimeAmountGoal[Goal] + "," + _TimeAmountCount[Goal] + "," + _TimeAmountObjective[Goal] + "," +
                   _LocalesNames[LocaleIndex] + "," + CurrentFlourishes[FlourishIndex] + "," + MonsterNames[Mon1] +
                   "," + (M1Temper ? 1 : 0) + "," + MonsterNames[Mon2] + "," + (M2Temper ? 1 : 0) + "," +
                   MonsterNames[Mon3] + "," + (M3Temper ? 1 : 0) + "," + HP + "," + Attack + "," + Defense + "," + X3 +
                   "," + Y0 + "," + Y3 + "," + _FaintValues[FaintIndex] + "," + _PlayerCountValues[PlayerCountIndex] +
                   "," + MonsterRewards + "," +
                   ZennyBonus;
        }        

        public string Log()
        {
            var builder = new StringBuilder()
                .AppendLine($"_____________________________________")
                .AppendLine($"Attempts: {Attempts}")
                .AppendLine($"Locale: {_LocalesNames[LocaleIndex]} - {CurrentFlourishes[FlourishIndex]}")
                .AppendLine($"Rank: {_RankChoices[Rank]}")
                .AppendLine($"{(M1Temper?"Tempered ":"")}{MonsterNames[Mon1]}")
                .AppendLine($"{(M2Temper?"Tempered ":"")}{MonsterNames[Mon2]}")
                .AppendLine($"{(M3Temper?"Tempered ":"")}{MonsterNames[Mon3]}")
                .AppendLine($"HP: {HP} - Att: {Attack} - Def: {Defense} - X3: {X3}")
                .AppendLine("Goal: "+_TimeAmountGoal[Goal]+" "+(_TimeAmountCount[Goal]!=0?(_TimeAmountCount[Goal]+
                           " Monster"+(_TimeAmountCount[Goal]>1?"s":"")):"")+ " in "+_TimeAmountObjective[Goal]+" min")
                .AppendLine($"Y0: {Y0} - Y3:{Y3}")
                .AppendLine($"Faints: {_FaintValues[FaintIndex]} - Players: {_PlayerCountValues[PlayerCountIndex]} - Box Multiplier: {MonsterRewards} - Zenny Multiplier: {ZennyBonus}");
            return builder.ToString();
        }

    #endregion
        
    #region LegalityConditions
       
        private void UntemperableCondition(StringBuilder Bob)
            //Checks if an untemperable monster has been tempered
        {
            string mon = MonsterNames[Mon1];
            if (_untemperable.Contains(mon) && M1Temper)
                Bob.Append($"{mon} cannot be tempered.");
            mon = MonsterNames[Mon2];
            if (_untemperable.Contains(mon) && M2Temper)
                Bob.Append($"{mon} cannot be tempered.");
            mon = MonsterNames[Mon3];
            if (_untemperable.Contains(mon) && M3Temper)
                Bob.Append($"{mon} cannot be tempered.");
        }

        private void RankTemperCondition(StringBuilder Bob)
            //Checks if there's tempering on a Low or High Rank investigation
        {
            if ((M1Temper || M2Temper || M3Temper)&& Rank!=2)Bob.Append($"Cannot have a Tempered Monster on a Non-Tempered Hunt.{Environment.NewLine}");
        }
        
        private static bool IsElder(int moncode)
        {
            return _Elders.Contains(_MonsterNames[moncode]);
        }

        private static bool IsEmpty(int moncode)
        {
            return _MonsterNames[moncode] == "Empty";
        }
        
        private void ElderCondition(StringBuilder Bob)
            //Checks elder dragons have no other monsters on their hunt
        {
            if (IsElder(Mon2) || IsElder(Mon3)) Bob.Append($"Elder Dragons should be in slot 1.{Environment.NewLine}");
            if (IsElder(Mon1) && !(IsEmpty(Mon2) && IsEmpty(Mon3))) Bob.Append($"Elder Dragons should be alone, set slot 2 and 3 to Empty.{Environment.NewLine}");
            if (IsElder(Mon1) && Goal>=3) Bob.Append($"Elder Dragons only allow Hunt 1 Monster in 50/30/15 min as a goal.{Environment.NewLine}");
        }

        private void MonsterLocaleCondition(StringBuilder Bob)
            //Checks if Monsters can be in the Locale and Rank of the investigation
        {
            if (!MonsterInLocale(Mon1, LocaleIndex, Rank))Bob.Append($"{MonsterNames[Mon1]} cannot be found in {LocalesNames[LocaleIndex]} at rank {RankChoices[Rank]}.{Environment.NewLine}");
            if (!MonsterInLocale(Mon2, LocaleIndex, Rank))Bob.Append($"{MonsterNames[Mon2]} cannot be found in {LocalesNames[LocaleIndex]} at rank {RankChoices[Rank]}.{Environment.NewLine}");
            if (!MonsterInLocale(Mon3, LocaleIndex, Rank))Bob.Append($"{MonsterNames[Mon3]} cannot be found in {LocalesNames[LocaleIndex]} at rank {RankChoices[Rank]}.{Environment.NewLine}");
        }
        
        private static bool MonsterInLocale(int monster, int locale, int rank)
        {
            return true;
        }

        private void PickleBagelCondition(StringBuilder Bob)
            //Checks any of the bizarre Bagel and Pickle conditions
        {
        }
        

    #endregion
        
    #region DataCollections

        public static readonly int[] _TimeAmountObjective =
            {50, 30, 15, 50, 30, 50, 50, 50, 50, 30, 15};

        public static readonly string[] _TimeAmountGoal =
        {
            "Hunt", "Hunt", "Hunt", "Hunt", "Hunt", "Hunt", "Slay Wildlife 1", "Slay Wildlife 2", "Capture",
            "Capture", "Capture"
        };
        public static readonly int[] _TimeAmountCount = {1,1,1,2,2,3,0,0,1,1,1};

        public static  readonly UInt32[] _MonstersCodeList =
        {
            0x00, 0x01, 0x07, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x10, 0x11,
            0x12, 0x13, 0x14, 0x15, 0x16, 0x18, 0x19, 0x1B, 0x1C, 0x1D, 0x1E,
            0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x27, 0xFFFFFFFF
        };

        private static List<String> _untemperable = new List<string>()
        {
            "Dodogama", "Great Jagras", "Great Girros", "Kulu-Ya-Ku", "Tzitzi-Ya-Ku"
        };
        
        private static string[] _Elders = new[]
        {
            "Kirin", "Kushala Daora", "Teostra", "Lunastra", "Nergigante", "Vaal Hazak"
        };
        
        public ObservableCollection<int> CommonValues
        {
            get => new ObservableCollection<int>(new[] {0, 1, 2, 3, 4, 5});
        }

        public static readonly ObservableCollection<int> _ZennyValues = new ObservableCollection<int>(new[]{0,1,2,3,4});
        public static  ObservableCollection<int> ZennyValues => _ZennyValues;
        public static readonly ObservableCollection<int> _PlayerCountValues = new ObservableCollection<int>(new[]{4,2});
        public static ObservableCollection<int> PlayerCountValues => _PlayerCountValues;
        public static readonly ObservableCollection<int> _FaintValues = new ObservableCollection<int>(new[]{5,3,2,1});
        public static  ObservableCollection<int> FaintValues => _FaintValues;
        public static readonly ObservableCollection<int> _MonsterRewardsValues= new ObservableCollection<int>(new[]{0,1,2,3,4});
        public static  ObservableCollection<int> MonsterRewardsValues => _MonsterRewardsValues;
        public static readonly ObservableCollection<string> _LocalesNames = new ObservableCollection<string>(
            new []{"Ancient Forest", "Wildspire Wastes", "Coral Highlands","Rotten Vale", "Elder Recess"});
        public static  ObservableCollection<string> LocalesNames => _LocalesNames;
        public static readonly ObservableCollection<string> _RankChoices = new ObservableCollection<string>(
            new []{"Low Rank", "High Rank", "Tempered"});

        public static  ObservableCollection<string> RankChoices => _RankChoices;
        public ObservableCollection<string> CurrentFlourishes
        {
            get => _FlourishMatrix[LocaleIndex];
        }

        public static readonly ObservableCollection<string> _GoalChoices = new ObservableCollection<string>(new []
        {
            "Hunt 1 Monster in 50 min", "Hunt 1 Monster in 30 min", "Hunt 1 Monster in 15 min",
            
            "Hunt 2 Monsters in 50 min", "Hunt 2 Monsters in 30 min", "Hunt 3 Monsters in 50 min",
            "Slay Wildlife 1 in 50 min", "Slay Wildlife 2 in 50 min",
            "Capture 1 Monster in 50 min", "Capture 1 Monster in 30 min", "Capture 1 Monster in 15 min"
        });

        public static ObservableCollection<string> GoalChoices => _GoalChoices;
        public static readonly Dictionary<UInt32, int> _CodeToMonsterIndex = new Dictionary<UInt32, int>()
        {
            {0x00,0},{0x01,1},{0x07,2},{0x09,3},{0x0a,4},{0x0b,5},{0x0c,6},{0x0d,7},{0x0e,8},{0x10,9},{0x11,10},
            {0x12,11},{0x13,12},{0x14,13},{0x15,14},{0x16,15},{0x18,16},{0x19,17},{0x1b,18},{0x1c,19},{0x1d,20},{0x1e,21},
            {0x1f,22},{0x20,23},{0x21,24},{0x22,25},{0x23,26},{0x24,27},{0x25,28},{0x27,29},{0xffffffff,30}
        };

        private static readonly string[] _MonsterNames = new[]
        {
            "Anjanath", "Rathalos", "Great Jagras", "Rathian", "Pink Rathian", "Azure Rathalos",
            "Diablos", "Black Diablos", "Kirin", "Kushala Daora", "Lunastra", "Teostra",
            "Lavasioth", "Deviljho", "Barroth", "Uragaan", "Pukei-Pukei", "Nergigante",
            "Kulu-Ya-Ku", "Tzitzi-Ya-Ku", "Jyuratodus", "Tobi-Kadachi", "Paolumu",
            "Legiana", "Great Girros", "Odogaron", "Radobaan", "Vaal Hazak", "Dodogama",
            "Bazelgeuse", "Empty"
        };
        
        public ObservableCollection<string> MonsterNames
        {
            get => new ObservableCollection<string>(_MonsterNames);
        }

        public static readonly ObservableCollection<string>[] _FlourishMatrix =
        {
            new ObservableCollection<string>(new [] {"Nothing","Mushrooms", "Flower Beds", "Mining Outcrops","Bonepiles","Gathering Points"}),
            new ObservableCollection<string>(new [] {"Nothing","Cactus", "Fruit", "Mining Outcrops","Bonepiles","Gathering Points"}),
            new ObservableCollection<string>(new [] {"Nothing","Conch Shells", "Pearl Oysters", "Mining Outcrops","Bonepiles","Gathering Points"}),
            new ObservableCollection<string>(new [] {"Nothing","Ancient Fossils", "Crimson Fruit", "Mining Outcrops","Bonepiles","Gathering Points"}),
            new ObservableCollection<string>(new [] {"Nothing","Amber Deposits", "Beryl Deposits", "Mining Outcrops","Bonepiles","Gathering Points"})
        };
    #endregion
    }
}
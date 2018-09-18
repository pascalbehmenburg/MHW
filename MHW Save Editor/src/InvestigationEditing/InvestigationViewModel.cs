using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace MHW.InvestigationEditing
{
    public class InvestigationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        public InvestigationViewModel(Investigation entry = null)
        {
            if (entry != null) UnderlyingInvestigation = entry;
            else
            {
                byte[] newish = new byte[InvestigationList.inv_size];
                Array.Copy(Investigation.nullinvestigation, 0, newish, 0, InvestigationList.inv_size);
                UnderlyingInvestigation = new Investigation(newish);
            }
        }

        private Investigation UnderlyingInvestigation;
             
        public string InvestigationTitle
        {
            get
            {
                if (!UnderlyingInvestigation.Filled) return "Empty Slot";
                string objective = _TimeAmountGoal[Goal];
                int count = _TimeAmountCount[Goal];
                string mainmon =  count !=0?(MonsterNames[Mon1] + (count>1?", ...":"")):"Wildlife";
                return objective + " " + mainmon;
            }
        }
        #region Members
        public bool Filled
        {
            get => UnderlyingInvestigation.Filled;
        }
        
        public string ToggleState
        {
            get => UnderlyingInvestigation.Filled?"Clear":"Initialize";
        }
        public int Goal
        {
            get => UnderlyingInvestigation.TimeAmountIndex;
            set{
                UnderlyingInvestigation.TimeAmountIndex = value;
                RaisePropertyChanged("Goal");
                RaisePropertyChanged("InvestigationTitle");
            }
        }
        public int Rank
        {
            get => UnderlyingInvestigation.Rank;
            set
            {
                UnderlyingInvestigation.Rank = value;
                RaisePropertyChanged("Rank");
            }
        }
        public int Mon1
        {
            get => _CodeToMonsterIndex[UnderlyingInvestigation.Mon1];
            set{
                UnderlyingInvestigation.Mon1 = _MonstersCodeList[value];
                RaisePropertyChanged("Mon1");
                RaisePropertyChanged("InvestigationTitle");
            }
        }
        public int Mon2
        {
            get => _CodeToMonsterIndex[UnderlyingInvestigation.Mon2];
            set{
                UnderlyingInvestigation.Mon2 = _MonstersCodeList[value];
                RaisePropertyChanged("Mon2");
            }
        }
        public int Mon3
        {
            get => _CodeToMonsterIndex[UnderlyingInvestigation.Mon3];
            set{
                UnderlyingInvestigation.Mon3 = _MonstersCodeList[value];
                RaisePropertyChanged("Mon3");
            }
        }
        public bool M1Temper
        {
            get => UnderlyingInvestigation.M1Temper;
            set
            {
                UnderlyingInvestigation.M1Temper = value;
                RaisePropertyChanged("M1Temper");
            }
        }
        public bool M2Temper
        {
            get => UnderlyingInvestigation.M2Temper;
            set
            {
                UnderlyingInvestigation.M2Temper = value;
                RaisePropertyChanged("M2Temper");
            }
        }
        public bool M3Temper
        {
            get => UnderlyingInvestigation.M3Temper;
            set
            {
                UnderlyingInvestigation.M3Temper = value;
                RaisePropertyChanged("M3Temper");
            }
        }

        public int HP
        {
            get => UnderlyingInvestigation.HP;
            set
            {
                UnderlyingInvestigation.HP = value;
                RaisePropertyChanged("HP");
            }
        }
        public int Attack
        {
            get => UnderlyingInvestigation.Attack;
            set
            {
                UnderlyingInvestigation.Attack = value;
                RaisePropertyChanged("Attack");
            }
        }
        public int Size
        {
            get => UnderlyingInvestigation.Size;
            set
            {
                UnderlyingInvestigation.Size = value;
                RaisePropertyChanged("Size");
            }
        }
        public int X3
        {
            get => UnderlyingInvestigation.X3;
            set
            {
                UnderlyingInvestigation.X3 = value;
                RaisePropertyChanged("X3");
            }
        }
        public int FaintIndex
        {
            get => UnderlyingInvestigation.FaintIndex;
            set
            {
                UnderlyingInvestigation.FaintIndex = value;
                RaisePropertyChanged("FaintIndex");
            }
        }
        public int PlayerCountIndex
        {
            get => UnderlyingInvestigation.PlayerCountIndex;
            set
            {
                UnderlyingInvestigation.PlayerCountIndex = value;
                RaisePropertyChanged("PlayerCountIndex");
            }
        }   
        public int ZennyBonus
        {
            get => UnderlyingInvestigation.PlayerCountIndex;
            set
            {
                UnderlyingInvestigation.PlayerCountIndex = value;
                RaisePropertyChanged("PlayerCountIndex");
            }
        }  
        public int LocaleIndex
        {
            get => UnderlyingInvestigation.LocaleIndex;
            set
            {
                UnderlyingInvestigation.LocaleIndex = value;
                RaisePropertyChanged("LocaleIndex");
                RaisePropertyChanged("CurrentFlourishes");
            }
        }

        public int FlourishIndex
        {
            get => UnderlyingInvestigation.FlourishIndex;
            set
            {
                UnderlyingInvestigation.FlourishIndex = value;
                RaisePropertyChanged("FlourishIndex");
            }
        }
        public int Y0
        {
            get => UnderlyingInvestigation.Y0;
            set
            {
                UnderlyingInvestigation.Y0 = value;
                RaisePropertyChanged("Y0");
            }
        }
        public int Y3
        {
            get => UnderlyingInvestigation.Y3;
            set
            {
                UnderlyingInvestigation.Y3 = value;
                RaisePropertyChanged("Y3");
            }
        }
        public int MonsterRewards
        {
            get => UnderlyingInvestigation.MonsterRewards;
            set
            {
                UnderlyingInvestigation.MonsterRewards = value;
                RaisePropertyChanged("MonsterRewards");
            }
        }

        public int Attempts
        {
            get => UnderlyingInvestigation.Attempts;
            set
            {
                UnderlyingInvestigation.Attempts = value;
                RaisePropertyChanged("Attempts");
            }
        }
        #endregion

        #region Methods

        public byte[] Serialize()
        {
            return UnderlyingInvestigation.Serialize();
        }

        public void Toggle()
        {
            if (Filled) UnderlyingInvestigation.Clear();
            else UnderlyingInvestigation.Initialize();
            RaisePropertyChanged(null);
        }

        public void Clear()
        {
            if (Filled)
            {
                UnderlyingInvestigation.Clear();
                RaisePropertyChanged(null);
            }
        }

        public void Overwrite(byte [] overwriter)
        {
            UnderlyingInvestigation = new Investigation(overwriter);
            RaisePropertyChanged(null);
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
                .AppendLine($"HP: {HP} - Att: {Attack} - Size: {Size} - X3: {X3}")
                .AppendLine($"Goal: "+_TimeAmountGoal[Goal]+" "+(_TimeAmountCount[Goal]!=0?(_TimeAmountCount[Goal]+
                           " Monster"+(_TimeAmountCount[Goal]>1?"s":"")):"")+ " in "+_TimeAmountObjective[Goal]+" min")
                .AppendLine($"Y0: {Y0} - Y3:{Y3}")
                .AppendLine($"Faints: {_FaintValues[FaintIndex]} - Players: {_PlayerCountValues[PlayerCountIndex]} - Box Multiplier: {MonsterRewards} - Zenny Multiplier: {ZennyBonus}");
            return builder.ToString();
        }

        #endregion
        
        #region DataCollections

        public static readonly int[] _TimeAmountObjective =
            {50, 30, 15, 50, 30, 50, 50, 50, 50, 30, 15};

        public static readonly string[] _TimeAmountGoal =
        {
            "Hunt", "Hunt", "Hunt", "Hunt", "Hunt", "Hunt", "Slay Wildlife 1", " Slay Wildlife 2", "Capture",
            "Capture", "Capture"
        };
        public static readonly int[] _TimeAmountCount = {1,1,1,2,2,3,0,0,1,1,1};

        public static  readonly UInt32[] _MonstersCodeList =
        {
            0x00, 0x01, 0x07, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x10, 0x11,
            0x12, 0x13, 0x14, 0x15, 0x16, 0x18, 0x19, 0x1B, 0x1C, 0x1D, 0x1E,
            0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x27, 0xFFFFFFFF
        };
        
        public static readonly ObservableCollection<int> _CommonValues = new ObservableCollection<int>(new[]{0,1,2,3,4,5});
        public static  ObservableCollection<int> CommonValues => _CommonValues;
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

        public ObservableCollection<string> MonsterNames
        {
            get => new ObservableCollection<string>(new[]
            {
                "Anjanath", "Rathalos", "Great Jagras", "Rathian", "Pink Rathian", "Azure Rathalos",
                "Diablos", "Black Diablos", "Kirin", "Kushala Daora", "Lunastra", "Teostra",
                "Lavasioth", "Deviljho", "Barroth", "Uragaan", "Pukei-Pukei", "Nergigante",
                "Kulu-Ya-Ku", "Tzitzi-Ya-Ku", "Jyuratodus", "Tobi-Kadachi", "Paolumu",
                "Legiana", "Great Girros", "Odogaron", "Radobaan", "Vaal Hazak", "Dodogama",
                "Bazelgeuse", "Empty"
            });
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            }
        }
        public int Mon1
        {
            get => _CodeToMonsterIndex[_underlyingInvestigationThinLayer.Mon1];
            set{
                _underlyingInvestigationThinLayer.Mon1 = _MonstersCodeList[value];
                RaisePropertyChanged();
                RaisePropertyChanged("InvestigationTitle");
            }
        }
        public int Mon2
        {
            get => _CodeToMonsterIndex[_underlyingInvestigationThinLayer.Mon2];
            set{
                _underlyingInvestigationThinLayer.Mon2 = _MonstersCodeList[value];
                RaisePropertyChanged();
                RaisePropertyChanged("InvestigationTitle");
            }
        }
        public int Mon3
        {
            get => _CodeToMonsterIndex[_underlyingInvestigationThinLayer.Mon3];
            set{
                _underlyingInvestigationThinLayer.Mon3 = _MonstersCodeList[value];
                RaisePropertyChanged();
                RaisePropertyChanged("InvestigationTitle");
            }
        }
        public bool M1Temper
        {
            get => _underlyingInvestigationThinLayer.M1Temper;
            set
            {
                _underlyingInvestigationThinLayer.M1Temper = value;
                RaisePropertyChanged();
            }
        }
        public bool M2Temper
        {
            get => _underlyingInvestigationThinLayer.M2Temper;
            set
            {
                _underlyingInvestigationThinLayer.M2Temper = value;
                RaisePropertyChanged();
            }
        }
        public bool M3Temper
        {
            get => _underlyingInvestigationThinLayer.M3Temper;
            set
            {
                _underlyingInvestigationThinLayer.M3Temper = value;
                RaisePropertyChanged();
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
        public int Size
        {
            get => _underlyingInvestigationThinLayer.Size;
            set
            {
                _underlyingInvestigationThinLayer.Size = value;
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
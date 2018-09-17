using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace MHW.InvestigationEditing
{
    public class InvestigationViewModelWBL : INotifyPropertyChanged
    {
        public InvestigationViewModelWBL(Investigation entry = null)
        {
            if (entry != null) _inv = entry;
            else
            {
                byte[] newish = new byte[InvestigationList.inv_size];
                Array.Copy(Investigation.nullinvestigation, 0, newish, 0, InvestigationList.inv_size);
                _inv = new Investigation(newish);
            }
        }

        private Investigation _inv;
             
        public string InvestigationTitle
        {
            get
            {
                if (!_inv.GetIsFilled()) return "Empty Slot";
                string objective = _inv.GetAmount() == 0 ? "Slay" : (_inv.GetCapture() ? "Capture" : "Hunt");
                string mainmon = _inv.GetAmount()!=0?_inv.GetMonsters()[0].Item1 + (_inv.GetAmount()>1?", ...":""):"Wildlife";
                return objective + " " + mainmon;
            }
        }

        private static readonly Dictionary<string, byte> rankToCode = new Dictionary<string, byte>
        {
            {Investigation.ranklist[0], 0x00},
            {Investigation.ranklist[1], 0x01},
            {Investigation.ranklist[2], 0x02}
        };
        
        #region RankManagement
        public string InvestigationRank
        {
            get => _inv.GetRank();
            set
            {
                byte code = rankToCode[value];
                if (code == 0x00)SetLowRank();
                if (code == 0x01)SetHighRank();
                if (code == 0x02)SetTemperedRank();
                RaisePropertyChanged("InvestigationRank");
                RaisePropertyChanged("MonsterChoices");
                RaisePropertyChanged("Mon1");RaisePropertyChanged("Mon2");RaisePropertyChanged("Mon3");
            }
        }

        private void SetLowRank()
        {
            if (Locale == Investigation.area4)
            {
                _inv.SetLocale(0);
                RaisePropertyChanged("Locale");
                RaisePropertyChanged("Flourish");
                RaisePropertyChanged("CurrentFlourish");
            }
            if (_inv.IsElderInvestigation())SetDefaultMonsters(Locale);
            _inv.SetRank(0x00);
            M1Temper = false; M2Temper = false; M3Temper = false;
            bool v1 = MonsterChoices.Contains(Mon1);
            bool v2 = MonsterChoices.Contains(Mon1);
            bool v3 = MonsterChoices.Contains(Mon1);
            if (v1 && v2 && v3) return;
            SetDefaultMonsters(Locale);
        }

        private void SetHighRank()
        {
            _inv.SetRank(0x01);
            M1Temper = false; M2Temper = false; M3Temper = false;
        }

        private void SetTemperedRank()
        {
            _inv.SetRank(0x02);
            if (!M1Temper) M1Temper=true;
            
        }
    
        #endregion
        #region MonsterManagement
        public string Mon1
        {
            get => _inv.GetMonsters()[0].Item1;
            set => SetMonster(0, value);
        }
        public string Mon2
        {
            get => _inv.GetMonsters()[1].Item1;
            set => SetMonster(1, value);
        }
        public string Mon3
        {
            get => _inv.GetMonsters()[2].Item1;
            set => SetMonster(2, value);
        }
        
        private string[] GetMonsters()
        {
            return new [] {Mon1, Mon2, Mon3};
        }
        
        public void SetMonster(int index, string monname)
        {
            string[] monsters = GetMonsters();
            UInt32 code = Investigation.monsterToCode[monname];
            if (code == 0xFFFFFFFF) return;
            if (_inv.IsElderInvestigation() && !_inv.IsElder(code))
            {
                SetDefaultMonsters(Locale);
                ForceSetMonster(index, code);
                RaisePropertyChanged("InvestigationTitle");
                RaisePropertyChanged("GoalChoices");
                return;
            }
            if (_inv.IsElder(code))
            {
                ForceSetMonster(0, code);
                ForceSetMonster(1, 0xFFFFFFFF);
                ForceSetMonster(2, 0xFFFFFFFF);
                SetTemper(1, false);
                SetTemper(2, false);
                if (!FixedGoalChoices.Contains(Goal)) Goal = FixedGoalChoices[0];
                RaisePropertyChanged("InvestigationTitle");
                RaisePropertyChanged("GoalChoices");
                if (index != 0) monname = "Empty";
                return;
            }
            if (monsters[(index + 1) % 3] == monname)
            {
                SwapMonsters(index, (index + 1) % 3);
                RaisePropertyChanged("InvestigationTitle");
            }
            else if (monsters[(index - 1) % 3] == monname)
            {
                SwapMonsters(index, (index - 1) % 3);
                RaisePropertyChanged("InvestigationTitle");
            }
            else
            {
                ForceSetMonster(index, code);
                RaisePropertyChanged("InvestigationTitle");
            }
        }

        public void ForceSetMonster(int index, UInt32 code)
        {
            _inv.SetMonster(index, code);
            RaisePropertyChanged($"Mon{index+1}");
        }
        
        private void SwapMonsters(int i1, int i2)
        {
            UInt32 tempcode = _inv.GetMonsterCode(i1);
            ForceSetMonster(i1,_inv.GetMonsterCode(i2));
            ForceSetMonster(i2,tempcode);
        }

        public void SetDefaultMonsters(string Locale)
        {
            UInt32[] monsters = Investigation.localeToDefault[Locale];
            ForceSetMonster(0, monsters[0]);
            ForceSetMonster(1, monsters[1]);
            ForceSetMonster(2, monsters[2]);
            RaisePropertyChanged(Mon1);
            RaisePropertyChanged(Mon2);
            RaisePropertyChanged(Mon3);
            RaisePropertyChanged(InvestigationTitle);
        }
        #endregion
        
        #region Tempering
        public bool M1Temper
        {
            get => _inv.GetTemper(0);
            set => SetTemper(0, value);
        }
        public bool M2Temper
        {
            get => _inv.GetTemper(1);
            set => SetTemper(1, value);
        }
        public bool M3Temper
        {
            get => _inv.GetTemper(2);
            set => SetTemper(2, value);
        }
        private void SetTemper(int index, bool value)
        {
            _inv.SetTemper(index, value);
            RaisePropertyChanged($"M{index+1}Temper");
            if (InvestigationRank != "Tempered" && value)
            {
                _inv.SetRank(0x02);
                RaisePropertyChanged("InvestigationRank");
                RaisePropertyChanged("MonsterChoices");
                RaisePropertyChanged("LocaleChoices");
                RaiseMonsters();
            }

            if (InvestigationRank == "Tempered" && !M1Temper)
            {
                _inv.SetRank(0x01);
                _inv.SetTemper(1, false);
                _inv.SetTemper(2, false);
                RaisePropertyChanged("M2Temper");
                RaisePropertyChanged("M3Temper");
                RaisePropertyChanged("InvestigationRank");
                RaisePropertyChanged("MonsterChoices");
                RaisePropertyChanged("LocaleChoices");
                RaiseMonsters();
            }
        }
        #endregion
        
        public bool Filled
        {
            get => _inv.GetIsFilled();
        }
        
        public bool Selected
        {
            get => _inv.GetIsSelected();
            set
            {
                _inv.SetSelected(value);
                RaisePropertyChanged("Selected");
            }
        }
        
        public Int32 Attempts
        {
            get => _inv.GetAttempts();
            set
            {
                _inv.SetAttempts(value);
                RaisePropertyChanged("Attempts");
            }
        }        
        
        public bool Seen
        {
            get => _inv.GetSeen();
            set
            {
                _inv.SetSeen(value?(byte)0x03:(byte)0x00);
                RaisePropertyChanged("Seen");
            }
        }

        public byte HP
        {
            get => _inv.GetHP();
            set {
                _inv.SetHP(value);
                RaisePropertyChanged("HP");}
        }
        public byte Attack
        {
            get => _inv.GetAttack();
            set {
                _inv.SetAttack(value);
                RaisePropertyChanged("Attack");}
        }
        public byte Size
        {
            get => _inv.GetSize();
            set {
                _inv.SetSize(value);
                RaisePropertyChanged("Size");}
        }
        public byte X3
        {
            get => _inv.GetX3();
            set {
                _inv.SetX3(value);
                RaisePropertyChanged("X3");}
        }
        public byte Y0
        {
            get => _inv.GetY0();
            set {
                _inv.SetY0(value);
                RaisePropertyChanged("Y0");}
        }
        public byte Y3
        {
            get => _inv.GetY3();
            set {
                _inv.SetY3(value);
                RaisePropertyChanged("Y3");}
        }
        public byte BoxBonus
        {
            get => _inv.GetBoxBonus();
            set {
                _inv.SetBoxBonus(value);
                RaisePropertyChanged("BoxBonus");}
        }
        public byte ZennyBonus
        {
            get => _inv.GetZennyBonus();
            set {
                _inv.SetZennyBonus(value);
                RaisePropertyChanged("ZennyBonus");}
        }

        private byte flourishcode
        {
            get => _inv.GetFlourish();
            set {
                _inv.SetFlourish(value);
                RaisePropertyChanged("Flourish");}
        }

        public string Flourish
        {
            get => Investigation.flourishByLocale[localeToCode[Locale]][flourishcode];
            set
            {
                int code = Array.IndexOf(Investigation.flourishByLocale[localeToCode[Locale]], value);
                flourishcode = (byte) code;
                RaisePropertyChanged("Flourish");
            }
        }
        
        private readonly Dictionary<string, byte> localeToCode = new Dictionary<string,byte>()
        {
            {Investigation.area0, 0x00},
            {Investigation.area1, 0x01},
            {Investigation.area2, 0x02},
            {Investigation.area3, 0x03},
            {Investigation.area4, 0x04}
        };
        
        public string Locale
        {
            get => _inv.GetLocale();
            set
            {
                _inv.SetLocale(localeToCode[value]);
                SetDefaultMonsters(value);
                RaisePropertyChanged("Locale");
                RaisePropertyChanged("MonsterChoices");
                RaisePropertyChanged("Flourish");
                RaisePropertyChanged("CurrentFlourish");
            }
        }

        private static readonly string goal1 = "Slay-Wildlife1";
        private static readonly string goal2 = "Slay-Wildlife2";
        private static readonly string goal3 ="Capture";
        private static readonly string goal4 ="Hunt";

        public string Goal
        {
            get
            {
                int monamount = _inv.GetAmount();
                int time = _inv.GetTime();
                bool capture = _inv.GetCapture();
                string result = "";
                if (monamount == 0)
                {
                    result += !capture ? goal1 : goal2;
                }
                else
                {
                    result += capture ? goal3 : goal4;
                    result += " " + monamount + " Monster";
                    if (monamount > 1) result += "s";
                }

                result += " in " + time + " min";
                return result;
            }
            set
            {
                int monamount;
                int time;
                bool capture;
                string[] parts = value.Split(null);
                if (parts[0]==goal1){monamount = 0;time=50;capture=false;}
                else if (parts[0]==goal2){monamount = 0;time=50;capture=true;}
                else
                {
                    capture = parts[0] == goal3;
                    time = Convert.ToInt32(parts[parts.Length - 2]);
                    monamount = Convert.ToInt32(parts[1]);
                }
                _inv.SetTimeAmount((byte) Array.IndexOf(Investigation.codeToTime, (time, monamount, capture)));
                RaisePropertyChanged("Goal");
                RaisePropertyChanged("InvestigationTitle");
            }
        }

        public int Faints
        {
            get => _inv.GetFaints();
            set
            {
                _inv.SetFaints((byte) Array.IndexOf(Investigation.faintslist,value));
                RaisePropertyChanged("Faints");
            }
        }

        public int PlayerCount
        {
            get => _inv.GetPlayerCount();
            set
            {
                _inv.SetPlayerCount((byte) Array.IndexOf(Investigation.playercount, value));
                RaisePropertyChanged("PlayerCount");
            }
        }
        
        

        public string ToggleState
        {
            get => Filled ? "Clear" : "Initialize";
        }
        
        public void Undo()
        {
            _inv.Undo();
            RaiseAll();
        }

        public void Clear()
        {
            _inv.Clear();
            RaiseAll();
        }
        
        public void Initialize()
        {
            _inv.Initialize();
            RaiseAll();
        }       
        public void Toggle()
        {
            if (Filled) _inv.Clear();
            else _inv.Initialize();
            RaiseAll();
        }

        public byte[] Serialize(){return _inv.Serialize();}
        public void Commit(){_inv.Commit();}
        public string Log(){return _inv.Log();}

        public void RaiseMonsters()
        {
            RaisePropertyChanged("Mon1");
            RaisePropertyChanged("Mon2");
            RaisePropertyChanged("Mon3");
            RaisePropertyChanged("M1Temper");
            RaisePropertyChanged("M2Temper");
            RaisePropertyChanged("M3Temper");
        }
        public void RaiseAll()
        {
            RaisePropertyChanged("Filled");
            RaisePropertyChanged("Selected");
            RaisePropertyChanged("Attempts");
            RaisePropertyChanged("ToggleState");
            RaisePropertyChanged("Seen");
            RaisePropertyChanged("HP");
            RaisePropertyChanged("Attack");
            RaisePropertyChanged("Size");
            RaisePropertyChanged("X3");
            RaisePropertyChanged("Y0");
            RaisePropertyChanged("Y3");
            RaisePropertyChanged("BoxBonus");
            RaisePropertyChanged("ZennyBonus");
            RaisePropertyChanged("Mon1");
            RaisePropertyChanged("Mon2");
            RaisePropertyChanged("Mon3");
            
            RaisePropertyChanged("InvestigationRank");
            RaisePropertyChanged("Locale");
            
            RaisePropertyChanged("MonsterChoices");

            RaisePropertyChanged("M1Temper");
            RaisePropertyChanged("M2Temper");
            RaisePropertyChanged("M3Temper");
            RaisePropertyChanged("Flourish");
            RaisePropertyChanged("GoalChoices");
            RaisePropertyChanged("InvestigationTitle");
            RaisePropertyChanged("LocaleChoices");
        }
        

        public void Overwrite(IList<byte> newestdata)
        {
            _inv.Overwrite(newestdata);
            RaiseAll();
        }

        public ObservableCollection<string> LocaleChoices
        {
            get => new ObservableCollection<string>(InvestigationRank == "Low Rank" ? LowRankChoices : LowRankChoices.Concat(HighRankChoices));
        }

        private static readonly ObservableCollection<string> LowRankChoices = new ObservableCollection<string>
        {
            Investigation.area0, Investigation.area1, Investigation.area2, Investigation.area3
        };
        private static readonly ObservableCollection<string> HighRankChoices = new ObservableCollection<string>
        {
            Investigation.area4
        };

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
        
        private ObservableCollection<string> FixedGoalChoices = new ObservableCollection<string>
        {
            "Hunt 1 Monster in 50 min", "Hunt 1 Monster in 30 min", "Hunt 1 Monster in 15 min"
        };

        private ObservableCollection<string> NonElderGoals = new ObservableCollection<string>
        {
            "Hunt 2 Monsters in 50 min", "Hunt 2 Monsters in 30 min", "Hunt 3 Monsters in 50 min",
            "Slay-Wildlife1 in 50 min", "Slay-Wildlife2 in 50 min",
            "Capture 1 Monster in 50 min", "Capture 1 Monster in 30 min", "Capture 1 Monster in 15 min"
        };

        public ObservableCollection<string> GoalChoices
        {
            get => new ObservableCollection<string>(_inv.IsElderInvestigation() ? FixedGoalChoices : FixedGoalChoices.Concat(NonElderGoals));
        }
        
        private ObservableCollection<string> FixedRankChoices = new ObservableCollection<string>
        {
            "Low Rank", "High Rank", "Tempered"
        };

        public ObservableCollection<string> RankChoices
        {
            get => FixedRankChoices;
        }

        private ObservableCollection<string>[] ChoicesMatrix = new[]
        {
            new ObservableCollection<string> { "Anjanath", "Great Jagras", "Kulu-Ya-Ku", "Pukei-Pukei", "Rathalos", "Rathian", "Tobi-Kadachi"},
            new ObservableCollection<string> { "Barroth", "Diablos", "Jyuratodus", "Kulu-Ya-Ku", "Rathian", "Anjanath" },
            new ObservableCollection<string> { "Legiana", "Paolumu", "Tzitzi-Ya-Ku", "Odogaron", "Kirin"},
            new ObservableCollection<string> { "Great Girros", "Odogaron", "Radobaan"},
            new ObservableCollection<string> { }
        };
        private ObservableCollection<string>[] HighRankMatrix = new[]
        {
            new ObservableCollection<string> { "Azure Rathalos", "Kushala Daora", "Bazelgeuse", "Deviljho", "Empty"},
            new ObservableCollection<string> { "Bazelgeuse", "Black Diablos", "Teostra", "Deviljho", "Empty" },
            new ObservableCollection<string> { "Pink Rathian", "Bazelgeuse", "Deviljho", "Empty" },
            new ObservableCollection<string> { "Bazelgeuse", "Vaal Hazak", "Deviljho", "Empty" },
            new ObservableCollection<string> { "Azure Rathalos", "Bazelgeuse", "Behemoth", "Dodogama", "Deviljho", "Lavasioth", "Uragaan", "Nergigante", "Teostra", "Kushala Daora", "Deviljho", "Lunastra", "Empty"}
        };

        public ObservableCollection<string> MonsterChoices
        {
            get =>  new ObservableCollection<string>(rankToCode[InvestigationRank] == 0
                ? ChoicesMatrix[localeToCode[Locale]]
                : ChoicesMatrix[localeToCode[Locale]].Concat(HighRankMatrix[localeToCode[Locale]]));
        }

        public ObservableCollection<byte> CommonValues
        {
            get => new ObservableCollection<byte>(new byte[] {0, 1, 2, 3, 4, 5});
        }
        public ObservableCollection<byte> BoxValues
        {
            get => new ObservableCollection<byte>(new byte[] {0, 1, 2});
        }
        public ObservableCollection<int> FaintValues
        {
            get => new ObservableCollection<int>(new int[] {5, 3, 2, 1});
        }       
        public ObservableCollection<int> PlayerValues
        {
            get => new ObservableCollection<int>(new int[] {4,2});
        }   
        public ObservableCollection<byte> ZennyValues
        {
            get => new ObservableCollection<byte>(new byte[] {0, 1, 2, 3, 4});
        }

        public ObservableCollection<string> CurrentFlourish
        {
            get => new ObservableCollection<string>(Investigation.flourishByLocale[localeToCode[Locale]]);
        }
        
    }
}
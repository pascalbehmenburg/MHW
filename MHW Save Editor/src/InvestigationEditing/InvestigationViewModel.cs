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
    public class InvestigationViewModel : INotifyPropertyChanged
    {
        public InvestigationViewModel(Investigation entry = null)
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
                if (code == 0x00)_inv.SetRank(0x00);
                if (code == 0x01)_inv.SetRank(0x01);
                if (code == 0x02)_inv.SetRank(0x02);
                RaisePropertyChanged("InvestigationRank");
                RaisePropertyChanged("MonsterChoices");
                RaisePropertyChanged("Mon1");RaisePropertyChanged("Mon2");RaisePropertyChanged("Mon3");
            }
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
            UInt32 code = Investigation.monsterToCode[monname];
            _inv.SetMonster(index, code);
            RaisePropertyChanged($"Mon{index+1}");
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
                
        private static readonly Dictionary<string, byte> localeToCode = new Dictionary<string,byte>()
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
                RaisePropertyChanged("Locale");
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
            get => new ObservableCollection<string>(LowRankChoices.Concat(HighRankChoices));
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
            get => new ObservableCollection<string>(FixedGoalChoices.Concat(NonElderGoals));
        }
        
        private ObservableCollection<string> FixedRankChoices = new ObservableCollection<string>
        {
            "Low Rank", "High Rank", "Tempered"
        };

        public ObservableCollection<string> RankChoices
        {
            get => FixedRankChoices;
        }

        public ObservableCollection<string> MonsterChoices
        {
            get => new ObservableCollection<string>( new string[]
            {
                "Anjanath",
                "Rathalos",
                "Great Jagras",
                "Rathian",
                "Pink Rathian",
                "Azure Rathalos",
                "Diablos",
                "Black Diablos",
                "Kirin",
                "Kushala Daora",
                "Lunastra",
                "Teostra",
                "Lavasioth",
                "Deviljho",
                "Barroth",
                "Uragaan",
                "Pukei-Pukei",
                "Nergigante",
                "Kulu-Ya-Ku",
                "Tzitzi-Ya-Ku",
                "Jyuratodus",
                "Tobi-Kadachi",
                "Paolumu",
                "Legiana",
                "Great Girros",
                "Odogaron",
                "Radobaan",
                "Vaal Hazak",
                "Dodogama",
                "Bazelgeuse",
                "Empty"
            });
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
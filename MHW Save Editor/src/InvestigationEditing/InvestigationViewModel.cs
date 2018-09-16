using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

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
        
        #region Members
        
        public string InvestigationTitle
        {
            get
            {
                if (!_inv.GetIsFilled()) return "Empty Slot";
                string objective = _inv.GetAmount() == 0 ? "Slay" : (_inv.GetCapture() ? "Caputre" : "Hunt");
                string mainmon = _inv.GetAmount()!=0?_inv.GetMonsters()[0].Item1 + (_inv.GetAmount()>1?"...":""):"Wildlife";
                return objective + " " + mainmon;
            }
        }

        private static readonly Dictionary<string, byte> rankToCode = new Dictionary<string, byte>
        {
            {Investigation.ranklist[0], 0x00},
            {Investigation.ranklist[1], 0x01},
            {Investigation.ranklist[2], 0x02}
        };
        
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
            }
        }
        
        private void SetLowRank()
        {
            SetHighRank();
            if (Locale==Investigation.area4){Locale=Investigation.area0;}
            if (_inv.IsElderInvestigation()){SetDefaultMonsters(Locale);}
        }

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

        private void SwapMonsters(int i1, int i2)
        {
            UInt32 tempcode = _inv.GetMonsterCode(i1);
            _inv.SetMonster(i1,_inv.GetMonsterCode(i2));
            _inv.SetMonster(i2,tempcode);
            RaisePropertyChanged("M{i1}");
            RaisePropertyChanged("M{i2}");
        }

        private void SetDefaultMonsters(string locale)
        {
            UInt32[] monsters = Investigation.localeToDefault[locale];
            Mon1 = Investigation.codeToMonster[monsters[0]];
            Mon2 = Investigation.codeToMonster[monsters[1]];
            Mon3 = Investigation.codeToMonster[monsters[2]];
        }
        
        private void SetMonster(int index, string monname)
        {
            UInt32 code = Investigation.monsterToCode[monname];
            if (_inv.IsElder(code))
            {
                _inv.SetMonster(0, code);
                _inv.SetMonster(1, 0xFFFFFFFF);
                _inv.SetMonster(2, 0xFFFFFFFF);
                RaisePropertyChanged("M1");
                RaisePropertyChanged("M2");
                RaisePropertyChanged("M3");
                return;
            }
            string[] monsters = GetMonsters();
            if (monsters[(index + 1) % 2] == monname) SwapMonsters(index, (index + 1) % 2);
            else if (monsters[(index - 1) % 2] == monname) SwapMonsters(index, (index - 1) % 2);
            else
            {
                _inv.SetMonster(index, code);
                RaisePropertyChanged("M{index}");
            }
        }

        private void SetHighRank()
        {
            M1Temper = false;
            M2Temper = false;
            M3Temper = false;
        }

        private void SetTemperedRank()
        {
            Tuple<string, bool>[] Monsters = _inv.GetMonsters();
            M1Temper = true;
        }

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
            RaisePropertyChanged("M{index}Temper");
        }
        
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
                RaisePropertyChanged("Flourish");
                RaisePropertyChanged("Locale");
            }
        }

        private static readonly string goal1 = "Slay-Wildlife1";
        private static readonly string goal2 = "Slay-Wildlife2";
        private static readonly string goal3 ="Capture";
        private static readonly string goal4 ="Hunt";
            
        public string Goal
        {
            get => (_inv.GetAmount() == 0 ? (!_inv.GetCapture()?goal1:goal2) : (_inv.GetCapture() ? goal3 : goal4)+" "+_inv.GetAmount()+" monster"+(_inv.GetAmount()>1?"s":"")) 
                                                                        + " in " + _inv.GetTime() + " min";
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
            }
        }

        public int Faints
        {
            get => _inv.GetFaints();
            set
            {
                _inv.SetFaints((byte) Investigation.faintslist[value]);
                RaisePropertyChanged("Faints");
            }
        }

        public int PlayerCount
        {
            get => _inv.GetPlayerCount();
            set
            {
                _inv.SetPlayerCount((byte) Investigation.playercount[value]);
                RaisePropertyChanged("PlayerCount");
            }
        }
        
        #endregion

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
        

        public void RaiseAll()
        {
            RaisePropertyChanged("InvestigationTitle");
            RaisePropertyChanged("InvestigationRank");
            RaisePropertyChanged("Filled");
            RaisePropertyChanged("ToggleState");
            RaisePropertyChanged("Selected");
            RaisePropertyChanged("Attempts");
            RaisePropertyChanged("Seen");
            RaisePropertyChanged("Locale");
            RaisePropertyChanged("Mon1");
            RaisePropertyChanged("Mon2");
            RaisePropertyChanged("Mon3");
            RaisePropertyChanged("M1Temper");
            RaisePropertyChanged("M2Temper");
            RaisePropertyChanged("M3Temper");
            RaisePropertyChanged("HP");
            RaisePropertyChanged("Attack");
            RaisePropertyChanged("Size");
            RaisePropertyChanged("X3");
            RaisePropertyChanged("Y0");
            RaisePropertyChanged("Y3");
            RaisePropertyChanged("Flourish");
            RaisePropertyChanged("BoxBonus");
            RaisePropertyChanged("ZennyBonus");
        }


        public void Overwrite(IList<byte> newestdata)
        {
            _inv.Overwrite(newestdata);
            RaiseAll();
        }
        
        public ObservableCollection<string> LocaleChoices;

        private static readonly ObservableCollection<string> LowRankChoices = new ObservableCollection<string>
        {
            Investigation.area0, Investigation.area1, Investigation.area2, Investigation.area3
        };
        private static readonly ObservableCollection<string> HighRankChoices = new ObservableCollection<string>
        {
            Investigation.area0, Investigation.area1, Investigation.area2, Investigation.area3, Investigation.area4
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
    }
}
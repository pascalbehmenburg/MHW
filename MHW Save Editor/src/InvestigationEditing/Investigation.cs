using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHW.InvestigationEditing
{
    //position on save file: 3dadb1

    public class Investigation
    {
        private ArraySegmentWrapper<byte> filedata;
        private bool modified;
        private ArraySegmentWrapper<byte> newdata;

        public void __filesystemswap__(Investigation other)
        {
            modified = true;
            other.modified = true;
            ArraySegmentWrapper<byte> temp = other.filedata;
            other.filedata = filedata;
            filedata = temp;
        }
        
        public Investigation(ArraySegmentWrapper<byte> data)
        {
            filedata = data;
            Undo();
        }
      
        #region Getters
        //First match 00001000 3075
        //Delimiter 4 bytes 30 75 00 00
        //if quest is empty FF FF FF FF
        public bool GetIsFilled(){return (modified?newdata:filedata).Slice(0, 4).SequenceEqual(questIsFilled);}
        //01 Selected, 00 Not Selected
        public bool GetIsSelected(){return (modified?newdata:filedata)[4]==1;}
        //4 bytes attempts
        public Int32 GetAttempts(){return BitConverter.ToInt32((modified?newdata:filedata).Slice(5, 9),0);}
        //4 bytes Empty-NotSeen-Seen
        public bool GetSeen(){return BitConverter.ToInt32((modified?newdata:filedata).Slice(9, 13),0)==3;}
        //1 byte Location: 00/01/02/03/04 - AF/WW/CH/RV/ER
        public string GetLocale(){return codeToLocale[(modified?newdata:filedata)[13]];}
        //1 byte Tempered/HR/LR  - 02/01/00
        public string GetRank(){return ranklist[(modified?newdata:filedata)[14]];}
        //4 bytes Monster1
        //4 bytes Monster2
        //4 bytes Monster3
        //1 byte Monster1IsTempered
        //1 byte Monster2IsTempered
        //1 byte Monster3IsTempered   
        public Tuple<string, bool>[] GetMonsters()
        {
            Tuple<string, bool>[] Monsters =
            {
                Tuple.Create(codeToMonster[BitConverter.ToUInt32((modified?newdata:filedata).Slice(15, 19), 0)], (modified?newdata:filedata)[28] != 0),
                Tuple.Create(codeToMonster[BitConverter.ToUInt32((modified?newdata:filedata).Slice(19, 23), 0)], (modified?newdata:filedata)[29] != 0),
                Tuple.Create(codeToMonster[BitConverter.ToUInt32((modified?newdata:filedata).Slice(23, 27), 0)], (modified?newdata:filedata)[30] != 0),
            };
            return Monsters;
        }
        //and X0-3 Y0-3 Z0-3 with the same meaning as in memory for those
        public byte GetHP(){return (modified ? newdata : filedata)[30];}
        public byte GetAttack(){return (modified?newdata:filedata)[31];}
        public byte GetSize (){return (modified?newdata:filedata)[32];}
        public byte GetX3 (){return (modified?newdata:filedata)[33];}
        public byte GetY0 (){return (modified?newdata:filedata)[34];}
        public string GetFlourish (){return LocaleAndFlourish((modified?newdata:filedata)[13],(modified?newdata:filedata)[35]);}
        public string GetTimeAmount (){return codeToTime[(modified?newdata:filedata)[36]];}
        public byte GetY3 (){return (modified?newdata:filedata)[37];}
        public int GetFaints(){return faintslist[(modified?newdata:filedata)[38]];} 
        public int GetPlayerCount(){return playercount[(modified?newdata:filedata)[39]];} 
        public byte GetBoxBonus (){return (modified?newdata:filedata)[40];} 
        public byte GetZennyBonus (){return (modified?newdata:filedata)[41];}
        //box bonus up to 00/01/02, zennybonus up to 04
        
        private bool IsElder(UInt32 monster){return Array.IndexOf(elders, monster ) != -1;} 
        private bool IsElderInvestigation(){return IsElder(BitConverter.ToUInt32((modified?newdata:filedata).Slice(15, 19), 0));}
        
        #endregion

        #region Setters

        //No setter for emptiness, use clear instead
        public Investigation ToggleIsSelected(){
            modified = true;
            newdata[4] = (byte)(newdata[4]^newdata[4]);
            return this;}

        public Investigation SetAttempts(Int32 newvalue)
        {
            modified = true;
            //newdata[5] = (byte) Math.Min(0xFF, newvalue);
            newdata.CopyFrom(BitConverter.GetBytes(newvalue), 5);
            return this;
        }

        public Investigation ToggleSeen()
        {
            modified = true;
            newdata[9] = (byte) (newdata[9] ^ 0x03);;
            return this;
        }

        public Investigation SetLocale(byte newlocale)
        {
            modified = true;
            newdata[13] = newlocale;
            return this;
        }

        public Investigation SetRank(byte newrank)
        {
            modified = true;
            if (newrank == 0) SetLowRank();
            if (newrank == 1) SetHighRank();
            newdata[14] = newrank;
            return this;
        }

        private void SetLowRank()
        {
            SetHighRank();
            if (GetLocale()=="Elder Recess"){SetLocale(0x00);}
            if (IsElderInvestigation()){SetDefaultMonsters(GetLocale());}
        }

        private void SetHighRank()
        {
            Tuple<string, bool>[] Monsters = GetMonsters();
            if (Monsters[0].Item2){newdata[28]=0x00;}
            if (Monsters[1].Item2){newdata[29]=0x00;}
            if (Monsters[2].Item2){newdata[30]=0x00;}
        }

        public Investigation SetMonsters(int index, uint monster)
        {
            modified = true;
            if (IsElder(monster)){SetElder(monster); return this;}
            if (IsElderInvestigation()){SetDefaultMonsters(GetLocale());}
            int match = match = InMonsters(monster);
            if (match!=-1){MonsterSwap(index, match);}
            newdata.CopyFrom(BitConverter.GetBytes(monster),15+index*4);
            return this;
        }

        private int InMonsters(uint monster)
        {
            Tuple<string, bool>[] Monsters = GetMonsters();
            for(int i=0; i<3; i++)if (Monsters[i].Item1==codeToMonster[monster]){return i;}
            return -1;
        }

        private uint MonsterCode(int index)
        {
            return BitConverter.ToUInt32(newdata.Slice(15 + 4 * index, 15 + 4 * index + 4), 0);
        }
        
        private void MonsterSwap(int index1, int index2)
        {
            uint temp = MonsterCode(index1);
            ForceSetMonster(index1, MonsterCode(index2));
            ForceSetMonster(index2, temp);
        }
        
        private void ForceSetMonster(int index, uint monster)
            {
                newdata.CopyFrom(BitConverter.GetBytes(monster), 15+4*index);
            }

        private void SetElder(UInt32 monster)
        {
            ForceSetMonster(0, monster);
            ForceSetMonster(1, 0xFFFFFFF);
            ForceSetMonster(2, 0xFFFFFFF);
        }

        private void SetDefaultMonsters(string Locale)
        {
            UInt32[] defaultmonsters = localeToDefault[Locale];
            ForceSetMonster(0, defaultmonsters[0]);
            ForceSetMonster(1, defaultmonsters[1]);
            ForceSetMonster(2, defaultmonsters[2]);       
        }

    
        public void SetHP(byte HP){
            modified = true;
            newdata[30] = HP;
        }

        public void SetAttack(byte Att)
        {
            modified = true;
            newdata[31] = Att;
        }

        public void SetSize(byte Size)
        {
            modified = true;
            newdata[32] = Size;
        }     

        public void SetX3(byte x3)
        {
            modified = true;
            newdata[33] = x3;
        }                     
        public void SetY0(byte y0)
        {
            modified = true;
            newdata[34] = y0;
        }     
        public void SetFlourish(byte flourish)
        {
            modified = true;
            newdata[35] = flourish;
        }             
        public void SetTimeAmount(byte ta)
        {
            modified = true;
            newdata[36] = ta;
        }    
        public void SetY3(byte y3)
        {
            modified = true;
            newdata[37] = y3;
        }   
        public void SetFaints(byte fa)
        {
            modified = true;
            newdata[38] = fa;
        }   
        public void SetPlayerCount(byte pc)
        {
            modified = true;
            newdata[39] = pc;
        }   
        public void SetBoxBonus(byte bb)
        {
            modified = true;
            newdata[40] = bb;
        }

        public void SetZennyBonuss(byte zb)
        {
            modified = true;
            newdata[41] = zb;
        }

        //box bonus up to 00/01/02, zennybonus up to 04
        #endregion
        
        public Investigation Undo()
        {
            modified = false;
            byte[] tempdata = new byte[filedata.Count];
            for (int i = 0; i < filedata.Count; i++) newdata[i] = filedata[i];
            newdata = new ArraySegmentWrapper<byte>(tempdata, 0, tempdata.Length);
            return this;
        }
        
        private byte[] nullinvestigation= {0xFF,0xFF,0xFF,0xFF,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0xFF,0xFF,
            0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};
        
        private byte[] defaultinvestigation= {0x30,0x75,0x00,0x00,0x00,0x08,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x07,0x00,0x00,0x00,0x18,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00};
        
        public Investigation Clear()
        {
            //Call every single set with the correct clean parameters or just do THIS!
            return Overwrite(nullinvestigation);
        }

        public Investigation Initialize()
        {
            return Overwrite(defaultinvestigation);
        }

        public byte[] Serialize()
        {
            byte[] result = new byte[42];
            newdata.CopyTo(result, 0);
            return result;
        }

        public Investigation Overwrite(IList<byte> newestdata)
        {
            modified = true;
            for (int i = 0; i < filedata.Count; i++) newdata[i] = newestdata[i];
            return this;
        }

        public Investigation Commit()
        {
            if (modified) for (int i = 0; i < newdata.Count; i++) filedata[i] = newdata[i];
            return this;
        }

        public string Log()
        {
            Tuple<string, bool>[] monsters = GetMonsters();
            var builder = new StringBuilder()
                .AppendLine($"_____________________________________{Environment.NewLine}")
                .AppendLine($"Attempts: {GetAttempts()}{Environment.NewLine}")
                .AppendLine($"Locale: {GetLocale()} - {GetFlourish()}{Environment.NewLine}")
                .AppendLine($"Rank: {GetRank()}{Environment.NewLine}")
                .AppendLine($"{(monsters[0].Item2?"Tempered ":"")}{monsters[0].Item1}{Environment.NewLine}")
                .AppendLine($"{(monsters[1].Item2?"Tempered ":"")}{monsters[1].Item1}{Environment.NewLine}")
                .AppendLine($"{(monsters[2].Item2?"Tempered ":"")}{monsters[2].Item1}{Environment.NewLine}")
                .AppendLine($"HP: {GetHP()} - Att: {GetAttack()} - Size: {GetSize()} - X3: {GetX3()}{Environment.NewLine}")
                .AppendLine($"Goal: {GetTimeAmount()}{Environment.NewLine}")
                .AppendLine($"Y0: {GetY0()} - Y3:{GetY3()}{Environment.NewLine}")
                .AppendLine($"Faints: {GetFaints()} - Players: {GetPlayerCount()} - Box Multiplier: {GetBoxBonus()} - Zenny Multiplier: {GetZennyBonus()}{Environment.NewLine}");
            return builder.ToString();
        }
        
        #region DataTranslations
        private byte[] questIsFilled = {0x30, 0x75, 0x00, 0x00};
        private UInt32[] elders = {0x04, 0x0E, 0x10, 0x11, 0x12, 0x19, 0x1A, 0x24, 0x26};

        private static IDictionary<string, UInt32[]> localeToDefault = new Dictionary<string, UInt32[]>()
        {
            {"Ancient Forest", new UInt32[] {0x07, 0x18, 0x00}},
            {"Wildspire Wastes", new UInt32[] {0x15, 0x1D, 0x1B}},
            {"Coral Highlands", new UInt32[] {0x1C,0x1F,0x20}},
            {"Rotten Vale", new UInt32[] {0x23,0x22,0x21}},
            {"Elder Recess", new UInt32[] {0x25,0x16,0x13}}
        };
        
        
        private static IDictionary<UInt32, string> codeToMonster = new Dictionary<UInt32, string>()
        {
            {0x00,"Anjanath"},
            {0x01,"Rathalos"},
            {0x02,"*Aptonoth"},
            {0x03,"*Jagras"},
            {0x04,"Zorah Magdaros"},
            {0x05,"Mosswine"},
            {0x06,"Gajau"},
            {0x07,"Great Jagras"},
            {0x08,"Kestodon(Male)"},
            {0x09,"Rathian"},
            {0x0A,"Pink Rathian"},
            {0x0B,"Azure Rathalos"},
            {0x0C,"Diablos"},
            {0x0D,"Black Diablos"},
            {0x0E,"Kirin"},
            {0x10,"Kushala Daora"},
            {0x11,"Lunastra"},
            {0x12,"Teostra"},
            {0x13,"Lavasioth"},
            {0x14,"Deviljho"},
            {0x15,"Barroth"},
            {0x16,"Uragaan"},
            {0x18,"Pukei"},
            {0x19,"Nergigante"},
            {0x1A,"Xeno'Jiiva"},
            {0x1B,"Kulu-Ya-Ku"},
            {0x1C,"Tzitzi-Ya-Ku"},
            {0x1D,"Jyuratodus"},
            {0x1E,"Tobi-Kadachi"},
            {0x1F,"Paolumu"},
            {0x20,"Legiana"},
            {0x21,"Great Girros"},
            {0x22,"Odogaron"},
            {0x23,"Radobaan"},
            {0x24,"Vaal Hazak"},
            {0x25,"Dodogama"},
            {0x26,"Kulve Taroth"},
            {0x27,"Bazelgeuse"},
            {0x28,"Apceros"},
            {0x29,"Kelbi"},
            {0x2A,"Kelbi(2)"},
            {0x2B,"Hornetaurs"},
            {0x2C,"Vespoid"},
            {0x2D,"Mernos"},
            {0x2E,"Kestodon(Female)"},
            {0x2F,"Raphinos"},
            {0x30,"Shamos"},
            {0x31,"Barnos"},
            {0x32,"Girros"},
            {0x33,"unknown"},
            {0x34,"Gastodons"},
            {0x35,"Noios"},
            {0xFFFFFFFF, "Empty"}
        };

        private static IDictionary<byte, string> codeToLocale = new Dictionary<byte, string>()
        {
            {0x00, "Ancient Forest"},
            {0x01, "Wildspire Wastes"},
            {0x02, "Coral Highlands"},
            {0x03, "Rotten Vale"},
            {0x04, "Elder Recess"}
         };
 
        private string[] flourishByLocale = 
        {
            "Mushrooms", "Flower Beds", "Cactus", "Fruit", "Conch Shells", "Pearl Oysters", "Ancient Fossils",
            "Crimson Fruit", "Amber Deposits", "Beryl Deposits"
        };

        private string[] ranklist =
        {
            "Low Rank", "High Rank", "Tempered"
        };

        private int[] faintslist =
        {
            5, 3, 2, 1
        };

        private int[] playercount =
            {4, 2};

        public string LocaleAndFlourish(byte locale, byte fcode)
        {
            switch (fcode)
            {
                case 0x00:
                    return "Nothing";
                case 0x01:
                case 0x02:
                    return flourishByLocale[locale * 2 + fcode - 1];
                case 0x03:
                    return "Mining Outcrops";
                case 0x04:
                    return "Bonepiles";
                case 0x05:
                    return "Gathering Points";
                default:
                    throw new ArgumentException("Flourish Cannot exceed 0x05");
            }
        }

        private static string[] codeToTime =
        {
            "50min", "30min", "15min", "50min 2Mon", "30min 2Mon", "50min 3Mon",
            "50min Wildlife", "50min Wildlife", "50min Capture", "30min Capture",
            "15min Capture", "0min Capture"
        };
        #endregion
        
    }
    
}
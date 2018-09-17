using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHW.InvestigationEditing
{
    //position on save file: 3dadb1

    public class Investigation
    {
        private byte[] filedata;
        private byte[] newdata;

     
        public Investigation(byte[] data)
        {
            filedata = data;
            newdata = new byte[InvestigationList.inv_size];
            Undo();
        }
      
        #region Getters
        //First match 00001000 3075
        //Delimiter 4 bytes 30 75 00 00
        //if quest is empty FF FF FF FF
        public bool GetIsFilled(){return newdata.Slice(0, 4).SequenceEqual(questIsFilled);}
        //01 Selected, 00 Not Selected
        public bool GetIsSelected(){return newdata[4]==1;}
        //4 bytes attempts
        public Int32 GetAttempts(){return BitConverter.ToInt32(newdata.Slice(5, 9),0);}
        //4 bytes Empty-NotSeen-Seen
        public bool GetSeen(){return BitConverter.ToInt32(newdata.Slice(9, 13),0)==3;}
        //1 byte Location: 00/01/02/03/04 - AF/WW/CH/RV/ER
        public string GetLocale(){return codeToLocale[newdata[13]];}
        //1 byte Tempered/HR/LR  - 02/01/00
        public string GetRank(){return ranklist[newdata[14]];}
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
                Tuple.Create(codeToMonster[BitConverter.ToUInt32(newdata.Slice(15, 19), 0)], newdata[27] != 0),
                Tuple.Create(codeToMonster[BitConverter.ToUInt32(newdata.Slice(19, 23), 0)], newdata[28] != 0),
                Tuple.Create(codeToMonster[BitConverter.ToUInt32(newdata.Slice(23, 27), 0)], newdata[29] != 0),
            };
            return Monsters;
        }
        public UInt32 GetMonsterCode(int index){return BitConverter.ToUInt32(newdata.Slice(15+index*4, 19+index*4), 0);}
        public bool GetTemper(int index){return newdata[27+index] != 0;}
        //and X0-3 Y0-3 Z0-3 with the same meaning as in memory for those
        public byte GetHP(){return newdata[30];}
        public byte GetAttack(){return newdata[31];}
        public byte GetSize (){return newdata[32];}
        public byte GetX3 (){return newdata[33];}
        public byte GetY0 (){return newdata[34];}
        public byte GetFlourish (){return newdata[35];}
        public int GetTime (){return codeToTime[newdata[36]].Time;}
        public int GetAmount (){return codeToTime[newdata[36]].MonAmount;}
        public bool GetCapture (){return codeToTime[newdata[36]].Capture;}
        public string GetTimeAmount(){return codeToTimeAmount[newdata[36]];}
        public byte GetY3 (){return newdata[37];}
        public int GetFaints(){return faintslist[newdata[38]];} 
        public int GetPlayerCount(){return playercount[newdata[39]];} 
        public byte GetBoxBonus (){return newdata[40];} 
        public byte GetZennyBonus (){return newdata[41];}
        //box bonus up to 00/01/02, zennybonus up to 04
        
        public bool IsElder(UInt32 monster){return Array.IndexOf(elders, monster ) != -1;} 
        public bool IsElderInvestigation(){return IsElder(BitConverter.ToUInt32(newdata.Slice(15, 19), 0));}
        
        #endregion

        #region Setters

        //No setter for emptiness, use clear instead
        public Investigation SetSelected(bool selected){
            newdata[4] = (byte)(selected?0x01:0x00);
            return this;}

        public Investigation SetAttempts(Int32 newvalue)
        {
            Array.Copy(BitConverter.GetBytes(newvalue), 0, newdata, 5, 4);
            return this;
        }

        public Investigation SetSeen(byte seen)
        {
            newdata[9] = seen;
            return this;
        }

        public Investigation SetLocale(byte newlocale)
        {
            newdata[13] = newlocale;
            return this;
        }

        public Investigation SetRank(byte newrank)
        {
            newdata[14] = newrank;
            return this;
        }
        
        public void SetMonster(int index, uint monster)
            {
                Array.Copy(BitConverter.GetBytes(monster), 0, newdata, 15+4*index,4);
            }
        
        public void SetTemper(int index, bool temper)
        {
            newdata[27+index]=(byte)(temper?0x01:0x00);
        }
    
        public void SetHP(byte HP){
            newdata[30] = HP;
        }

        public void SetAttack(byte Att)
        {
            newdata[31] = Att;
        }

        public void SetSize(byte Size)
        {
            newdata[32] = Size;
        }     

        public void SetX3(byte x3)
        {
            newdata[33] = x3;
        }                     
        public void SetY0(byte y0)
        {
            newdata[34] = y0;
        }     
        public void SetFlourish(byte flourish)
        {
            newdata[35] = flourish;
        }             
        public void SetTimeAmount(byte ta)
        {
            newdata[36] = ta;
        }    
        public void SetY3(byte y3)
        {
            newdata[37] = y3;
        }   
        public void SetFaints(byte fa)
        {
            newdata[38] = fa;
        }   
        public void SetPlayerCount(byte pc)
        {
            newdata[39] = pc;
        }   
        public void SetBoxBonus(byte bb)
        {
            newdata[40] = bb;
        }

        public void SetZennyBonus(byte zb)
        {
            newdata[41] = zb;
        }

        //box bonus up to 00/01/02, zennybonus up to 04
        #endregion
        
        public Investigation Undo()
        {
            //modified = false;
            for (int i = 0; i < filedata.Length; i++) newdata[i] = filedata[i];
            return this;
        }
        
        public static readonly byte[] nullinvestigation= {0xFF,0xFF,0xFF,0xFF,0x00,0x00,0x00,
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
            //modified = true;
            for (int i = 0; i < newdata.Length; i++) newdata[i] = newestdata[i];
            return this;
        }

        public Investigation Commit()
        {
            for (int i = 0; i < newdata.Length; i++) filedata[i] = newdata[i];//modified
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

        public static readonly IDictionary<string, UInt32[]> localeToDefault = new Dictionary<string, UInt32[]>()
        {
            {"Ancient Forest", new UInt32[] {0x07, 0x18, 0x00}},
            {"Wildspire Wastes", new UInt32[] {0x15, 0x1D, 0x1B}},
            {"Coral Highlands", new UInt32[] {0x1C,0x1F,0x20}},
            {"Rotten Vale", new UInt32[] {0x23,0x22,0x21}},
            {"Elder Recess", new UInt32[] {0x25,0x16,0x13}}
        };
        
        
        public static readonly IDictionary<UInt32, string> codeToMonster = new Dictionary<UInt32, string>()
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
            {0x18,"Pukei-Pukei"},
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
        
        public static readonly IDictionary<string, UInt32> monsterToCode = new Dictionary<string, UInt32>()
        {
            {"Anjanath",0x00},
            {"Rathalos",0x01},
            {"Aptonoth",0x02},
            {"Jagras",0x03},
            {"Zorah Magdaros",0x04},
            {"Mosswine",0x05},
            {"Gajau",0x06},
            {"Great Jagras",0x07},
            {"Kestodon(Male)",0x08},
            {"Rathian",0x09},
            {"Pink Rathian",0x0A},
            {"Azure Rathalos",0x0B},
            {"Diablos",0x0C},
            {"Black Diablos",0x0D},
            {"Kirin",0x0E},
            {"Kushala Daora",0x10},
            {"Lunastra",0x11},
            {"Teostra",0x12},
            {"Lavasioth",0x13},
            {"Deviljho",0x14},
            {"Barroth",0x15},
            {"Uragaan",0x16},
            {"Pukei-Pukei",0x18},
            {"Nergigante",0x19},
            {"Xeno'Jiiva",0x1A},
            {"Kulu-Ya-Ku",0x1B},
            {"Tzitzi-Ya-Ku",0x1C},
            {"Jyuratodus",0x1D},
            {"Tobi-Kadachi",0x1E},
            {"Paolumu",0x1F},
            {"Legiana",0x20},
            {"Great Girros",0x21},
            {"Odogaron",0x22},
            {"Radobaan",0x23},
            {"Vaal Hazak",0x24},
            {"Dodogama",0x25},
            {"Kulve Taroth",0x26},
            {"Bazelgeuse",0x27},
            {"Apceros",0x28},
            {"Kelbi",0x29},
            {"Kelbi(2)",0x2A},
            {"Hornetaurs",0x2B},
            {"Vespoid",0x2C},
            {"Mernos",0x2D},
            {"Kestodon(Female)",0x2E},
            {"Raphinos",0x2F},
            {"Shamos",0x30},
            {"Barnos",0x31},
            {"Girros",0x32},
            {"unknown",0x33},
            {"Gastodons",0x34},
            {"Noios",0x35},
            {"Empty",0xFFFFFFFF}
        };
        

        public static readonly string area0 = "Ancient Forest";
        public static readonly string area1 = "Wildspire Wastes";
        public static readonly string area2 = "Coral Highlands";
        public static readonly string area3 = "Rotten Vale";
        public static readonly string area4 = "Elder Recess";        
        
        private static IDictionary<byte, string> codeToLocale = new Dictionary<byte, string>()
        {
            {0x00, area0},
            {0x01, area1},
            {0x02, area2},
            {0x03, area3},
            {0x04, area4}
         };
 
        
        public static readonly string[][] flourishByLocale = 
        {
            new [] {"Nothing","Mushrooms", "Flower Beds", "Mining Outcrops","Bonepiles","Gathering Points"},
            new [] {"Nothing","Cactus", "Fruit", "Mining Outcrops","Bonepiles","Gathering Points"},
            new [] {"Nothing","Conch Shells", "Pearl Oysters", "Mining Outcrops","Bonepiles","Gathering Points"},
            new [] {"Nothing","Ancient Fossils", "Crimson Fruit", "Mining Outcrops","Bonepiles","Gathering Points"},
            new [] {"Nothing","Amber Deposits", "Beryl Deposits", "Mining Outcrops","Bonepiles","Gathering Points"},
        };

        public static readonly string[] ranklist =
        {
            "Low Rank", "High Rank", "Tempered"
        };

        public static readonly int[] faintslist =
        {
            5, 3, 2, 1
        };

        public static readonly int[] playercount =
            {4, 2};

        public string LocaleAndFlourish(byte locale, byte fcode)
        {
            return flourishByLocale[locale][fcode];
        }

        public static readonly (int Time, int MonAmount, bool Capture)[] codeToTime = 
        {
            (50,1,false), (30,1,false), (15,1,false), 
            (50,2,false), (30,2,false),
            (50,3,false),
            (50,0,false), (50,0,true),
            (50,1,true), (30,1,true), (15,1,true)
        };

        private static string[] codeToTimeAmount =
        {
            "50min Hunt", "30min Hunt", "15min Hunt",
            "50min 2Mon Hunt", "30min 2Mon Hunt", "50min 3Mon Hunt",
            "50min Wildlife", "50min Wildlife2",
            "50min Capture", "30min Capture", "15min Capture"
        };

        #endregion

    }
    
}
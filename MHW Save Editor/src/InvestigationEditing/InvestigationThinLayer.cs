using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHW_Save_Editor.InvestigationEditing
{
    //position on save file: 3dadb1

    public class InvestigationThinLayer
    {
        private byte[] invdata;

     
        public InvestigationThinLayer(byte[] data)
        {
            invdata = data;
        }

        #region Members
        private static readonly byte[] questIsFilled = {0x30, 0x75, 0x00, 0x00};
        public bool Filled
        {
            get => invdata.Slice(0, 4).SequenceEqual(questIsFilled);
        }
        public bool Selected
        {
            get => invdata[4] != 0x00;
            set => invdata[4] = (byte)(value ? 0x01 : 0x00);
        }
        public Int32 Attempts
        {
            get => BitConverter.ToInt32(invdata.Slice(5, 9),0);
            set => Array.Copy(BitConverter.GetBytes(value), 0, invdata, 5, 4);
        }
        public bool Seen
        {
            get => BitConverter.ToInt32(invdata.Slice(9, 13),0)==3;
            set => invdata[9] = (byte) (value?0x03:0x00);
        }
        public Int32 LocaleIndex
        {
            get => invdata[13];
            set => invdata[13] = (byte) value;
        }
        public Int32 Rank
        {
            get => invdata[14];
            set => invdata[14] = (byte) value;
        }
        public UInt32 Mon1
        {
            get => GetMonster(0);
            set => SetMonster(value,0);
        }
        public UInt32 Mon2
        {
            get => GetMonster(1);
            set => SetMonster(value,1);
        }
        public UInt32 Mon3
        {
            get => GetMonster(2);
            set => SetMonster(value,2);
        }
        private UInt32 GetMonster(int index)
        {
            return BitConverter.ToUInt32(invdata.Slice(15+index*4, 19+index*4),0);
        }
        private void SetMonster(UInt32 value, int index)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, invdata, 15 + 4 * index, 4);
        }
        public bool M1Temper
        {
            get => invdata[27]!=0x00;
            set => invdata[27] = (byte) (value ? 0x01 : 0x00);
        }
        public bool M2Temper
        {
            get => invdata[28]!=0x00;
            set => invdata[28] = (byte) (value ? 0x01 : 0x00);
        }
        public bool M3Temper
        {
            get => invdata[29]!=0x00;
            set => invdata[29] = (byte) (value ? 0x01 : 0x00);
        }
        public int HP
        {
            get => invdata[30];
            set => invdata[30] = (byte) value;
        }
        public int Attack
        {
            get => invdata[31];
            set => invdata[31] = (byte) value;
        }
        public int Size
        {
            get => invdata[32];
            set => invdata[32] = (byte) value;
        }
        public int X3
        {
            get => invdata[33];
            set => invdata[33] = (byte) value;
        }
        public int Y0
        {
            get => invdata[34];
            set => invdata[34] = (byte) value;
        }
        public int FlourishIndex
        {
            get => invdata[35];
            set => invdata[35] = (byte) value;
        }
        public int TimeAmountIndex
        {
            get => invdata[36];
            set => invdata[36] = (byte) value;
        }
        public int Y3
        {
            get => invdata[37];
            set => invdata[37] = (byte) value;
        }
        public int FaintIndex
        {
            get => invdata[38];
            set => invdata[38] = (byte) value;
        }
        public int PlayerCountIndex
        {
            get => invdata[39];
            set => invdata[39] = (byte) value;
        }

        public int MonsterRewards
        {
            get => invdata[40];
            set => invdata[40] = (byte) value;
        }

        public int ZennyMultiplier
        {
            get => invdata[41];
            set => invdata[41] = (byte) value;
        }
        #endregion
        
        public static readonly byte[] nullinvestigation= {0xFF,0xFF,0xFF,0xFF,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0xFF,0xFF,
            0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};
        
        public static readonly byte[] defaultinvestigation= {0x30,0x75,0x00,0x00,0x00,0x08,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x07,0x00,0x00,0x00,0x18,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00};
        
        public void Clear() => Overwrite(nullinvestigation);

        public void Initialize() => Overwrite(defaultinvestigation);

        public byte[] Serialize()
        {
            byte[] result = new byte[42];
            invdata.CopyTo(result, 0);
            return result;
        }

        public void Overwrite(IList<byte> newestdata)
        {
            for (int i = 0; i < Investigation.inv_size; i++) invdata[i] = newestdata[i];
        }

    }
    
}
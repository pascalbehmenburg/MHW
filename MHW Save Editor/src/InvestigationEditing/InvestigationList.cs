using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;

namespace MHW.InvestigationEditing
{
    public class InvestigationList
    {
        private Investigation[] invlist;
        private Byte[] clipboard;
        private bool resort_flag;
        private bool name_cache_flag;
        private int current;
        
        private void set_resort(){resort_flag=true;}
        private void set_cache(){name_cache_flag=true;}
        private void set_all(){set_resort();set_cache();}
        private void clear_resort(){resort_flag=false;}
        private void clear_cache(){name_cache_flag=false;}
        private void clear_all(){clear_resort();clear_cache();}

        private static readonly int inv_count = 250;
        private static readonly int inv_size = 42;
        private static readonly byte[]investigation_signature = {
            0x00,0x00,0x10,0x00,0x30,0x75,0x00,0x10,0x00,0x30,0x75
        };
        //investigations are 42 bytes long
        public InvestigationList(byte[] savefile)
        {
            int offset = savefile.StartingIndex(investigation_signature)+4;
            invlist = new Investigation[inv_count];
            for (int i = 0; i < inv_count; i++)
            {
                invlist[i] = new Investigation(new ArraySegmentWrapper<byte>(savefile,offset+i*inv_size,inv_size));
            }
            current = 0;
        }

        public InvestigationList Seek(int i)
        {
            current = Math.Max(0,Math.Min(i, invlist.Length));
            return this;
        }
        
        public InvestigationList Prev(){return Seek(current-1);}
        public InvestigationList First(){return Seek(0);}
        public InvestigationList Next(){return Seek(current+1);}
        public InvestigationList Last(){return Seek(invlist.Length);}

        public InvestigationList ReSort(Func<Investigation , int> function = null)
        {
            if (function==null){function = (x => x.GetIsFilled()?1:0);}
            int[] indices = Enumerable.Range(0, invlist.Length).ToList().OrderByDescending(i => function(invlist[i])).ToArray();
            for (int i = 0; i < invlist.Length; i++)Swap(i, indices[i]);
            return this;
        }

        public InvestigationList Commit()
        {
            if (resort_flag){ReSort();}
            clear_all();
            foreach(Investigation inv in invlist){inv.Commit();}
            return this;
        }
        
        public InvestigationList Toggle(){
            set_all();
            if (invlist[current].GetIsFilled()) invlist[current].Clear();
            else invlist[current].Initialize();
            return this;}
        public InvestigationList InitializeAll(){set_all();foreach(Investigation inv in invlist)if (!inv.GetIsFilled())inv.Initialize();return this;}

        public InvestigationList ClearAll(Func<Investigation, bool> filter =null)
        {
            set_all();
            if (filter==null){filter = (x => true);}
            foreach(Investigation inv in invlist)if(filter(inv))inv.Clear();return this;
        }

        
        public InvestigationList ImportAt(string path, int[] positions)
        {
            byte[] import = File.ReadAllBytes(path);
            if ((import.Length % inv_size)!=0)throw new ConstraintException("File is not a collection of 42-byte investigations");
            if ((import.Length / inv_size)!=positions.Length)throw new ConstraintException("Number of investigations does not match number of replacements");
            set_all();
            for (int i = 0; i < import.Length / inv_size; i++)
            {
                invlist[positions[i]].Overwrite(new ArraySegmentWrapper<Byte>(import, i * inv_size, inv_size));
            }
            return this;
        }

        public InvestigationList ExportAt(string path, int[] positions)
        {
            byte[] content = new byte[inv_size*invlist.Length];
            int i = 0;
            foreach (Investigation inv in invlist)
            {
                inv.Serialize().CopyTo(content, i * inv_size);
                i++;
            }
            File.WriteAllBytes(path,content);
            return this;
        }
        
        public InvestigationList Import(string path){return ImportAt(path, new [] {current});}
        public InvestigationList Export(string path){return ExportAt(path, new [] {current});}

        public InvestigationList Copy()
        {
            clipboard = invlist[current].Serialize();
            return this;
        }
        
        public InvestigationList PasteAt(int[] positions)
        {
            set_all();
            foreach (int i in positions){invlist[i].Overwrite(clipboard);}
            return this;
        }    
        public InvestigationList Paste(){return PasteAt(new [] {current});}

        public InvestigationList GenerateLog(string path)
        {
            var builder = new StringBuilder();
            foreach (Investigation inv in invlist)
            {
                builder.AppendLine(inv.Log());
            }

            File.WriteAllText(path, builder.ToString());
            return this;
        }

        public InvestigationList Prepend(string path)
        {
            int filesize = (int) new FileInfo(path).Length;
            if ((filesize % inv_size)!=0)throw new ConstraintException("File is not a collection of 42-byte investigations");
            return ShiftUp(filesize / inv_size).ImportAt(path, Enumerable.Range(0, filesize).ToArray());
        }

        private InvestigationList ShiftUp(int n)
        {
            int top_pointer = 0;
            int low_pointer = n;
            while (top_pointer > low_pointer)
            {
                Swap(top_pointer, low_pointer);
                top_pointer++;
                low_pointer++;
                if (low_pointer > invlist.Length) low_pointer = n;
            }
            return this;
        }
        
        private InvestigationList Swap(int i, int j)
        {
            invlist[i].__filesystemswap__(invlist[j]);
            Investigation temp = invlist[i];
            invlist[i] = invlist[j];
            invlist[j] = temp;
            return this;
        }
        
        //
    }
}
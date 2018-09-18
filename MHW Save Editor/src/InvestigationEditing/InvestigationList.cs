using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;

namespace MHW.InvestigationEditing
{
    public class InvestigationList:INotifyPropertyChanged
    {
        public ObservableCollection<InvestigationViewModel> InvestigationCollection;
       
        private Byte[] clipboard;
        private Byte[] datasource;
        private int offset;
        
        private int _CurrentIndex;
        public int CurrentIndex
        {
            get { return _CurrentIndex; }
            set
            {
                _CurrentIndex = value;
                RaisePropertyChanged("CurrentIndex");
            }
        }
        private bool initialized;
        
        public static readonly int inv_count = 250;
        public static readonly int inv_size = 42;
        private static readonly byte[]investigation_signature = {
            0x00,0x00,0x10,0x00,0x30,0x75
        };
        
        private static readonly InvestigationViewModel empty_investigation = new InvestigationViewModel();
        
        public InvestigationList()
        {
            initialized = false;
            InvestigationCollection =  new ObservableCollection<InvestigationViewModel>();
            InvestigationCollection.Add(empty_investigation);
        }

        //investigations are 42 bytes long

        public InvestigationList Populate(byte[] savefile)
        {
            datasource = savefile;
            initialized = true;
            InvestigationCollection = new ObservableCollection<InvestigationViewModel>();
            offset = savefile.BMHIndexOf(investigation_signature)+4;
            for (int i = 0; i < inv_count; i++)
            {
                InvestigationCollection.Add(new InvestigationViewModel(new Investigation(savefile.Slice(offset+i*inv_size,offset+(i+1)*inv_size))));
            }
            CurrentIndex = 0;
            return this;
        }

        public InvestigationViewModel Expose()
        {
            return initialized?InvestigationCollection[CurrentIndex]:empty_investigation;
        }

        public InvestigationList Seek(int i)
        {
            CurrentIndex = Math.Max(0,Math.Min(i, inv_count-1));
            return this;
        }
        
        public InvestigationList Prev(){return Seek(CurrentIndex-1);}
        public InvestigationList First(){return Seek(0);}
        public InvestigationList Next(){return Seek(CurrentIndex+1);}
        public InvestigationList Last(){return Seek(inv_count);}

        public InvestigationList ReSort(Func<InvestigationViewModel , int> function = null)
        {
            if (function==null){function = (x => x.Filled?1:0);}
            int[] indices = Enumerable.Range(0, inv_count).ToList().OrderByDescending(i => function(InvestigationCollection[i])).ToArray();
            for (int i = 0; i < inv_count; i++)Swap(i, indices[i]);
            return this;
        }

        public InvestigationList Commit()
        {
            ReSort();
            int i = 0;
            foreach (InvestigationViewModel inv in InvestigationCollection)
            {
                inv.Serialize().CopyTo(datasource, offset + i * inv_size);
                i++;
            }
            return this;
        }
        
        public InvestigationList Toggle()
        {
            InvestigationCollection[CurrentIndex].Toggle();
            return this;
        }
        
        public InvestigationList InitializeAll(){foreach(InvestigationViewModel inv in InvestigationCollection)if (!inv.Filled)inv.Toggle();return this;}

        public InvestigationList ClearAll(Func<InvestigationViewModel, bool> filter =null)
        {
            if (filter==null){filter = (x => true);}
            foreach(InvestigationViewModel inv in InvestigationCollection)if(filter(inv))inv.Clear();return this;
        }

        
        public InvestigationList ImportAt(string path, int[] positions)
        {
            byte[] import = File.ReadAllBytes(path);
            if ((import.Length % inv_size)!=0)throw new ConstraintException("File is not a collection of 42-byte investigations");
            if ((import.Length / inv_size)!=positions.Length)throw new ConstraintException("Number of investigations does not match number of replacements");
            for (int i = 0; i < import.Length / inv_size; i++)
            {
                InvestigationCollection[positions[i]].Overwrite(import.Slice(i * inv_size, i * inv_size+inv_size));
            }
            return this;
        }

        public InvestigationList ExportAt(string path, int[] positions)
        {
            byte[] content = new byte[inv_size*positions.Length];
            for (int i =0; i<positions.Length; i++)
            {
                InvestigationCollection[positions[i]].Serialize().CopyTo(content, i * inv_size);
            }
            File.WriteAllBytes(path,content);
            return this;
        }
        
        public InvestigationList Import(string path){return ImportAt(path, new [] {CurrentIndex});}
        public InvestigationList Export(string path){return ExportAt(path, new [] {CurrentIndex});}

        public InvestigationList Copy()
        {
            clipboard = InvestigationCollection[CurrentIndex].Serialize();
            return this;
        }
        
        public InvestigationList PasteAt(int[] positions)
        {
            if (clipboard == null) return this;
            foreach (int i in positions){InvestigationCollection[i].Overwrite(clipboard);}
            return this;
        }    
        public InvestigationList Paste(){return PasteAt(new [] {CurrentIndex});}

        public InvestigationList GenerateLog(string path)
        {
            var builder = new StringBuilder();
            foreach (InvestigationViewModel inv in InvestigationCollection)
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
                if (low_pointer > inv_size) low_pointer = n;
            }
            return this;
        }
        
        private InvestigationList Swap(int i, int j)
        {
            InvestigationViewModel temp = InvestigationCollection[i];
            InvestigationCollection[i] = InvestigationCollection[j];
            InvestigationCollection[j] = temp;
            return this;
        }
        
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
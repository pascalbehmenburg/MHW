using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace MHW_Save_Editor.Data
{
    public class NotifyUIBase : INotifyPropertyChanged
    {
        // Very minimal implementation of INotifyPropertyChanged matching msdn
        // Note that this is dependent on .net 4.5+ because of CallerMemberName
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}

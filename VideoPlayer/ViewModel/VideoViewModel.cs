using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayer.ViewModel
{
    public class VideoViewModel: INotifyPropertyChanged
    {
        private Model.Contentlist[] _Contentlist;
        public Model.Contentlist[] Contentlist
        {
            get
            {
                return _Contentlist;
            }
            set
            {
                if(_Contentlist != value)
                {
                    _Contentlist = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(Contentlist)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}

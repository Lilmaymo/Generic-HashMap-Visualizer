using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MyHashMape
{
    public class HashBucketView : INotifyPropertyChanged
    {
        public int Index { get; set; }

        private string _key;
        private string _value;
        private string _indexColor = "Black";
        private string _mainBorderColor = "Black";

        public string Key
        {
            get => _key;
            set { _key = value; OnPropertyChanged(nameof(Key)); }
        }

        public string Value
        {
            get => _value;
            set { _value = value; OnPropertyChanged(nameof(Value)); }
        }

        public string IndexColor
        {
            get => _indexColor;
            set { _indexColor = value; OnPropertyChanged(nameof(IndexColor)); }
        }

        public string MainBorderColor
        {
            get => _mainBorderColor;
            set { _mainBorderColor = value; OnPropertyChanged(nameof(MainBorderColor)); }
        }

        public ObservableCollection<ChainEntry> Chain { get; set; } = new ObservableCollection<ChainEntry>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
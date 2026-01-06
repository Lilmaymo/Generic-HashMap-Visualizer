using System.ComponentModel;

namespace MyHashMape
{
    public class ChainEntry : INotifyPropertyChanged
    {
        public string Key { get; set; }
        public string Value { get; set; }

        private string _borderColor = "Gray";
        public string BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; OnPropertyChanged(nameof(BorderColor)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
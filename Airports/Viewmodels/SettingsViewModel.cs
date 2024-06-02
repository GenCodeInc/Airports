using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Airports.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private string _selectedMapType;
        public ObservableCollection<string> MapTypes { get; } = new ObservableCollection<string>
        {
            "Satellite",
            "Street",
            "Hybrid"
        };

        public string SelectedMapType
        {
            get => _selectedMapType;
            set
            {
                _selectedMapType = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

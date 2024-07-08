using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Übung_Gerät.Windows.ViewModel;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler PropertyChanged;

    // Method to raise the PropertyChanged event
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

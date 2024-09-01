using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CSharpExtender.Models
{
    /// <summary>
    /// Base class for observable models (used for simpler property changed notification).
    /// </summary>
    public abstract class ObservableModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T backingField, T value, 
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(backingField, value))
            {
                return false;
            }

            backingField = value;

            OnPropertyChanged(propertyName);

            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

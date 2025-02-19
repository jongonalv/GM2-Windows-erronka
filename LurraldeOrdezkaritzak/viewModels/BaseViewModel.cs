using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LurraldeOrdezkaritzak.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        // Propietatea aldatzen denean gertakizuna (evento) jaurtitzen du
        public event PropertyChangedEventHandler PropertyChanged;

        // Propietate baten aldaketa jakinarazteko metodoa
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Propietate baten balioa eguneratzen du eta aldaketa jakinarazten du
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}

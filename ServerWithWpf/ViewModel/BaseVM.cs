using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace ServerWithWpf.ViewModel;


public class BaseVM : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnChanged([CallerMemberName] string PropertyName = null)
    {
        PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(PropertyName));
    }

    protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnChanged(PropertyName); return true;
    }
}
using System.ComponentModel;
using System.Runtime.CompilerServices;

<<<<<<<< HEAD:ServerWithWpf/ViewModel/BaseVM.cs
namespace ServerWithWpf.ViewModel;
========
namespace ClientWithWpf.ViewModel;
>>>>>>>> ClientWpf:ClientWithWpf/ViewModel/BaseVM.cs

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
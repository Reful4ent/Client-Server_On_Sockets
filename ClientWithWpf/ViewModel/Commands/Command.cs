using System.Windows.Input;

<<<<<<<< HEAD:ServerWithWpf/ViewModel/Commands/Command.cs
namespace ServerWithWpf.ViewModel.Commands;
========
namespace ClientWithWpf.ViewModel.Commands;
>>>>>>>> ClientWpf:ClientWithWpf/ViewModel/Commands/Command.cs

public class Command : ICommand
{
    public event EventHandler? CanExecuteChanged;
    public readonly Action action;

    public Command(Action action) => this.action = action;

    public static Command Create(Action action) => new Command(action);
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        action();
    }
    
}
using System.Windows.Input;


namespace ClientWithWpf.ViewModel.Commands;

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
using System.Windows.Input;

namespace Übung_Gerät.Windows.ViewModel;

public class ViewModelCommand : ICommand
{
    //Fields
    private readonly Action<object> _executeAction;
    private readonly Predicate<object> _canExecuteAction;

    //Constructors
    public ViewModelCommand(Action<object> executeAction)
    {
        _executeAction = executeAction;
        _canExecuteAction = null!;
    }

    public ViewModelCommand(Action<object> executeAction, Predicate<object> canExecuteAction)
    {
        _executeAction = executeAction;
        _canExecuteAction = canExecuteAction;
    }

    //Events
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    //Methods
    public bool CanExecute(object parameter) => _canExecuteAction == null ? true : _canExecuteAction(parameter);

    public void Execute(object parameter) => _executeAction(parameter);
}

// Async RelayCommand
public class AsyncViewModelCommand : ICommand
{
    //Fields
    private readonly Func<object, Task> _execute;
    private readonly Predicate<object> _canExecute;

    //Constructors
    public AsyncViewModelCommand(Func<object, Task> execute, Predicate<object> canExecute = null!)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

    public async void Execute(object parameter) => await _execute(parameter);

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}

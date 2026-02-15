using CommunityToolkit.Mvvm.Input;
using DelegateInjection;

namespace MvvmCommands;

public sealed class RelayCommandBuilder(DelegateInjector injector)
{
    public RelayCommand From(Delegate execute)
    {
        ArgumentNullException.ThrowIfNull(execute, nameof(execute));

        static bool True() => true;

        var injectedDelegate = injector.Apply(execute);

        return injectedDelegate switch
        {
            Action funcTask => new RelayCommand(funcTask, True),

            _ => throw new ArgumentException("Can't create AsyncRelayCommand")
        };
    }

    public RelayCommand From(Delegate execute, Func<bool> canExecute)
    {
        ArgumentNullException.ThrowIfNull(execute, nameof(execute));
        ArgumentNullException.ThrowIfNull(canExecute, nameof(execute));

        var injectedDelegate = injector.Apply(execute);

        return (injectedDelegate, canExecute) switch
        {
            (Action funcTask, Func<bool> check) => new RelayCommand(funcTask, check),

            _ => throw new ArgumentException("Can't create AsyncRelayCommand")
        };
    }

    public AsyncRelayCommand AsyncFrom(Delegate execute)
    {
        ArgumentNullException.ThrowIfNull(execute, nameof(execute));

        static bool True() => true;

        var injectedDelegate = injector.Apply(execute);

        return injectedDelegate switch
        {
            Func<Task> funcTask => new AsyncRelayCommand(funcTask, True),
            Func<CancellationToken, Task> funcCancelTask => new AsyncRelayCommand(funcCancelTask, True),

            _ => throw new ArgumentException("Can't create AsyncRelayCommand")
        };
    }

    public AsyncRelayCommand AsyncFrom(Delegate execute, Func<bool> canExecute)
    {
        ArgumentNullException.ThrowIfNull(execute, nameof(execute));
        ArgumentNullException.ThrowIfNull(canExecute, nameof(execute));

        var injectedDelegate = injector.Apply(execute);

        return (injectedDelegate, canExecute) switch
        {
            (Func<Task> funcTask, Func<bool> check) => new AsyncRelayCommand(funcTask, check),
            (Func<CancellationToken, Task> funcCancelTask, Func<bool> funcCanExecute) => new AsyncRelayCommand(funcCancelTask, funcCanExecute),

            _ => throw new ArgumentException("Can't create AsyncRelayCommand")
        };
    }
}
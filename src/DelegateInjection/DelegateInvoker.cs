namespace DelegateInjection;

public sealed class DelegateInvoker(DelegateInjector delegateInjector)
{
    private readonly DelegateInjector delegateInjector = delegateInjector;

    /// <summary>
    /// Invokes and await  a delegate of type Func<Task>, Func<CancellationToken, Task>, Action or Action<CancellationToken> after applying dependencies from the <see cref="IServiceProvider"/> given in the constructor.
    /// </summary>
    public async Task InvokeAsync(Delegate appliedToDelegate, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(appliedToDelegate);

        var injectedDelegate = this.delegateInjector.Apply(appliedToDelegate);

        await InvokeDelegateAsync(appliedToDelegate, injectedDelegate, cancellationToken);
    }

    private static async Task InvokeDelegateAsync(Delegate appliedToDelegate, Delegate injectedDelegate, CancellationToken cancellationToken)
    {
        Func<Task> invokeableDelegate = injectedDelegate switch
        {
            Func<CancellationToken, Task> funcWithCancellationToken => () => funcWithCancellationToken(cancellationToken),
            Func<Task> funcWithoutCancellationToken => () => funcWithoutCancellationToken(),
            Action<CancellationToken> actionWithCancellationToken => () =>
            {
                actionWithCancellationToken(cancellationToken);
                return Task.CompletedTask;
            }
            ,
            Action actionWithoutCancellationToken => () =>
            {
                actionWithoutCancellationToken(); return Task.CompletedTask;
            }
            ,
            _ => throw new ArgumentException($"Delegate of type {appliedToDelegate.GetType()} is not supported. Only Func<Task>, Func<CancellationToken, Task>, Action and Action<CancellationToken> are supported.")
        };

        await invokeableDelegate();
    }

    /// <summary>
    /// Invokes and await  a delegate of type Func<Task>, Func<CancellationToken, Task>, Action or Action<CancellationToken> after applying dependencies from the <see cref="IServiceProvider"/> given in the constructor.
    /// </summary>
    public async Task InvokeAsync(Delegate appliedToDelegate, CancellationToken cancellationToken, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(appliedToDelegate);
        ArgumentNullException.ThrowIfNull(args);

        var injectedDelegate = this.delegateInjector.Apply(appliedToDelegate, args);

        await InvokeDelegateAsync(appliedToDelegate, injectedDelegate, cancellationToken);
    }

    /// <summary>
    /// Invokes and await  a delegate of type Func<Task>, Func<CancellationToken, Task>, Action or Action<CancellationToken> after applying dependencies from the <see cref="IServiceProvider"/> given in the constructor.
    /// </summary>
    public void Invoke(Delegate injectToDelegate)
    {
        ArgumentNullException.ThrowIfNull(injectToDelegate);

        var injectedDelegate = this.delegateInjector.Apply(injectToDelegate);

        InvokeDelegate(injectToDelegate, injectedDelegate);
    }

    /// <summary>
    /// Invokes and await  a delegate of type Func<Task>, Func<CancellationToken, Task>, Action or Action<CancellationToken> after applying dependencies from the <see cref="IServiceProvider"/> given in the constructor.
    /// </summary>
    public void Invoke(Delegate injectToDelegate, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(injectToDelegate);
        ArgumentNullException.ThrowIfNull(args);

        var injectedDelegate = this.delegateInjector.Apply(injectToDelegate, args);

        InvokeDelegate(injectToDelegate, injectedDelegate);
    }

    private static void InvokeDelegate(Delegate injectToDelegate, Delegate injectedDelegate)
    {
        if (injectedDelegate is Action { } invokableAction)
            invokableAction();
        else
            throw new ArgumentException($"Delegate of type {injectToDelegate.GetType()} is not supported. Only Func<Task>, Func<CancellationToken, Task>, Action and Action<CancellationToken> are supported.");
    }
}
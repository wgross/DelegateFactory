using Microsoft.Extensions.DependencyInjection;
using MvvmCommands;
using DelegateInjection;
using CommunityToolkit.Mvvm.Input;

// Create a dependnecy injecxtton container with the delegate injector, the AsyncRelayCommandBuilder from this example
// and an arbitrary dependency that will be injected

var serviceCollection = new ServiceCollection();
serviceCollection
    .AddDelegateInjection()
    .AddSingleton<RelayCommandBuilder>()
    .AddSingleton<Dependency>();

var serviceProvider = serviceCollection.BuildServiceProvider();

// create acommand with the AsyncRelayCommandBuilder: the executed delegate i jnjected with the Dependency instance form the DI container.
RelayCommand command = serviceProvider.GetRequiredService<RelayCommandBuilder>().From(ExecutedBusinessLogic);

// if the command is executed the execution delegate get the test from the Dependency instance and write it to the console

if (command.CanExecute(default))
    command.Execute(default);

static void ExecutedBusinessLogic(Dependency dependency) => Console.WriteLine("command was executed: " + dependency.GetText());

public class Dependency
{
    public string GetText() => "text from dependency";
}
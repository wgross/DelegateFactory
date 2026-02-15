## About

The project implements dependency injection to delegates with services from an IServiceProvider.
It is inspired by ASP.Net `RequestDelegateInjection` and provides a similar service like this outside of the
specialized scope of web service development with ASP.Net.

## Key Features

- Static factory method receives a `Delegate` instance and an `IServiceProvider` creates a new delegate with injected arguments.
- `DelegateInjector` instance can be added to and retrieved from an `IServiceProvider` as a service itself, injecting dependencies to a delegate from that `IServiceProvider`.
- `DelegateInvoker` instance ca be added to a `IServiceProvider` and uses the `DelegateInjector` to invoke an injected delegate.

## How to Use

Use `DelegateInjector` like `ActivatorUtilities` and inject to delegate with the static factory method:

```csharp
using DelegateInjection;

var serviceProvider = new ServiceCollection()
    .AddSingleton<IDependency,Dependency>()
    .BuildServiceProvider();

void ADelegateImplementation(int number, IDependency dep) {...}

// IDependency will be injected, the delegate argument of type int remains
var injectedDelegate = DelegateInjection.Apply<Action<int>>(ADelegateImplementation, serviceProvider);
 
injectedDelegate(1);
```

Add `DelegateInjection` to the service collection and retrieve it as a service:

```csharp
using DelegateInjection;

var serviceProvider = new ServiceCollection()
    .AddDelegateInjection()
    .AddSingleton<IDependency,Dependency>()
    .BuildServiceProvider();

int ADelegateImplementation(int number, IDependency dep) {..};

// IDependency will be injected, the delegate argument and return value of type int remains
var injectedDelegate = serviceProvider.GetRequiredService<DelegateInjector>().Apply<Func<int,int>>(ADelegateImplementation);
 
var result = injectedDelegate(1);
```

Add `DelegateInjection` to the service and invoke the injected delegate:

```csharp
using DelegateInjection;

var serviceProvider = new ServiceCollection()
    .AddDelegateInjection()
    .AddSingleton<IDependency,Dependency>()
    .BuildServiceProvider();

Task ADelegateImplementation(IDependency dep) {..};

// IDependency will be injected, the delegate argument and return value of type int remains
await serviceProvider.GetRequiredService<DelegateInvoker>().InvokeAsync(ADelegateImplementation);
```
Invoking of the delegate is supported for delegates of type:
 - `Action` (using `DelegateInvoker.InvokeAsync` or `DelegateInvoker.Invoke`)
 - `Action<CancellationToken>`
 - `Func<Task>`
 - `Func<CancellationToken,Task>`

 ### Advanced examples
 
 The project in `examples/MvvmCommands` shown an advanced use case. 
 A `RelayCommand` from the Mvvm Community extension is build with a delegate that requires a dependency fom the service provider. 
 Instead of injecting it as well in the view model and keeping it there in a member variable the `RelayCommandBuider` injects it to the delegate given to the command for execution directly.
 
 ```csharp
 static void ExecutedBusinessLogic(Dependency dependency) => Console.WriteLine("command was executed: " + dependency.GetText());

 RelayCommand command = serviceProvider.GetRequiredService<RelayCommandBuilder>().From(ExecutedBusinessLogic);
 ```

The same could be done for the Can-Execute handler. 
By using this command bulder the number of dependencies injected in the view model can be reduced. The dependencies for the business logic are provided by the RelayCommand builder to the executed delegate instead of the view model.

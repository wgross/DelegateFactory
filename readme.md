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

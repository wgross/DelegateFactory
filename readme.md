## About

Implements dependency injection to delegates with services from an IServiceProvider.
`DelegateInjection` was inspired by ASP.Net `RequestDelegateInjection` and provides a similar service like this outside of the
specialized scope of web service development.

## Key Features

- Static factory method receives a `Delegate` instance and an `IServiceProvider` creates a new delegate with injected arguments.
- DelegateInjection instance can be added to and retrieved from an `IServiceProvider` as a service itself, injecting dependencies to a delegate from that `IServiceProvider`.

## How to Use

Use `DelegateInjection` like `ActivatorUtilities` and inject to delegate with the static factory method:

```csharp
using DelegateInjection;

var serviceProvider = new ServiceCollection().AddSingleton<IDependency,Dependency>().BuildServiceProvider();

void ADelegateImplementation(int number, IDependency dep) {...}

// IDependency will be injected, the delegate argument of type int remains
var injectedDelegate = DelegateInjection.Apply<Action<int>>(ADelegateImplementation, serviceProvider);
 
injectedDelegate(1);
```

Add `DelegateInjection` to the service collection and retrieve it as a service:

```csharp
using DelegateInjection;

var serviceProvider = new ServiceCollection()
    .AddSingleton<IDependency,Dependency>()
    .AddSingleton<DelegateInjection>()
    .BuildServiceProvider();

int ADelegateImplementation(int number, IDependency dep) {..};

// IDependency will be injected, the delegate argument and return value of type int remains
var injectedDelegate = serviceProvider.GetRequiredService<DelegateInjection>().Apply<Func<int,int>>(ADelegateImplementation);
 
var result = injectedDelegate(1);
```

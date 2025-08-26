## About

Supports dependency injection to delegates by injecting arguments of a given delegate with services from an IServiceProvider.
`DelegateFactory` was inspired by ASP.Net `RequestDelegateFactory` and provides a similar service like this outside of the
specialized scope of web ser ver development.

## Key Features

- Static factory method receives a `Delegate` instance and an `IServiceProvider` creates a new delegate with injected arguments.
- Factory instance can be added to and retrieved from a an `IServiceProvider` as a service itself, ijectin to delegate from that `IServiceProvider`.

## How to Use

Use `DelegateFactory` like `ActivatorUtilities` and inject to delegate with the static factory method:

```csharp
using DelegateFactory;

var serviceProvider = new ServiceCollection().AddSingleton<IDependency,Dependency>().BuildServiceProvider();

void ADelegateImplementation(int number, IDependency dep) {...}

// IDependency will be injected, the delegate argument of type int remains
var injectedDelegate = DelegateFactory.Apply<Action<int>>(ADelegateImplementation, serviceProvider);
 
injectedDelegate(1);
```

Add `DelegateFactory` to the service collection and retrieve it as a service:

```csharp
using DelegateFactory;

var serviceProvider = new ServiceCollection()
    .AddSingleton<IDependency,Dependency>()
    .AddSingleton<DelegateFactory>()
    .BuildServiceProvider();

int ADelegateImplementation(int number, IDependency dep) {..};

// IDependency will be injected, the delegate argument and return value of type int remains
var injectedDelegate = serviceProvider.GetRequiredService<DelegateFactory>().Apply<Func<int,int>>(ADelegateImplementation);
 
var result = injectedDelegate(1);
```

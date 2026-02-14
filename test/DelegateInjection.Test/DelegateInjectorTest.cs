using Microsoft.Extensions.DependencyInjection;

namespace DelegateInjection.Test;

public class DelegateInjectorTest
{
    private readonly IServiceCollection serviceCollection;

    public DelegateInjectorTest()
    {
        this.serviceCollection = new ServiceCollection().AddDelegateInjection();
    }

    public class Dependency
    {
        public int Value { get; set; } = 0;
    }

    private static Dependency global = new();

    [Fact]
    public void Apply_static_Action()
    {
        // ARRANGE
        static void Test() => global.Value = 1;

        // ACT
        var result = DelegateInjector.Apply<Action>(Test, serviceCollection.BuildServiceProvider());
        result();

        // ASSERT
        True(global is { Value: 1 });
    }

    [Fact]
    public void Apply_instance_Action()
    {
        // ARRANGE
        void Test() => global.Value = 2;

        // ACT
        var result = DelegateInjector.Apply<Action>(Test, serviceCollection.BuildServiceProvider());
        result();

        // ASSERT
        True(global is { Value: 2 });
    }

    [Fact]
    public void Apply_static_Action_p1()
    {
        // ARRANGE
        static void Test(int i) => global.Value = i + 1;

        // ACT
        var result = new DelegateInjector(serviceCollection.BuildServiceProvider()).Apply<Action<int>>(Test);
        result(99);

        // ASSERT
        True(global is { Value: 100 });
    }

    [Fact]
    public void Apply_instance_Action_p1()
    {
        // ARRANGE
        void Test(int i) { global.Value = i + 1; }

        // ACT
        var result = new DelegateInjector(serviceCollection.BuildServiceProvider()).Apply<Action<int>>(Test);
        result(999);

        // ASSERT
        True(global is { Value: 1000 });
    }

    [Fact]
    public void Apply_static_Action_DI()
    {
        // ARRANGE
        Dependency injected = new();

        static void Test(Dependency d)
        {
            d.Value = 1;
        }

        this.serviceCollection.AddTransient(sp => injected);

        // ACT
        var result = new DelegateInjector(serviceCollection.BuildServiceProvider()).Apply<Action>(Test);
        result();

        // ASSERT
        True(injected is { Value: 1 });
    }

    [Fact]
    public void Apply_instance_Action_DI()
    {
        // ARRANGE
        Dependency? injected = null;

        void Test(Dependency d)
        {
            injected = d;
            injected.Value = 1;
        }

        this.serviceCollection.AddTransient<Dependency>();

        // ACT
        var result = new DelegateInjector(serviceCollection.BuildServiceProvider()).Apply<Action>(Test);
        result();

        // ASSERT
        True(injected is { Value: 1 });
    }

    [Fact]
    public void Apply_static_Action_DI_p1()
    {
        // ARRANGE
        Dependency injected = new();

        static void Test(int i, Dependency d) => d.Value = i + 1;

        this.serviceCollection.AddTransient(sp => injected);

        // ACT
        var result = new DelegateInjector(serviceCollection.BuildServiceProvider()).Apply<Action<int>>(Test);

        result(99);

        // ASSERT
        True(injected is { Value: 100 });
    }

    [Fact]
    public void Apply_instance_Action_DI_p1()
    {
        // ARRANGE
        Dependency? injected = new();

        void Test(int i, Dependency d) => d.Value = i + 1;

        this.serviceCollection.AddTransient(sp => injected);

        // ACT
        var result = new DelegateInjector(serviceCollection.BuildServiceProvider()).Apply<Action<int>>(Test);
        result(99);

        // ASSERT
        True(injected is { Value: 100 });
    }

    [Fact]
    public void Apply_static_Func_DI_p1()
    {
        // ARRANGE
        Dependency injected = new();

        static int Test(int i, Dependency d)
        {
            d.Value = i + 2;
            return d.Value + 1;
        }

        this.serviceCollection.AddTransient(sp => injected);

        // ACT
        var result = new DelegateInjector(serviceCollection.BuildServiceProvider()).Apply<Func<int, int>>(Test);
        var retVal = result(88);

        // ASSERT
        True(injected is { Value: 90 });
        Equal(91, retVal);
    }

    [Fact]
    public void Apply_static_Func_DI_p1_args()
    {
        // ARRANGE
        Dependency injected = new();
        Dependency? injectedArg = new();

        static int Test(int i, Dependency d)
        {
            d.Value = i + 2;
            return d.Value + 1;
        }

        this.serviceCollection.AddTransient(sp => injected);

        // ACT
        var result = new DelegateInjector(serviceCollection.BuildServiceProvider()).Apply<Func<int, int>>(Test, injectedArg);
        var retVal = result(88);

        // ASSERT
        True(injectedArg is { Value: 90 });
        Equal(91, retVal);
    }

    [Fact]
    public void Apply_instance_Func_DI_p1_args()
    {
        // ARRANGE
        Dependency? injected = null;
        Dependency? injectedArg = new();

        int Test(int i, Dependency d)
        {
            injected = d;
            injected.Value = i + 2;
            return injected.Value + 1;
        }

        this.serviceCollection.AddTransient<Dependency>();

        // ACT
        var result = new DelegateInjector(serviceCollection.BuildServiceProvider()).Apply<Func<int, int>>(Test, injectedArg);
        var retVal = result(98);

        // ASSERT
        Same(injectedArg, injected);
        True(injectedArg is { Value: 100 });
        Equal(101, retVal);
    }

    [Fact]
    public async Task Apply_static_Func_DI_p_async()
    {
        // ARRANGE
        Dependency? injected = new();

        static async Task<int> Test(int i, Dependency d)
        {
            await Task.Delay(100);
            d.Value = i + 3;
            return d.Value + 1;
        }

        this.serviceCollection.AddTransient(sp => injected);

        // ACT
        var result = new DelegateInjector(serviceCollection.BuildServiceProvider()).Apply<Func<int, Task<int>>>(Test);
        var output = await result(97);

        // ASSERT
        True(injected is { Value: 100 });
        Equal(101, output);
    }

    [Fact]
    public async Task Apply_instance_Func_DI_p_async()
    {
        // ARRANGE
        Dependency? injected = null;

        async Task<int> Test(int i, Dependency d)
        {
            await Task.Delay(100);
            injected = d;
            injected.Value = i + 3;
            return injected.Value + 1;
        }
        this.serviceCollection.AddTransient<Dependency>();

        // ACT
        var result = new DelegateInjector(serviceCollection.BuildServiceProvider()).Apply<Func<int, Task<int>>>(Test);
        var output = await result(97);

        // ASSERT
        True(injected is { Value: 100 });
        Equal(101, output);
    }

    [Fact]
    public async Task Dependency_is_injected_once_on_creation()
    {
        // ARRANGE
        Dependency? injected = null;

        async Task<int> Test(int i, Dependency d)
        {
            await Task.Delay(100);
            injected = d;
            injected.Value = i + 3;
            return injected.Value + 1;
        }

        this.serviceCollection.AddTransient<Dependency>();

        // ACT
        var result = new DelegateInjector(serviceCollection.BuildServiceProvider()).Apply<Func<int, Task<int>>>(Test);
        var _ = await result(97);
        var output = await result(97);

        // ASSERT
        True(injected is { Value: 100 });
        Equal(101, output);
    }
}
using Microsoft.Extensions.DependencyInjection;

using static Xunit.Assert;

namespace DelegateFactory.Test;

public class DelegateFactoryTest
{
    private readonly ServiceCollection serviceCollection;

    public DelegateFactoryTest()
    {
        this.serviceCollection = new ServiceCollection();
    }

    static int globalWasCalled = 0;

    [Fact]
    public void Apply_static_Action()
    {
        // ARRANGE
        globalWasCalled = 0;
        void Test() { globalWasCalled++; }

        // ACT
        var result = DelegateFactory.Apply<Action>(Test, serviceCollection.BuildServiceProvider());
        result();

        // ASSERT
        Equal(1, globalWasCalled);
    }

    [Fact]
    public void Apply_instance_Action()
    {
        // ARRANGE
        int wasCalled = 0;

        void Test() { wasCalled++; }

        // ACT
        var result = DelegateFactory.Apply<Action>(Test, serviceCollection.BuildServiceProvider());
        result();

        // ASSERT
        Equal(1, wasCalled);
    }

    [Fact]
    public void Apply_static_Action_p1()
    {
        // ARRANGE
        globalWasCalled = 0;
        void Test(int i) { globalWasCalled = i; }

        // ACT
        var result = new DelegateFactory(serviceCollection.BuildServiceProvider()).Apply<Action<int>>(Test);
        result(99);

        // ASSERT
        Equal(99, globalWasCalled);
    }

    [Fact]
    public void Apply_instance_Action_p1()
    {
        // ARRANGE
        int wasCalled = 0;

        void Test(int i) { wasCalled = i; }

        // ACT
        var result = new DelegateFactory(serviceCollection.BuildServiceProvider()).Apply<Action<int>>(Test);
        result(99);

        // ASSERT
        Equal(99, wasCalled);
    }

    public class IntProvider
    {
        private int i = 0;

        public int Get() => ++i;
    }

    [Fact]
    public void Apply_static_Action_DI()
    {
        // ARRANGE
        globalWasCalled = 0;

        void Test(IntProvider intProvider) { globalWasCalled = intProvider.Get(); }

        this.serviceCollection.AddTransient<IntProvider>();
        // ACT
        var result = new DelegateFactory(serviceCollection.BuildServiceProvider()).Apply<Action>(Test);
        result();

        // ASSERT
        Equal(1, globalWasCalled);
    }

    [Fact]
    public void Apply_instance_Action_DI()
    {
        // ARRANGE
        int wasCalled = 0;

        void Test(IntProvider intProvider) { wasCalled = intProvider.Get(); }

        this.serviceCollection.AddTransient<IntProvider>();

        // ACT
        var result = new DelegateFactory(serviceCollection.BuildServiceProvider()).Apply<Action>(Test);
        result();

        // ASSERT
        Equal(1, wasCalled);
    }

    [Fact]
    public void Apply_static_Action_DI_p1()
    {
        // ARRANGE
        globalWasCalled = 0;

        void Test(int i, IntProvider intProvider) { globalWasCalled = intProvider.Get() + i; }

        this.serviceCollection.AddTransient<IntProvider>();
        // ACT
        var result = new DelegateFactory(serviceCollection.BuildServiceProvider()).Apply<Action<int>>(Test);
        result(99);

        // ASSERT
        Equal(100, globalWasCalled);
    }

    [Fact]
    public void Apply_instance_Action_DI_p1()
    {
        // ARRANGE
        int wasCalled = 0;

        void Test(int i, IntProvider intProvider) { wasCalled = intProvider.Get() + i; }

        this.serviceCollection.AddTransient<IntProvider>();

        // ACT
        var result = new DelegateFactory(serviceCollection.BuildServiceProvider()).Apply<Action<int>>(Test);
        result(99);

        // ASSERT
        Equal(100, wasCalled);
    }

    [Fact]
    public void Apply_static_Func_DI_p1()
    {
        // ARRANGE
        globalWasCalled = 0;

        int Test(int i, IntProvider intProvider) { globalWasCalled = intProvider.Get() + i; return globalWasCalled; }

        this.serviceCollection.AddTransient<IntProvider>();

        // ACT
        var result = new DelegateFactory(serviceCollection.BuildServiceProvider()).Apply<Func<int, int>>(Test);
        var output = result(99);

        // ASSERT
        Equal(100, globalWasCalled);
        Equal(100, output);
    }

    [Fact]
    public void Apply_instance_Func_DI_p1()
    {
        // ARRANGE
        int wasCalled = 0;

        int Test(int i, IntProvider intProvider) { wasCalled = intProvider.Get() + i; return wasCalled; }

        this.serviceCollection.AddTransient<IntProvider>();

        // ACT
        var result = new DelegateFactory(serviceCollection.BuildServiceProvider()).Apply<Func<int, int>>(Test);
        var output = result(99);

        // ASSERT
        Equal(100, wasCalled);
        Equal(100, output);
    }

    [Fact]
    public async Task Apply_static_Func_DI_p_async()
    {
        // ARRANGE
        globalWasCalled = 0;

        async Task<int> Test(int i, IntProvider intProvider)
        {
            await Task.Delay(100);
            globalWasCalled = intProvider.Get() + i; return globalWasCalled;
        }

        this.serviceCollection.AddTransient<IntProvider>();

        // ACT
        var result = new DelegateFactory(serviceCollection.BuildServiceProvider()).Apply<Func<int, Task<int>>>(Test);
        var output = await result(99);

        // ASSERT
        Equal(100, globalWasCalled);
        Equal(100, output);
    }

    [Fact]
    public async Task Apply_instance_Func_DI_p_async()
    {
        // ARRANGE
        int wasCalled = 0;

        async Task<int> Test(int i, IntProvider intProvider)
        {
            await Task.Delay(100);
            wasCalled = intProvider.Get() + i; return wasCalled;
        }

        this.serviceCollection.AddTransient<IntProvider>();

        // ACT
        var result = new DelegateFactory(serviceCollection.BuildServiceProvider()).Apply<Func<int, Task<int>>>(Test);
        var output = await result(99);

        // ASSERT
        Equal(100, wasCalled);
        Equal(100, output);
    }

    [Fact]
    public async Task Dependency_is_injected_once_on_creation()
    {
        // ARRANGE
        int wasCalled = 0;

        async Task<int> Test(int i, IntProvider intProvider)
        {
            await Task.Delay(100);
            wasCalled = intProvider.Get() + i; return wasCalled;
        }

        this.serviceCollection.AddTransient<IntProvider>();

        // ACT
        var result = new DelegateFactory(serviceCollection.BuildServiceProvider()).Apply<Func<int, Task<int>>>(Test);
        var _ = await result(99);
        var output = await result(99);

        // ASSERT
        Equal(101, wasCalled);
        Equal(101, output);
    }
}
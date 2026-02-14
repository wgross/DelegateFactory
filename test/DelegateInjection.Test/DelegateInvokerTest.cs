using Microsoft.Extensions.DependencyInjection;

namespace DelegateInjection.Test;

public class DelegateInvokerTest
{
    private readonly ServiceProvider serviceProvider;

    public DelegateInvokerTest()
    {
        this.serviceProvider = new ServiceCollection()
            .AddDelegateInjection()
            .AddTransient<Dependency>()
            .BuildServiceProvider();
    }

    public class Dependency()
    {
        public int Value { get; set; } = 0;
    }

    [Fact]
    public void Invokes_action()
    {
        // ARRANGE
        Dependency? injected = null;
        void action(Dependency d) => injected = d;

        // ACT
        this.serviceProvider.GetRequiredService<DelegateInvoker>().Invoke(action);

        // ASSERT
        True(injected is not null);
    }

    [Fact]
    public void Invokes_action_with_arg()
    {
        // ARRANGE
        Dependency? injected = null;
        void action(Dependency d, int i)
        {
            injected = d;
            injected.Value = i;
        }

        // ACT
        this.serviceProvider.GetRequiredService<DelegateInvoker>().Invoke(action, 10);

        // ASSERT
        True(injected is { Value: 10 });
    }

    [Fact]
    public async Task Invokes_action_async()
    {
        // ARRANGE
        Dependency? injected = null;
        void action(Dependency d) => injected = d;

        // ACT
        await this.serviceProvider.GetRequiredService<DelegateInvoker>().InvokeAsync(action, TestContext.Current.CancellationToken);

        // ASSERT
        True(injected is not null);
    }

    [Fact]
    public async Task Invokes_action_async_with_args()
    {
        // ARRANGE
        Dependency? injected = null;
        void action(Dependency d) => injected = d;

        // ACT
        await this.serviceProvider.GetRequiredService<DelegateInvoker>().InvokeAsync(action, TestContext.Current.CancellationToken, 1);

        // ASSERT
        True(injected is not null);
    }


    [Fact]
    public async Task Invokes_action_with_cancellation()
    {
        // ARRANGE
        Dependency? injected = null;
        void action(Dependency d, CancellationToken _) => injected = d;

        // ACT
        await this.serviceProvider.GetRequiredService<DelegateInvoker>().InvokeAsync(action, TestContext.Current.CancellationToken);

        // ASSERT
        True(injected is not null);
    }

    [Fact]
    public async Task Invokes_async_action()
    {
        // ARRANGE
        Dependency? injected = null;
        async Task action(Dependency d)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            injected = d;
        }

        // ACT
        await this.serviceProvider.GetRequiredService<DelegateInvoker>().InvokeAsync(action, TestContext.Current.CancellationToken);

        // ASSERT
        True(injected is not null);
    }

    [Fact]
    public async Task Invokes_async_action_with_cancellation()
    {
        // ARRANGE
        Dependency? injected = null;
        async Task action(Dependency d, CancellationToken ct)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            injected = d;
        }

        // ACT
        await this.serviceProvider.GetRequiredService<DelegateInvoker>().InvokeAsync(action, TestContext.Current.CancellationToken);

        // ASSERT
        True(injected is not null);
    }
}
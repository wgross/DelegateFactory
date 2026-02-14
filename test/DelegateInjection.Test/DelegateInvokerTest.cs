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
    { }

    [Fact]
    public void Invokes_action()
    {
        // ARRANGE
        bool invoked = true;
        void action(Dependency d) => invoked = d is not null;

        // ACT
        this.serviceProvider.GetRequiredService<DelegateInvoker>().Invoke(action);

        // ASSERT
        True(invoked);
    }

    [Fact]
    public async Task Invokes_action_async()
    {
        // ARRANGE
        bool invoked = true;
        void action(Dependency d) => invoked = d is not null;

        // ACT
        await this.serviceProvider.GetRequiredService<DelegateInvoker>().InvokeAsync(action, TestContext.Current.CancellationToken);

        // ASSERT
        True(invoked);
    }

    [Fact]
    public async Task Invokes_action_with_cancellation()
    {
        // ARRANGE
        bool invoked = true;
        void action(Dependency d, CancellationToken _) => invoked = d is not null;

        // ACT
        await this.serviceProvider.GetRequiredService<DelegateInvoker>().InvokeAsync(action, TestContext.Current.CancellationToken);

        // ASSERT
        True(invoked);
    }

    [Fact]
    public async Task Invokes_async_action()
    {
        // ARRANGE
        bool invoked = true;
        async Task action(Dependency d)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            invoked = d is not null;
        }

        // ACT
        await this.serviceProvider.GetRequiredService<DelegateInvoker>().InvokeAsync(action, TestContext.Current.CancellationToken);

        // ASSERT
        True(invoked);
    }

    [Fact]
    public async Task Invokes_async_action_with_cancellation()
    {
        // ARRANGE
        bool invoked = true;
        async Task action(Dependency d, CancellationToken ct)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            invoked = d is not null;
        }

        // ACT
        await this.serviceProvider.GetRequiredService<DelegateInvoker>().InvokeAsync(action, TestContext.Current.CancellationToken);

        // ASSERT
        True(invoked);
    }
}
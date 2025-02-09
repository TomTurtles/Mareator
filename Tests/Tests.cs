
namespace Tests;

[TestClass]
public sealed class Tests
{
    [TestMethod]
    public void _1_GetMareatorInstance_ShouldWorkAsync()
    {
        var services = new ServiceCollection()
            .AddMareator();

        var provider = services.BuildServiceProvider();

        var mareator = provider.GetService<IMareator>();

        Assert.IsNotNull(mareator);
        Assert.IsInstanceOfType<Mareator.Mareator>(mareator);
    }

    [TestMethod]
    public async Task _2_Send_ShouldWorkAsync()
    {
        Assembly[] assemblies = [
            Assembly.GetExecutingAssembly()
        ];

        var services = new ServiceCollection()
            .AddMareator(assemblies);

        var provider = services.BuildServiceProvider();
        var mareator = provider.GetRequiredService<IMareator>();

        var throwsException = false;
        try
        {
            await mareator.SendAsync(new TestEvent());
        }
        catch (Exception ex)
        {
            throwsException = true;
        }

        Assert.IsFalse(throwsException);
    }

    [TestMethod]
    public async Task _3_SendWithReturnValue_ShouldWorkAsync()
    {
        Assembly[] assemblies = [
            Assembly.GetExecutingAssembly()
        ];

        var services = new ServiceCollection()
            .AddMareator(assemblies);

        var provider = services.BuildServiceProvider();
        var mareator = provider.GetRequiredService<IMareator>();

        var throwsException = false;
        var result = 0;
        try
        {
            result = await mareator.SendAsync<TestEvent2, int>(new TestEvent2());
        }
        catch (Exception ex)
        {
            throwsException = true;
        }

        Assert.IsFalse(throwsException);
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public async Task _4_Publish_ShouldWorkAsync()
    {
        Assembly[] assemblies = [
            Assembly.GetExecutingAssembly()
        ];

        var services = new ServiceCollection()
            .AddMareator(assemblies);

        var provider = services.BuildServiceProvider();
        var mareator = provider.GetRequiredService<IMareator>();

        var throwsException = false;
        var result = 0;
        try
        {
            await mareator.PublishAsync(new TestEvent3());
        }
        catch (Exception ex)
        {
            throwsException = true;
        }

        Assert.IsFalse(throwsException);
    }
}



public class TestEvent(): Request();
public class TestEvent2(): Request<int>();
public class TestEvent3(): Notification;

public class TestHandler : IRequestHandler<TestEvent>, IRequestHandler<TestEvent2, int>, INotificationHandler<TestEvent3>
{

    public async Task HandleAsync(TestEvent request, CancellationToken cancellationToken = default)
    {
        await Task.Delay(500);
        Console.WriteLine($"TestHandler TestEvent");
    }

    public async Task<int> HandleAsync(TestEvent2 request, CancellationToken cancellationToken = default)
    {
        await Task.Delay(500);
        Console.WriteLine($"TestHandler TestEvent2");
        return 2;
    }

    public async Task Handle(TestEvent3 notification)
    {
        await Task.Delay(500);
        Console.WriteLine($"TestHandler TestEvent3 {DateTime.Now.Millisecond}");
    }
}

public class TestHandler2 : INotificationHandler<TestEvent3>
{
    public async Task Handle(TestEvent3 notification)
    {
        await Task.Delay(500);
        Console.WriteLine($"TestHandler2 TestEvent3 {DateTime.Now.Millisecond}");
    }
}
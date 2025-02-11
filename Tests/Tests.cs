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
    public async Task _2_RunTestCommand_ShouldWorkAsync()
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
            await mareator.RunAsync(new TestCommand());
        }
        catch (Exception)
        {
            throwsException = true;
        }

        Assert.IsFalse(throwsException);
    }
    
    [TestMethod]
    public async Task _3_RunTestRequest_ShouldWorkAsync()
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
            var result = await mareator.RequestAsync<TestRequest, TestRequestResult>(new TestRequest());
            Assert.AreEqual(3, result.Value);
        }
        catch (Exception)
        {
            throwsException = true;
        }

        Assert.IsFalse(throwsException);
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

        var result = 0;

        mareator.Subscribe<TestEvent>((sender, args) =>
        {
            result++;
        });
        mareator.Subscribe<TestEvent2>((sender, args) =>
        {
            result++;
        });

        var throwsException = false;
        try
        {
            mareator.Publish(this, new TestEvent());
            mareator.Publish(this, new TestEvent());
            mareator.Publish(this, new TestEvent2());
        }
        catch (Exception)
        {
            throwsException = true;
        }

        await Task.Delay(1000);

        Assert.IsFalse(throwsException);
        Assert.AreEqual(3, result);
    }
}


public class TestCommand() : ICommand;
public class TestCommand2() : ICommand;
public class TestRequest() : IRequest<TestRequestResult>;
public class TestRequestResult(int Value)
{
    public int Value { get; } = Value;
}

public class TestEvent() : EventArgs();
public class TestEvent2() : EventArgs();

public class TestHandler : ICommandHandler<TestCommand>, IRequestHandler<TestRequest, TestRequestResult>
{
    public bool Handled { get; private set; } = false;

    public async Task HandleAsync(TestCommand command, CancellationToken cancellationToken = default)
    {
        await Task.Delay(500);
        Console.WriteLine($"TestHandler TestCommand {DateTime.Now.Millisecond}");
    }

    public Task<TestRequestResult> HandleAsync(TestRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new TestRequestResult(3));
    }
}

public class TestHandler2 : ICommandHandler<TestCommand2>
{
    public bool Handled { get; private set; } = false;
    public async Task HandleAsync(TestCommand2 notification, CancellationToken cancellationToken = default)
    {
        await Task.Delay(500);
        Console.WriteLine($"TestHandler2 TestCommand2 {DateTime.Now.Millisecond}");
        Handled = true;
    }
}

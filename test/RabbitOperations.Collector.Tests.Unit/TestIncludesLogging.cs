using System;
using RabbitOperations.Collector.Tests.Unit;
using Xunit.Abstractions;

public class TestIncludesLogging : IDisposable
{
    private readonly IDisposable logCapture;
    protected ITestOutputHelper OutputHelper;

    public TestIncludesLogging(ITestOutputHelper outputHelper)
    {
        this.OutputHelper = outputHelper;
        logCapture = LoggingHelper.Capture(outputHelper);
    }


    public void Dispose()
    {
        Dispose(true);
    }

    public virtual void Dispose(bool isDisposing)
    {
        logCapture.Dispose();
    }
}
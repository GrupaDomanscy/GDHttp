using GDHttp;

namespace Testing;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CheckIfCancellationTokenWorks()
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        bool started = false;
        bool finished = false;

        Task serverProcess = Task.Run(() =>
        {
            HttpListener httpListener = new HttpListener(new[] { "http://localhost:3000/" });

            httpListener.ServerStarted += delegate { started = true; };

            httpListener.ServerClosed += delegate { finished = true; };

            httpListener.Start(_ => HttpResponse.EmptyNotFound(), cts.Token);
        });
        
        Thread.Sleep(100);

        if (started == false)
        {
            Assert.Fail("Server should have started by now, something is wrong.");
        }
        
        cts.Cancel();

        if (!serverProcess.Wait(TimeSpan.FromMilliseconds(1200)))
        {
            Assert.Fail("Server should have closed by now, something is wrong.");
        }

        if (finished == false)
        {
            Assert.Fail("Server did not fire close event when closing itself.");
        }
        
        Assert.Pass();
    }

    [Test]
    public void ServerCanStartWithoutEventListeners()
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        Task serverProcess = Task.Run(() =>
        {
            HttpListener httpListener = new HttpListener(new[] { "http://localhost:3001/" });

            httpListener.Start(_ => HttpResponse.EmptyNotFound(), cts.Token);
        });
        
        Thread.Sleep(100);

        cts.Cancel();

        if (!serverProcess.Wait(TimeSpan.FromMilliseconds(1200)))
        {
            Assert.Fail("Server should have closed by now, something is wrong.");
        }
        
        Assert.Pass();
    }
}
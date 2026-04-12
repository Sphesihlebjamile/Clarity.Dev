

var serviceProvider = new ServiceCollection()
    .AddClarityCli()
    .BuildServiceProvider();

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

var app = serviceProvider.GetRequiredService<IApplicationService>();
var exitCode = await app.RunAsync(args, cts.Token);
Environment.Exit(exitCode);
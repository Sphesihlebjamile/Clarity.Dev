

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
return await app.RunAsync(args, cts.Token);
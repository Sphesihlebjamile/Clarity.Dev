using Microsoft.Extensions.DependencyInjection;
using Clarity.Dev.CLI.Contracts;
using Clarity.Dev.CLI.Extensions;

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
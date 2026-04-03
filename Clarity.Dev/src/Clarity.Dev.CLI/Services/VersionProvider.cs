using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Clarity.Dev.CLI.Contracts;

namespace Clarity.Dev.CLI.Services;

public class VersionProvider : IVersionProvider
{
    private readonly IConfiguration _configuration;

    public VersionProvider()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public string GetCliVersion()
    {
        return _configuration.GetSection("AppSettings:Version").Value ?? "1.0.0";
    }
}

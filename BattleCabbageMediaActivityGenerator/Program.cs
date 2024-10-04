using BattleCabbageMediaActivityGenerator.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using BattleCabbageMediaActivityGenerator;
using Microsoft.EntityFrameworkCore.Diagnostics;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
string? conn_string = builder.Configuration.GetValue<string>("GEN_SQL_CONNECTION_STRING");

if (conn_string == null)
{
    Console.WriteLine("No connection string found.");
    return;
}

//Adding retry logic, because seriously...how could I miss that?
bool retry = builder.Configuration.GetValue<bool>("GEN_DISABLE_RETRY");
if (!retry)
{
    Console.WriteLine("Using retry logic.");
    builder.Services.AddDbContextFactory<BattleCabbageVideoContext>(options =>
    options
        .UseSqlServer(conn_string, options => options.EnableRetryOnFailure())
        );
}
else
{
    builder.Services.AddDbContextFactory<BattleCabbageVideoContext>(options =>
    options
        .UseSqlServer(conn_string));
}


builder.Services.AddHostedService<Generator>();

using IHost host = builder.Build();

await host.RunAsync();
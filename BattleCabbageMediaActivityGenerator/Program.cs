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
builder.Services.AddDbContext<BattleCabbageVideoContext>(options =>
    options
        .UseSqlServer(conn_string));

builder.Services.AddScoped<IGenerator, Generator>();

using IHost host = builder.Build();

// Ask the service provider for the configuration abstraction.
IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

RunGenerator(host.Services);

await host.RunAsync();

static void RunGenerator(IServiceProvider hostProvider)
{
    var config = hostProvider.GetService<IConfiguration>();
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;
    if (config != null && config.GetValue<bool>("GEN_PAST_DATA"))
    {
        DateTime startDate = config.GetValue<DateTime>("GEN_PAST_START_DATETIME");
        if(startDate == DateTime.MinValue)
        {
            Console.WriteLine("No start date found.");
            return;
        }

        DateTime endDate = config.GetValue<DateTime>("GEN_PAST_END_DATETIME");
        if (endDate == DateTime.MinValue)
        {
            endDate = DateTime.Now;
        }

        provider.GetRequiredService<IGenerator>().GeneratePastActivity(startDate, endDate).Wait();
    }
    else if (config != null)
    {
        provider.GetRequiredService<IGenerator>().GenerateCurrentActivity().Wait();
    }
    else
    {
        Console.WriteLine("No configuration found.");
        return;
    }
}
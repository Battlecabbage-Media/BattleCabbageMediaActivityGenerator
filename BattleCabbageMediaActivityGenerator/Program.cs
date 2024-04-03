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
    builder.Services.AddDbContext<BattleCabbageVideoContext>(options =>
    options
        .UseSqlServer(conn_string, options => options.EnableRetryOnFailure())
        );
}
else
{
    builder.Services.AddDbContext<BattleCabbageVideoContext>(options =>
    options
        .UseSqlServer(conn_string));
}


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

        bool resumeGeneration = config.GetValue<bool>("GEN_RESUME_GENERATION");
        if (resumeGeneration)
        {
            var _dbContext = provider.GetRequiredService<BattleCabbageVideoContext>();
            // Get the last transaction date from the database to resume generation from that point.
            startDate = _dbContext.Purchases.Where(p => p.TransactionCreatedOn >= startDate && p.TransactionCreatedOn <= endDate).OrderByDescending(p => p.TransactionCreatedOn).First().TransactionCreatedOn;
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
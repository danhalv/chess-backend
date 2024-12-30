using Microsoft.AspNetCore;

namespace ChessApi;

public class Program
{
  public static void Main(string[] args)
  {
    CreateWebHostBuilder(args).Build().Run();
  }

  public static IWebHostBuilder CreateWebHostBuilder(string[] args)
  {
    var config = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json")
                 .AddEnvironmentVariables()
                 .Build();

    return WebHost.CreateDefaultBuilder(args)
                  .UseConfiguration(config)
                  .UseStartup<Startup>();
  }
}

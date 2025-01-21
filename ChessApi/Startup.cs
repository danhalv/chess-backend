using ChessApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChessApi;

public class Startup
{
  public IConfiguration Configuration { get; }

  private readonly IWebHostEnvironment _env;

  public Startup(IConfiguration configuration,
                 IWebHostEnvironment env)
  {
    Configuration = configuration;
    _env = env;
  }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddControllers();

    if (_env.IsStaging())
    {
      var conn = Configuration.GetConnectionString("DefaultConnection");
      services.AddDbContext<ChessDbContext>(options =>
        options.UseNpgsql(conn));
    }
    else
    {
      services.AddDbContext<ChessDbContext>(options =>
        options.UseInMemoryDatabase("ChessDB"));
    }
  }

  public void Configure(IApplicationBuilder app)
  {
    var webSocketOptions = new WebSocketOptions
    {
      KeepAliveInterval = TimeSpan.FromMinutes(2)
    };
    app.UseWebSockets(webSocketOptions);

    app.UseRouting();

    app.UseCors(appBuilder => appBuilder.AllowAnyOrigin()
                                        .AllowAnyHeader()
                                        .AllowAnyMethod());

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
    });
  }
}

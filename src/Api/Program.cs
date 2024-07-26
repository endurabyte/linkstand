using System.Diagnostics;
using System.Runtime.InteropServices;
using Lamar.Microsoft.DependencyInjection;
using Api.Contracts;
using Api.Data;
using Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Api;

internal class Program
{
  private static async Task Main(string[] args)
  {
    string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

    string os = RuntimeInformation.OSDescription;
    os = os switch
    {
      _ when os.Contains("Windows", StringComparison.OrdinalIgnoreCase) => "Windows",
      _ when os.Contains("mac", StringComparison.OrdinalIgnoreCase) => "macOS",
      _ => "Linux",
    };

    IConfiguration configuration = new ConfigurationBuilder()
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json")
       .AddJsonFile($"appsettings.{env}.json", true)
       .AddJsonFile($"appsettings.{os}.json", true)
       .AddEnvironmentVariables()
       .Build();

    bool isProduction = !(Debugger.IsAttached || env == Environments.Development);
    string? dbConnectionString = configuration["LINKSTAND_DB_CONNECTION_STRING"];

    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddConfiguration(configuration);

    builder.Services.AddDbContext<AppDbContext>(options =>
    {
      options.UseNpgsql(dbConnectionString);
    });

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    if (builder.Environment.IsDevelopment())
    {
      builder.Services.AddCors(options =>
      {
        options.AddPolicy("DebugPolicy", pb =>
        {
          pb.AllowAnyOrigin();
          pb.AllowAnyMethod();
          pb.AllowAnyHeader();
        });
      });
    }
    else
    {
      // Allow localhost 8080 and linkstand.net
      builder.Services.AddCors(options =>
      {
        options.AddPolicy("ProductionPolicy", pb =>
        {
          pb.WithOrigins("http://localhost:8080", "https://linkstand.net", "https://www.linkstand.net");
          pb.WithMethods("GET", "POST");
        });
      });
    }

    builder.Host.UseLamar((_, registry) =>
    {
      registry.For<IAliasService>().Use<AliasService>();
      registry.For<IAliasRepo>().Use<AliasRepo>();
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();

      app.UseSwaggerUI(c =>
      {
        c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
      });

      app.UseFileServer();
      app.UseDeveloperExceptionPage();
      app.UseCors("DebugPolicy");
    }
    else
    {
      app.UseCors("ProductionPolicy");
    }

    var db = app.Services.GetService<AppDbContext>();
    if (db != null)
    {
      await db.InitAsync().OnAnyThread();
    }

    app.UseRouting();
    app.MapControllers();
    await app.RunAsync();
  }
}
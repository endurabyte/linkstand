using Lamar;
using Lamar.Microsoft.DependencyInjection;
using LinkStand.Controllers;

internal class Program
{
  private static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    if (builder.Environment.IsDevelopment())
    {
      builder.Services.AddCors(options =>
      {
        options.AddPolicy("DebugPolicy", builder =>
        {
          builder.AllowAnyOrigin();
          builder.AllowAnyMethod();
          builder.AllowAnyHeader();
        });
      });
    }
    else
    {
      // Allow localhost 8080 and linkstand.net
      builder.Services.AddCors(options =>
      {
        options.AddPolicy("ProductionPolicy", builder =>
        {
          builder.WithOrigins("http://localhost:8080", "https://linkstand.net");
          builder.WithMethods("GET", "POST");
        });
      });
    }

    builder.Host.UseLamar((context, registry) =>
    {
      registry.For<IUrlMapService>().Use<UrlMapService>().Singleton();
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();

      app.UseSwaggerUI(c =>
      {
        c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
      });

      app.UseDeveloperExceptionPage();
      app.UseCors("DebugPolicy");
    }
    else
    {
      app.UseCors("ProductionPolicy");
    }

    app.UseFileServer();
    app.UseRouting();
    app.MapControllers();
    app.Run();
  }
}
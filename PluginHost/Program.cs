using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using PluginHost.Features.Path;
using PluginHost.Features.Plugin;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole().SetMinimumLevel(LogLevel.Warning);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
  containerBuilder.RegisterType<PluginRunner>();
  containerBuilder.RegisterType<PathHelper>().As<IPathHelper>().SingleInstance();
});

var app = builder.Build();

app.MapGet("/{id:guid}",
  async ([FromRoute] Guid id, [FromServices] ILogger<Program> logger, [FromServices] PluginRunner.Factory factory) =>
  {
    await using var runner = factory(id);
    return runner.Transform();
  });

await app.RunAsync();
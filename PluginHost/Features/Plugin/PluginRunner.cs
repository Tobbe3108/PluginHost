using Abstractions;
using Autofac;
using McMaster.NETCore.Plugins;
using PluginHost.Features.Path;

namespace PluginHost.Features.Plugin;

public class PluginRunner : IAsyncDisposable
{
  public delegate PluginRunner Factory(Guid id);

  private readonly IPathHelper _pathHelper;
  private readonly PluginLoader? _pluginLoader;
  private ILifetimeScope? _container;

  public PluginRunner(Guid id, IPathHelper pathHelper)
  {
    _pathHelper = pathHelper;
    _pluginLoader = LoadPlugin(GetPathFromId(id));
    LoadPluginConfiguration();
  }

  public string Transform() => (_container ?? throw new InvalidOperationException()).Resolve<IPlugin>().Transform();

  private string GetPathFromId(Guid id)
  {
    if (_pathHelper.PluginPaths.TryGetValue(id, out var path) is false)
    {
      throw new InvalidOperationException($"Plugin with id {id} not found");
    }

    return path;
  }

  private static PluginLoader LoadPlugin(string pluginDllPath)
  {
    return PluginLoader.CreateFromAssemblyFile(pluginDllPath,
      sharedTypes: new[] { typeof(IPluginFactory), typeof(ContainerBuilder) },
      isUnloadable: true);
  }

  private void LoadPluginConfiguration()
  {
    var pluginFactoryType = _pluginLoader?.LoadDefaultAssembly()
      .GetTypes()
      .FirstOrDefault(t => typeof(IPluginFactory).IsAssignableFrom(t) && t.IsAbstract is false);

    var pluginType = _pluginLoader?.LoadDefaultAssembly()
      .GetTypes()
      .FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && t.IsAbstract is false);

    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType(pluginType ?? throw new InvalidOperationException()).As<IPlugin>();

    var plugin = Activator.CreateInstance(pluginFactoryType ?? throw new InvalidOperationException()) as IPluginFactory;
    plugin?.ConfigureContainer(containerBuilder);

    _container = containerBuilder.Build();
  }

  public async ValueTask DisposeAsync()
  {
    if (_container is not null) await _container.DisposeAsync();
    _pluginLoader?.Dispose();
  }
}
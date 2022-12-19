using Abstractions;
using Autofac;

namespace Plugin2;

public class PluginConfiguration : IPluginFactory
{
  public void ConfigureContainer(ContainerBuilder builder)
  {
    builder.RegisterType<SomeService>().As<ISomeService>();
  }
}
using Autofac;

namespace Abstractions;

public interface IPluginFactory
{
  void ConfigureContainer(ContainerBuilder builder);
}
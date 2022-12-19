using System.Collections.Concurrent;

namespace PluginHost.Features.Path;

public interface IPathHelper
{
  ConcurrentDictionary<Guid, string> PluginPaths { get; set; }
}
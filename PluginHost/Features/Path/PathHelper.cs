using System.Collections.Concurrent;
using System.Text.Json.Nodes;

namespace PluginHost.Features.Path;

public class PathHelper : IPathHelper
{
  public ConcurrentDictionary<Guid, string> PluginPaths { get; set; } = LoadPluginPaths();

  private static ConcurrentDictionary<Guid, string> LoadPluginPaths()
  {
    var pluginFolderPath = @"C:\Udv\Test\PluginHost\Plugins\bin\Debug"; //TODO Get path from config

    var tempDict = new ConcurrentDictionary<Guid, string>();
    if (Directory.Exists(pluginFolderPath) is false)
    {
      throw new FileNotFoundException("Main Plugin folder dose not found");
    }

    var subFolderPathStrings = Directory.GetDirectories(pluginFolderPath);
    foreach (var subFolderPathString in subFolderPathStrings)
    {
      var filePath = System.IO.Path.Combine(subFolderPathString, "PluginSettings.json");

      if (File.Exists(filePath) is false)
      {
        throw new FileNotFoundException($"Json File for  ( {subFolderPathString} ) not found.");
      }

      var document = JsonNode.Parse(File.ReadAllText(filePath))!;
      if (document is null)
      {
        throw new NullReferenceException("Deserialize json file fail to get ID for Service in folder: " +
                                         subFolderPathString);
      }

      var id = (Guid)(document["Id"] ?? throw new InvalidOperationException());

      var tsServicesDllPath = System.IO.Path.Combine(subFolderPathString,
        System.IO.Path.GetFileName(subFolderPathString) + ".dll");
      tempDict.TryAdd(id, tsServicesDllPath);
    }

    return tempDict;
  }
}
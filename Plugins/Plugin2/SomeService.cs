using Newtonsoft.Json;

namespace Plugin2;

public class SomeService : ISomeService
{
  public string GetString()
  {
    var newtonsoftName = typeof(JsonSerializer).Assembly.GetName();
    return $"{newtonsoftName.Name} {newtonsoftName.Version}";
  }
}
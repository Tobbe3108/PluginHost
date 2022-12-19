using Abstractions;

namespace Plugin1;

public class Plugin : IPlugin
{
  private readonly ISomeService _someService;

  public Plugin(ISomeService someService)
  {
    _someService = someService;
  }

  public string Transform() => _someService.GetString();
}
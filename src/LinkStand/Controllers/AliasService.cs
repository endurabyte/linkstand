namespace LinkStand.Controllers;

public interface IAliasService
{
  void Add(Alias alias);
  
  bool TryGet(string id, out Alias? alias);
}

public class AliasService : IAliasService
{
  private readonly Dictionary<string, Alias> _aliases = [];

  public void Add(Alias alias) => _aliases[alias.Id] = alias;

  public bool TryGet(string id, out Alias? alias) =>
    _aliases.TryGetValue(id, out alias);
}
using System.Collections.Concurrent;

namespace LinkStand.Controllers;

public interface IAliasService
{
  bool TryAdd(Alias alias);
  bool TryGetAlias(string id, out Alias? alias);
  
  void Add(AliasEvent e);
  bool TryGetEvents(string id, out List<AliasEvent>? events);
}

public class AliasService : IAliasService
{
  private readonly ConcurrentDictionary<string, Alias> _aliases = [];
  private readonly ConcurrentDictionary<string, List<AliasEvent>> _events = [];

  public bool TryAdd(Alias alias) => _aliases.TryAdd(alias.Id, alias) switch
  {
    true => true.AndDo(_ => _events.GetOrAdd(alias.Id, _ => [])),
    _ => false,
  };

  public bool TryGetAlias(string id, out Alias? alias) =>
    _aliases.TryGetValue(id, out alias);
  
  public void Add(AliasEvent e)
  {
    string id = e.AliasId;
    _events.GetOrAdd(id, _ => []);
    _events[id].Add(e);
  }
  
  public bool TryGetEvents(string id, out List<AliasEvent>? events) =>
    _events.TryGetValue(id, out events);
}
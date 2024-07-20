using System.Collections.Concurrent;

namespace LinkStand.Controllers;

public interface IAliasService
{
  bool TryAdd(Alias alias);
  bool TryGetAlias(AliasId id, out Alias? alias);
  
  void Add(AliasEvent e);
  bool TryGetEvents(AliasId id, out List<AliasEvent>? events);
}

public class AliasService : IAliasService
{
  private readonly ConcurrentDictionary<AliasId, Alias> _aliases = [];
  private readonly ConcurrentDictionary<AliasId, List<AliasEvent>> _events = [];

  public bool TryAdd(Alias alias) => _aliases.TryAdd(alias.Id, alias) switch
  {
    true => true.Chain(_ => _events.GetOrAdd(alias.Id, _ => [])),
    _ => false,
  };

  public bool TryGetAlias(AliasId id, out Alias? alias) =>
    _aliases.TryGetValue(id, out alias);
  
  public void Add(AliasEvent e)
  {
    AliasId id = e.AliasId;
    _events.GetOrAdd(id, _ => []);
    _events[id].Add(e);
  }
  
  public bool TryGetEvents(AliasId id, out List<AliasEvent>? events) =>
    _events.TryGetValue(id, out events);
}
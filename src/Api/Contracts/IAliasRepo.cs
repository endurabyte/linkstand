using Api.Model;

namespace Api.Contracts;

public interface IAliasRepo
{
  Task<DataAction> AddOrUpdateAsync(Alias alias);
  Task<Alias?> GetAsync(AliasId id);

  Task<DataAction> AddOrUpdateAsync(AliasEvent alias);
  Task<AliasEvent?> GetAsync(AliasEventId id);
  Task<List<AliasEvent>> GetAllAliasEventsAsync(AliasId id);
}


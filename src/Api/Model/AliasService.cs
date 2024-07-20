using Api.Contracts;

namespace Api.Model;

public interface IAliasService
{
  Task<DataAction> AddOrUpdateAsync(Alias alias);
  Task<Alias?> GetAsync(AliasId id);

  Task<DataAction> AddOrUpdateAsync(AliasEvent alias);
  Task<AliasEvent?> GetAsync(AliasEventId id);
  Task<List<AliasEvent>> GetAllAliasEventsAsync(AliasId id);
}

public class AliasService(IAliasRepo repo) : IAliasService
{
  public async Task<DataAction> AddOrUpdateAsync(Alias alias) => await repo.AddOrUpdateAsync(alias).OnAnyThread();
  public async Task<Alias?> GetAsync(AliasId id) => await repo.GetAsync(id).OnAnyThread();

  public async Task<DataAction> AddOrUpdateAsync(AliasEvent alias) => await repo.AddOrUpdateAsync(alias).OnAnyThread();
  public async Task<AliasEvent?> GetAsync(AliasEventId id) => await repo.GetAsync(id).OnAnyThread();

  public async Task<List<AliasEvent>> GetAllAliasEventsAsync(AliasId id) => await repo.GetAllAliasEventsAsync(id).OnAnyThread();
}
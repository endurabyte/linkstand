using Api.Contracts;
using Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class AliasRepo(AppDbContext db, ILogger<AliasRepo> log) : IAliasRepo
{
  public async Task<DataAction> AddOrUpdateAsync(Alias alias)
  {
    log.LogInformation($"{nameof(AddOrUpdateAsync)}({"{alias}"}", alias);

    Alias? existing = db.Alias
      .FirstOrDefault(a => a.Id == alias.Id);

    if (existing == null)
    {
      db.Alias.Add(alias);
    }
    else
    {
      db.Alias.Update(alias);
    }

    await db.SaveChangesAsync().OnAnyThread();

    return existing switch
    {
      null => DataAction.Added,
      _ => DataAction.Updated,
    };
  }

  public async Task<Alias?> GetAsync(AliasId id)
  {
    log.LogInformation($"{nameof(GetAsync)}({"{id}"}", id);

    return await db.Alias
      .FirstOrDefaultAsync(a => a.Id == id)
      .OnAnyThread();
  }

  public async Task<DataAction> AddOrUpdateAsync(AliasEvent e)
  {
    log.LogInformation($"{nameof(AddOrUpdateAsync)}({"{alias}"}", e);

    AliasEvent? existing = db.AliasEvent
      .FirstOrDefault(a => a.Id == e.Id);

    if (existing == null)
    {
      db.AliasEvent.Add(e);
    }
    else
    {
      db.AliasEvent.Update(e);
    }

    await db.SaveChangesAsync().OnAnyThread();

    return existing switch
    {
      null => DataAction.Added,
      _ => DataAction.Updated,
    };
  }

  public async Task<AliasEvent?> GetAsync(AliasEventId id)
  {
    log.LogInformation($"{nameof(GetAsync)}({"{id}"}", id);

    return await db.AliasEvent
      .FirstOrDefaultAsync(a => a.Id == id)
      .OnAnyThread();
  }

  public async Task<List<AliasEvent>> GetAllAliasEventsAsync(AliasId id) =>
    await db.AliasEvent.Where(a => a.AliasId == id).ToListAsync().OnAnyThread();
}

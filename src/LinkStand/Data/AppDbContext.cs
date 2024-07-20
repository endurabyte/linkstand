using LinkStand.Model;
using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<Alias> Alias { get; set; }
  public DbSet<AliasEvent> AliasEvent { get; set; }

  public async Task InitAsync()
  {
    await Database.EnsureCreatedAsync().OnAnyThread();
  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder.Entity<Alias>()
      .Property(e => e.Id)
      .HasVogenConversion();

    builder.Entity<AliasEvent>()
      .Property(e => e.Id)
      .HasVogenConversion();

    builder.Entity<Alias>()
      .HasMany(e => e.Events)
      .WithOne(e => e.Alias)
      .HasForeignKey(e => e.AliasId);
  }
}
namespace LinkStand.Model;

public record Alias(AliasId Id, string Target, AliasType Type, List<AliasEvent> Events)
{
  public static Alias None => new(AliasId.None, "", AliasType.None, []);
  public Alias() : this(AliasId.None, "", AliasType.None, []) { }
}
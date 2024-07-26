using System.Text.Json.Serialization;

namespace Api.Model;

public record AliasEvent(AliasEventId Id, AliasId AliasId, [property: JsonIgnore] Alias Alias, string Ip, DateTime Timestamp)
{
  public AliasEvent() : this(AliasEventId.None, AliasId.None, Alias.None, "", DateTime.MinValue) { }
}

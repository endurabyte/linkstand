using Vogen;

namespace LinkStand.Model;

[ValueObject<string>]
[Instance("None", "")]
public partial record AliasId { }

[EfCoreConverter<AliasId>]
[EfCoreConverter<AliasEventId>]
internal partial class VogenEfCoreConverters;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Adapter.SqlServer.Data.Converters;
public class ValueConverters
{
    public class UtcValueConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcValueConverter()
            : base(
                toDb => toDb.Kind == DateTimeKind.Utc ? toDb : DateTime.SpecifyKind(toDb, DateTimeKind.Utc),
                fromDb => DateTime.SpecifyKind(fromDb, DateTimeKind.Utc))
        { }
    }

    public class NullableUtcValueConverter : ValueConverter<DateTime?, DateTime?>
    {
        public NullableUtcValueConverter()
            : base(
                toDb => toDb.HasValue ? (toDb.Value.Kind == DateTimeKind.Utc ? toDb : DateTime.SpecifyKind(toDb.Value, DateTimeKind.Utc)) : null,
                fromDb => fromDb.HasValue ? DateTime.SpecifyKind(fromDb.Value, DateTimeKind.Utc) : null)
        { }
    }
}

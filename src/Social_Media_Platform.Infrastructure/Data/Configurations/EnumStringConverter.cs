using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

internal sealed class EnumStringConverter<TEnum> : ValueConverter<TEnum, string>
    where TEnum : struct, Enum
{
    public EnumStringConverter()
        : base(
            value => EnumStorageName.ToStorage(value),
            value => Enum.Parse<TEnum>(value.Replace("_", string.Empty), true))
    {
    }
}

internal sealed class NullableEnumStringConverter<TEnum> : ValueConverter<TEnum?, string?>
    where TEnum : struct, Enum
{
    public NullableEnumStringConverter()
        : base(
            value => value.HasValue ? EnumStorageName.ToStorage(value.Value) : null,
            value => value == null
                ? null
                : Enum.Parse<TEnum>(value.Replace("_", string.Empty), true))
    {
    }
}

internal static class EnumStorageName
{
    public static string ToStorage<TEnum>(TEnum value)
        where TEnum : struct, Enum =>
        string.Concat(value.ToString().Select((character, index) =>
            char.IsUpper(character) && index > 0
                ? $"_{char.ToLowerInvariant(character)}"
                : char.ToLowerInvariant(character).ToString()));
}

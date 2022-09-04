using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Expenses;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
            DateOnly.ParseExact(reader.GetString()!,
                "yyyy-MM-dd", CultureInfo.InvariantCulture);

    public override void Write(
        Utf8JsonWriter writer,
        DateOnly dateValue,
        JsonSerializerOptions options) =>
            writer.WriteStringValue(dateValue.ToString(
                "yyyy-MM-dd", CultureInfo.InvariantCulture));
}

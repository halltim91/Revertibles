using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;

namespace Dhazel.TestHelpers;

public static class TestOutputExtensions
{
    private static readonly JsonSerializerOptions PrettyJsonOptions = new()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };

    public static void Dump(this ITestOutputHelper output, object? value, string? label = null)
    {
        var json = value.ToPrettyJson();
        output.WriteLine(string.IsNullOrEmpty(label) ? json : $"{label}:{Environment.NewLine}{json}");
    }

    public static string ToPrettyJson(this object? value)
    {
        if (value is null)
            return "null";

        try
        {
            return JsonSerializer.Serialize(value, value.GetType(), PrettyJsonOptions);
        }
        catch (Exception ex)
        {
            return $"<failed to serialize {value.GetType().FullName}: {ex.Message}>";
        }
    }
}

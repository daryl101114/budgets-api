using System.Text.Json;
using System.Text.Json.Serialization;

namespace budget_api.Tests.Helpers
{
    public static class TestJsonOptions
    {
        // Matches the API's serializer config: string enums + ignore cycles
        public static readonly JsonSerializerOptions Default = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }
}

namespace DeepSeekMsAiDemo;

using System.Text.Json;

public record CategoriesResponse(List<Category> Categories);
public record Category(string Name, List<string> Brands);

public static class JsonSerializationOptions
{
    public readonly static JsonSerializerOptions LlmSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };
}

public record OrganizationResponse
{
    public List<OrganizationRecord> Organizations { get; set; } = new();

    public record OrganizationRecord
    {
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
    }
}
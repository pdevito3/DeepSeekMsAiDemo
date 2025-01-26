// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using DeepSeekMsAiDemo;
using Microsoft.Extensions.AI;

var deepseekChatClient = new OllamaChatClient(new Uri("http://localhost:11434"), "deepseek-r1");
var deepseekChatOptions = new ChatOptions
{
    ResponseFormat = ChatResponseFormat.Json,
    // response_format neded for deepseek json setup (doesn;t affect 3.2) -- https://api-docs.deepseek.com/guides/json_mode
    AdditionalProperties = new AdditionalPropertiesDictionary
    {
        ["response_format"] = new
        {
            type = "json_object"
        }
    },
};

var threeTwoChatClient = new OllamaChatClient(new Uri("http://localhost:11434"), "llama3.2");

var threeTwoChatOptions = new ChatOptions
{
    ResponseFormat = ChatResponseFormat.Json,
};

Console.WriteLine($"Processing LLM call...{Environment.NewLine}{Environment.NewLine}");

await ResponseManager.WorksAllTheTime(threeTwoChatClient, threeTwoChatOptions);
await ResponseManager.WorksThreeTwoNotDeepseek(threeTwoChatClient, threeTwoChatOptions);
await ResponseManager.WorksAllTheTime(deepseekChatClient, deepseekChatOptions);
await ResponseManager.WorksThreeTwoNotDeepseek(deepseekChatClient, deepseekChatOptions);


public static  class ResponseManager
{
    public static async Task WorksAllTheTime(IChatClient chatClient, ChatOptions chatOptions)
    {
        var prompt = """
                     Generate 5 product category names for an online retailer
                      of high-tech outdoor adventure goods and related clothing/electronics/etc.
                      Each category name is a single descriptive term, so it does not use the word 'and'.
                      Category names should be interesting and novel, e.g., "Mountain Unicycles", "AI Boots",
                      or "High-volume Water Filtration Plants", not simply "Tents".
                      This retailer sells relatively technical products.
                     
                      Each category has a list of up to 8 brand names that make products in that category. All brand names are
                      purely fictional. Brand names are usually multiple words with spaces and/or special characters, e.g.
                      "Orange Gear", "Aqua Tech US", "Livewell", "E & K", "JAXⓇ".
                      Many brand names are used in multiple categories. Some categories have only 2 brands.
                      
                      The response should be in a JSON format like below with the exact batch count of objects. It is very important you make sure it is in this format and that it is valid JSON. 
                      { "categories": [{"name":"Tents", "brands":["Rosewood", "Summit Kings"]}] }
                     """;

        var chatCompletion = await chatClient.CompleteAsync(prompt, chatOptions);
        var parsedJson = JsonSerializer.Deserialize<CategoriesResponse>(chatCompletion.Message.Text, JsonSerializationOptions.LlmSerializerOptions);

        // Console.WriteLine(JsonSerializer.Serialize(parsedJson, JsonSerializationOptions.LlmSerializerOptions));
        Console.WriteLine($"{chatClient.Metadata.ModelId} for categories has {parsedJson?.Categories?.Count ?? 0} items");
    }
    
    public static async Task WorksThreeTwoNotDeepseek(IChatClient chatClient, ChatOptions chatOptions)
    {
        var prompt =
            """
            Can you provide a list of 5 fake laboratory names? Here are a few examples (do not use any of these examples in your list): 

            - Redwood Genomics
            - Greater Peach Labs
            - GenoQuantum Diagnostics
            - Genesight Medical
            - Stonebridge Labs
            - Cardinal Diagnostics

            You should also make valid email domains for each organization. For example, Greater Peach Hospital might have a `greaterpeachhospital.com` or `gph.com` domain. Note that the domain does NOT have an `@` symbol.

            Make sure you return the response in valid json in the exact format below:

            {
                "organizations": [
                    { "name": "Redwood Genomics", "domain": "redgene.com" }
                ]
            }
            """;
        var chatCompletion = await chatClient.CompleteAsync(prompt, chatOptions);
        var parsedJson = JsonSerializer.Deserialize<OrganizationResponse>(chatCompletion.Message.Text, JsonSerializationOptions.LlmSerializerOptions);
        
        // Console.WriteLine(JsonSerializer.Serialize(parsedJson, JsonSerializationOptions.LlmSerializerOptions));
        Console.WriteLine($"{chatClient.Metadata.ModelId} for organizations has {parsedJson?.Organizations?.Count ?? 0} items");
        if (parsedJson?.Organizations?.Count == 0)
        {
            Console.WriteLine($"There should be 5 organizations in the response");
        }
    }
}

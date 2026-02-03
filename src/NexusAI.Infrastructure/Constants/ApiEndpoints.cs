namespace NexusAI.Infrastructure.Constants;

public static class ApiEndpoints
{
    public const string OllamaBase = "http://localhost:11434";
    public const string GeminiBase = "https://generativelanguage.googleapis.com/v1beta";
    
    public static string GeminiGenerateContent(string modelName) =>
        $"{GeminiBase}/models/{modelName}:generateContent";
}

namespace HelpYourself.Infrastructure.LLM;

public sealed class LlmOptions
{
    public const string Section = "Llm";

    public string BaseUrl { get; init; } = "http://localhost:1234";
    public string ChatEndpoint { get; init; } = "/v1/chat/completions";
    public string Model { get; init; } = "local-model";
}

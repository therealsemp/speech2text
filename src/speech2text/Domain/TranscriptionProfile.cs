namespace speech2text.Domain;

public class TranscriptionProfile
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public TranscriptionServiceType ServiceType { get; set; } = TranscriptionServiceType.AzureOpenAI;
    public string ApiKey { get; set; } = string.Empty;
    public string EndpointUrl { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
}

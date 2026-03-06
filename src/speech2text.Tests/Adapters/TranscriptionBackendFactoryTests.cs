using speech2text.Adapters.Transcription;
using speech2text.Domain;

namespace speech2text.Tests.Adapters;

public class TranscriptionBackendFactoryTests
{
    private readonly TranscriptionBackendFactory _factory = new();

    [Fact]
    public void Create_AzureOpenAI_ReturnsAzureOpenAIAdapter()
    {
        var profile = new TranscriptionProfile
        {
            ServiceType = TranscriptionServiceType.AzureOpenAI,
            EndpointUrl = "https://example.openai.azure.com/",
            ApiKey      = "test-key"
        };

        var backend = _factory.Create(profile);

        Assert.IsType<AzureOpenAITranscriptionAdapter>(backend);
    }

    [Fact]
    public void Create_UnknownServiceType_ThrowsNotSupportedException()
    {
        var profile = new TranscriptionProfile { ServiceType = (TranscriptionServiceType)999 };

        Assert.Throws<NotSupportedException>(() => _factory.Create(profile));
    }
}

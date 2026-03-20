using speech2text.Adapters.Transcription;
using speech2text.Domain;

namespace speech2text.Tests.Adapters;

public class TranscriptionBackendFactoryTests
{
    private readonly TranscriptionBackendFactory _factory = new();

    private static TranscriptionProfile ValidAzureProfile() => new()
    {
        ServiceType = TranscriptionServiceType.AzureOpenAI,
        EndpointUrl = "https://example.openai.azure.com/",
        ApiKey      = "test-key",
        ExtraParameters = new Dictionary<string, string> { ["deploymentName"] = "my-whisper" }
    };

    [Fact]
    public void Create_AzureOpenAI_ReturnsAzureOpenAIAdapter()
    {
        var backend = _factory.Create(ValidAzureProfile());

        Assert.IsType<AzureOpenAITranscriptionAdapter>(backend);
    }

    [Fact]
    public void Create_UnknownServiceType_ThrowsNotSupportedException()
    {
        var profile = new TranscriptionProfile { ServiceType = (TranscriptionServiceType)999 };

        Assert.Throws<NotSupportedException>(() => _factory.Create(profile));
    }

    [Fact]
    public void Create_AzureOpenAI_MissingEndpointUrl_ThrowsInvalidOperationException()
    {
        var profile = ValidAzureProfile();
        profile.EndpointUrl = string.Empty;

        Assert.Throws<InvalidOperationException>(() => _factory.Create(profile));
    }

    [Fact]
    public void Create_AzureOpenAI_MissingApiKey_ThrowsInvalidOperationException()
    {
        var profile = ValidAzureProfile();
        profile.ApiKey = string.Empty;

        Assert.Throws<InvalidOperationException>(() => _factory.Create(profile));
    }

    [Fact]
    public void Create_AzureOpenAI_MissingDeploymentName_ThrowsInvalidOperationException()
    {
        var profile = ValidAzureProfile();
        profile.ExtraParameters.Remove("deploymentName");

        Assert.Throws<InvalidOperationException>(() => _factory.Create(profile));
    }

    // --- GetParameterDefinitions ---

    [Fact]
    public void GetParameterDefinitions_AzureOpenAI_ReturnsDeploymentNameDefinition()
    {
        var definitions = _factory.GetParameterDefinitions(TranscriptionServiceType.AzureOpenAI);

        Assert.Single(definitions);
        Assert.Equal("deploymentName", definitions[0].Key);
        Assert.False(string.IsNullOrWhiteSpace(definitions[0].Label));
    }

    [Fact]
    public void GetParameterDefinitions_UnknownServiceType_ReturnsEmpty()
    {
        var definitions = _factory.GetParameterDefinitions((TranscriptionServiceType)999);

        Assert.Empty(definitions);
    }
}

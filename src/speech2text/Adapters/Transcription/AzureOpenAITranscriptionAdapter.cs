using speech2text.Domain;
using speech2text.Domain.Ports;

namespace speech2text.Adapters.Transcription;

public class AzureOpenAITranscriptionAdapter(TranscriptionProfile profile) : ITranscriptionBackend
{
    public Task<string> TranscribeAsync(byte[] audioData, string language, CancellationToken ct)
    {
        // Phase 4 : implémentation Azure OpenAI Whisper
        throw new NotImplementedException("Azure OpenAI adapter will be implemented in Phase 4.");
    }
}

namespace speech2text.Domain.Ports;

public interface ITranscriptionBackendFactory
{
    ITranscriptionBackend Create(TranscriptionProfile profile);
}

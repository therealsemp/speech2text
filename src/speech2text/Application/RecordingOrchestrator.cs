using speech2text.Domain;
using speech2text.Domain.Ports;

namespace speech2text.Application;

public class RecordingOrchestrator(
    IAudioCapture audioCapture,
    ITranscriptionBackendFactory backendFactory,
    ITextOutput textOutput,
    ISettingsRepository settingsRepository)
{
    private readonly RecordingSession _session = new();
    private CancellationTokenSource? _cts;
    private bool _cancelled;

    public RecordingState State => _session.State;

    /// <summary>Fired on the calling thread after each state transition.</summary>
    public event Action<RecordingState>? StateChanged;

    private void NotifyStateChanged() => StateChanged?.Invoke(_session.State);

    public async Task StartRecordingAsync()
    {
        _cts = new CancellationTokenSource();
        _cancelled = false;
        _session.StartRecording();
        NotifyStateChanged();

        byte[] audio;
        try
        {
            audio = await audioCapture.RecordAsync(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            _session.Cancel();
            NotifyStateChanged();
            return;
        }

        if (_cancelled)
        {
            _session.Cancel();
            NotifyStateChanged();
            return;
        }

        _session.StopRecording();
        NotifyStateChanged();

        var settings = settingsRepository.Load();
        var profile = settings.Profiles.First(p => p.Id == settings.ActiveProfileId);
        var backend = backendFactory.Create(profile);

        try
        {
            var text = await backend.TranscribeAsync(audio, profile.Language, CancellationToken.None);
            textOutput.InjectText(text);
            _session.CompleteTranscription(text);
        }
        catch (Exception)
        {
            // Phase 6 : gestion d'erreur complète
            _session.CompleteTranscription(string.Empty);
        }
        NotifyStateChanged();
    }

    public void StopRecording() => _cts?.Cancel();

    public void CancelRecording()
    {
        _cancelled = true;
        _cts?.Cancel();
    }
}

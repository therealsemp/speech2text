using Moq;
using speech2text.Application;
using speech2text.Domain;
using speech2text.Domain.Ports;

namespace speech2text.Tests.Application;

public class RecordingOrchestratorTests
{
    private readonly Mock<IAudioCapture> _audioCapture = new();
    private readonly Mock<ITranscriptionBackend> _backend = new();
    private readonly Mock<ITranscriptionBackendFactory> _backendFactory = new();
    private readonly Mock<ITextOutput> _textOutput = new();
    private readonly Mock<ISettingsRepository> _settingsRepository = new();
    private readonly RecordingOrchestrator _orchestrator;

    private static readonly byte[] SampleAudio = [1, 2, 3];

    public RecordingOrchestratorTests()
    {
        var profile = new TranscriptionProfile { Id = Guid.NewGuid(), Language = "en-US" };
        var settings = new AppSettings { ActiveProfileId = profile.Id, Profiles = [profile] };

        _settingsRepository.Setup(x => x.Load()).Returns(settings);
        _backendFactory.Setup(x => x.Create(It.IsAny<TranscriptionProfile>())).Returns(_backend.Object);
        _backend.Setup(x => x.TranscribeAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("transcribed text");

        _orchestrator = new RecordingOrchestrator(
            _audioCapture.Object,
            _backendFactory.Object,
            _textOutput.Object,
            _settingsRepository.Object);
    }

    // --- Flux normal ---

    [Fact]
    public async Task NormalFlow_TranscribesAndInjectsText()
    {
        var audioTcs = new TaskCompletionSource<byte[]>();
        _audioCapture.Setup(x => x.RecordAsync(It.IsAny<CancellationToken>())).Returns(audioTcs.Task);

        var sessionTask = _orchestrator.StartRecordingAsync();
        audioTcs.SetResult(SampleAudio);
        await sessionTask;

        _textOutput.Verify(x => x.InjectText("transcribed text"), Times.Once);
        Assert.Equal(RecordingState.Idle, _orchestrator.State);
    }

    [Fact]
    public async Task NormalFlow_StateIsRecordingWhileAudioCaptures()
    {
        var audioTcs = new TaskCompletionSource<byte[]>();
        _audioCapture.Setup(x => x.RecordAsync(It.IsAny<CancellationToken>())).Returns(audioTcs.Task);

        var sessionTask = _orchestrator.StartRecordingAsync();

        Assert.Equal(RecordingState.Recording, _orchestrator.State);

        audioTcs.SetResult(SampleAudio);
        await sessionTask;
    }

    [Fact]
    public async Task NormalFlow_PassesAudioAndLanguageToBackend()
    {
        var audioTcs = new TaskCompletionSource<byte[]>();
        _audioCapture.Setup(x => x.RecordAsync(It.IsAny<CancellationToken>())).Returns(audioTcs.Task);

        var sessionTask = _orchestrator.StartRecordingAsync();
        audioTcs.SetResult(SampleAudio);
        await sessionTask;

        _backend.Verify(x => x.TranscribeAsync(SampleAudio, "en-US", It.IsAny<CancellationToken>()), Times.Once);
    }

    // --- Flux cancel (Escape) ---

    [Fact]
    public async Task CancelFlow_DoesNotTranscribeOrInjectText()
    {
        _audioCapture.Setup(x => x.RecordAsync(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(async ct =>
            {
                await Task.Delay(Timeout.Infinite, ct);
                return [];
            });

        var sessionTask = _orchestrator.StartRecordingAsync();
        _orchestrator.CancelRecording();
        await sessionTask;

        _backend.Verify(x => x.TranscribeAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _textOutput.Verify(x => x.InjectText(It.IsAny<string>()), Times.Never);
        Assert.Equal(RecordingState.Idle, _orchestrator.State);
    }

    // --- Erreur de transcription ---

    [Fact]
    public async Task TranscriptionError_DoesNotThrow_ReturnsToIdle()
    {
        var audioTcs = new TaskCompletionSource<byte[]>();
        _audioCapture.Setup(x => x.RecordAsync(It.IsAny<CancellationToken>())).Returns(audioTcs.Task);
        _backend.Setup(x => x.TranscribeAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Network error"));

        var sessionTask = _orchestrator.StartRecordingAsync();
        audioTcs.SetResult(SampleAudio);
        await sessionTask; // ne doit pas throw

        Assert.Equal(RecordingState.Idle, _orchestrator.State);
    }

    [Fact]
    public async Task TranscriptionError_DoesNotInjectText()
    {
        var audioTcs = new TaskCompletionSource<byte[]>();
        _audioCapture.Setup(x => x.RecordAsync(It.IsAny<CancellationToken>())).Returns(audioTcs.Task);
        _backend.Setup(x => x.TranscribeAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Network error"));

        var sessionTask = _orchestrator.StartRecordingAsync();
        audioTcs.SetResult(SampleAudio);
        await sessionTask;

        _textOutput.Verify(x => x.InjectText(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task TranscriptionError_FiresErrorOccurred()
    {
        var audioTcs = new TaskCompletionSource<byte[]>();
        _audioCapture.Setup(x => x.RecordAsync(It.IsAny<CancellationToken>())).Returns(audioTcs.Task);
        _backend.Setup(x => x.TranscribeAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Transcription failed: network error."));

        string? capturedError = null;
        _orchestrator.ErrorOccurred += msg => capturedError = msg;

        var sessionTask = _orchestrator.StartRecordingAsync();
        audioTcs.SetResult(SampleAudio);
        await sessionTask;

        Assert.Equal("Transcription failed: network error.", capturedError);
    }

    // --- Erreur de capture audio ---

    [Fact]
    public async Task AudioCaptureError_DoesNotThrow_ReturnsToIdle()
    {
        _audioCapture.Setup(x => x.RecordAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Audio device unavailable."));

        await _orchestrator.StartRecordingAsync(); // ne doit pas throw

        Assert.Equal(RecordingState.Idle, _orchestrator.State);
    }

    [Fact]
    public async Task AudioCaptureError_FiresErrorOccurred_AndDoesNotTranscribe()
    {
        _audioCapture.Setup(x => x.RecordAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Audio device unavailable."));

        string? capturedError = null;
        _orchestrator.ErrorOccurred += msg => capturedError = msg;

        await _orchestrator.StartRecordingAsync();

        Assert.Equal("Audio device unavailable.", capturedError);
        _backend.Verify(x => x.TranscribeAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

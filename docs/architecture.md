# Technical Architecture

## Stack
- **Language**: C# 13 / .NET 9
- **UI framework**: WPF (Windows Presentation Foundation) — Windows-only, native
- **Deployment**: self-contained single `.exe` (no .NET runtime required on target machine)
- **Architecture pattern**: Hexagonal (Ports & Adapters) + MVVM for the UI layer
- **DDD**: ubiquitous language, `RecordingSession` aggregate, domain events (tactical DDD kept minimal)

## Overall structure

```
        [WPF / MVVM]                 ← primary adapter (drives the app)
              │
        [Domain core]                ← no dependency on anything external
              │
   ┌──────────┼──────────────┐
[NAudio]  [Azure OAI]  [JSON file]   ← secondary adapters (driven by the app)
```

The domain defines **ports** (interfaces). Adapters implement them.
WPF/MVVM is just another adapter — the domain has zero knowledge of the UI.

---

## Domain core

### Aggregate: RecordingSession
Central concept of the domain. Manages the lifecycle of a dictation session.

**States**: `Idle` → `Recording` → `Transcribing` → `Idle` (or `Cancelled` from Recording)

**Domain events**:
| Event | Triggered when |
|---|---|
| `RecordingStarted` | User activates the hotkey |
| `RecordingStopped` | User presses hotkey again |
| `RecordingCancelled` | User presses Escape |
| `TranscriptionCompleted` | Text is ready to inject |

### Models
- `TranscriptionProfile` — named config: service type, API key, endpoint URL, language
- `AppSettings` — active profile, selected audio device, hotkey binding
- `AudioDevice` — value object representing a microphone input

---

## Ports (defined by the domain)

```csharp
public interface ITranscriptionBackend
{
    Task<string> TranscribeAsync(byte[] audioData, string language, CancellationToken ct);
}

public interface ITranscriptionBackendFactory
{
    ITranscriptionBackend Create(TranscriptionProfile profile);
}

public interface IAudioCapture
{
    Task<byte[]> RecordAsync(CancellationToken ct);
}

public interface ITextOutput
{
    void InjectText(string text);
}

public interface ISettingsRepository
{
    AppSettings Load();
    void Save(AppSettings settings);
}

public interface IHotkeyRegistration
{
    void Register(string hotkey, Action onTriggered);
    void Unregister(string hotkey);
}
```

## Transcription backend factory

`TranscriptionProfile` carries a `TranscriptionServiceType` enum value.
The factory reads it and instantiates the correct adapter.
Adding a new backend = add an enum value + a switch case + an adapter class. Nothing else changes.

```csharp
public enum TranscriptionServiceType
{
    AzureOpenAI
    // future: GoogleSpeech, LocalWhisper, OpenAI, ...
}

public class TranscriptionBackendFactory : ITranscriptionBackendFactory
{
    public ITranscriptionBackend Create(TranscriptionProfile profile)
    {
        return profile.ServiceType switch
        {
            TranscriptionServiceType.AzureOpenAI => new AzureOpenAITranscriptionAdapter(profile),
            _ => throw new NotSupportedException($"Unsupported service type: {profile.ServiceType}")
        };
    }
}
```

---

## Adapters

### Primary (UI)
- **WPF / MVVM** — drives the domain through use cases
  - `OverlayViewModel` — exposes recording state, active profile, active device
  - `SettingsViewModel` — exposes and edits AppSettings
  - `OverlayWindow.xaml` / `SettingsWindow.xaml` — pure XAML, no logic

### Secondary (infrastructure)
| Adapter | Port | Library |
|---|---|---|
| `NAudioCaptureAdapter` | `IAudioCapture` | NAudio |
| `AzureOpenAITranscriptionAdapter` | `ITranscriptionBackend` | Azure.AI.OpenAI |
| `SendInputTextAdapter` | `ITextOutput` | InputSimulatorStandard |
| `JsonSettingsRepository` | `ISettingsRepository` | System.Text.Json |
| `NHotkeyAdapter` | `IHotkeyRegistration` | NHotkey.Wpf |

---

## Project structure

```
text2speech/
  Domain/
    RecordingSession.cs           # Aggregate — session lifecycle + domain events
    TranscriptionProfile.cs       # Entity — named service configuration
    AppSettings.cs                # Entity — global app settings
    AudioDevice.cs                # Value object
    Events/
      RecordingStarted.cs
      RecordingStopped.cs
      RecordingCancelled.cs
      TranscriptionCompleted.cs
    Ports/
      ITranscriptionBackend.cs
      ITranscriptionBackendFactory.cs
      IAudioCapture.cs
      ITextOutput.cs
      ISettingsRepository.cs
      IHotkeyRegistration.cs
  Application/
    RecordingOrchestrator.cs      # Use case: coordinates session, audio, transcription, injection
  Adapters/
    Audio/
      NAudioCaptureAdapter.cs
    Transcription/
      TranscriptionBackendFactory.cs
      AzureOpenAITranscriptionAdapter.cs
    TextOutput/
      SendInputTextAdapter.cs
    Settings/
      JsonSettingsRepository.cs
    Hotkey/
      NHotkeyAdapter.cs
  UI/
    ViewModels/
      OverlayViewModel.cs
      SettingsViewModel.cs
    Views/
      OverlayWindow.xaml
      SettingsWindow.xaml
  App.xaml                        # Entry point, DI container setup
```

---

## Testing strategy

- **Framework**: xUnit
- **Mocking**: Moq (for injecting fake port implementations)
- **Separate test project**: `text2speech.Tests/`

### What is tested

| Layer | Approach |
|---|---|
| Domain (`RecordingSession`, models) | Pure unit tests — no mocks needed, zero external dependencies |
| Application (`RecordingOrchestrator`) | Unit tests with mocked ports (`IAudioCapture`, `ITranscriptionBackend`, etc.) |
| Factory (`TranscriptionBackendFactory`) | Unit tests — verify correct adapter is instantiated per service type |
| Adapters (NAudio, Azure, etc.) | Not unit tested — covered by integration/manual tests |

### Test project structure

```
text2speech.Tests/
  Domain/
    RecordingSessionTests.cs
    TranscriptionProfileTests.cs
  Application/
    RecordingOrchestratorTests.cs
  Adapters/
    TranscriptionBackendFactoryTests.cs
```

---

## Configuration storage
- JSON file at `%AppData%\text2speech\settings.json`
- Contains: list of profiles, active profile ID, selected audio device, hotkey binding

## CI/CD — GitHub Actions

Public repository = unlimited free build minutes.

### Branching strategy (Gitflow)
```
main          ← numbered releases (v1.0.0, v1.1.0...)
  └── develop ← continuous integration
        └── feature/xxx ← one branch per feature
```

### Workflows

**`ci.yml`** — triggered on push to `develop` and `main`, and on PRs to `develop`:
```
dotnet restore → dotnet build → dotnet test
```

**`release.yml`** — triggered on tag push matching `v*` (e.g. `v1.0.0`):
```
dotnet publish (self-contained x64 exe) → GitHub Release with exe artifact
```

Workflow files to be created when bootstrapping the project.

---

## Technical constraints
- Windows only (WPF, NAudio, SendInput)
- .NET 9 required at build time (VS Code + C# Dev Kit extension)
- Target: self-contained x64 executable

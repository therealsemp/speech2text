# Roadmap

## Phase 1 — Bootstrap
> Project skeleton, everything builds and CI runs

- [ ] Create WPF solution + test project (`speech2text` / `speech2text.Tests`)
- [ ] Set up folder structure (Domain, Application, Adapters, UI)
- [ ] Add NuGet packages (NAudio, Azure.AI.OpenAI, NHotkey.Wpf, InputSimulatorStandard, Moq, xUnit)
- [ ] GitHub Actions `ci.yml` (push to develop/main → restore + build + test)
- [ ] Empty app launches without crashing

## Phase 2 — Domain core
> Pure C#, no external dependencies, fully unit tested

- [ ] `AudioDevice` value object
- [ ] `TranscriptionProfile` entity + `TranscriptionServiceType` enum
- [ ] `AppSettings` entity
- [ ] All port interfaces (`IAudioCapture`, `ITranscriptionBackend`, `ITranscriptionBackendFactory`, `ITextOutput`, `ISettingsRepository`, `IHotkeyRegistration`)
- [ ] `RecordingSession` aggregate — state machine (Idle → Recording → Transcribing → Idle / Cancelled)
- [ ] Domain events (`RecordingStarted`, `RecordingStopped`, `RecordingCancelled`, `TranscriptionCompleted`)
- [ ] Unit tests — `RecordingSessionTests` (all state transitions and events)

## Phase 3 — Application layer
> Orchestration logic, tested with mocked ports

- [ ] `TranscriptionBackendFactory` (enum-based, switch pattern)
- [ ] `RecordingOrchestrator` — coordinates session, audio capture, transcription, text injection
- [ ] Unit tests — `RecordingOrchestratorTests` (all scenarios: normal flow, cancel, transcription error)
- [ ] Unit tests — `TranscriptionBackendFactoryTests`

## Phase 4 — Infrastructure adapters
> Real implementations, manually testable

- [ ] `NAudioCaptureAdapter` — microphone recording, device selection
- [ ] `AzureOpenAITranscriptionAdapter` — Whisper API call
- [ ] `SendInputTextAdapter` — text injection at cursor
- [ ] `JsonSettingsRepository` — load/save `%AppData%\speech2text\settings.json`
- [ ] `NHotkeyAdapter` — global hotkey registration (`Ctrl+Shift+R` default)

## Phase 5 — UI
> Overlay + settings window, wired to the domain

- [ ] `OverlayWindow` — minimal floating window, recording indicator
- [ ] `OverlayViewModel` — recording state, active profile selector, audio device selector
- [ ] `SettingsWindow` — full settings panel
- [ ] `SettingsViewModel` — manage profiles (create, edit, delete), hotkey config
- [ ] DI container setup in `App.xaml.cs` — wire all ports to adapters
- [ ] Escape key cancels recording

## Phase 6 — Polish & error handling
> Stable and usable

- [ ] Error handling (transcription failure, no microphone, network error)
  - `NAudioCaptureAdapter`: catch `MmException` on `StartRecording()` — thrown when the device is in exclusive mode (e.g. locked by another app). Convert to a domain-level error and expose it via `RecordingOrchestrator` so the UI can display a meaningful message. Note: shared mode (default) allows concurrent access, so this only affects exclusive-mode drivers.
  - `NHotkeyAdapter`: catch `HotkeyAlreadyRegisteredException` — thrown when the requested hotkey is already registered by another application. Surface to the user with a suggestion to change the binding in settings.
  - `AzureOpenAITranscriptionAdapter`: handle network errors (`HttpRequestException`) and auth failures (401/403) with distinct user-facing messages.
- [ ] Default profile created on first launch (prompts for Azure credentials)

## Phase 7 — First release
> Published

- [ ] GitHub Actions `release.yml` (tag `v*` → self-contained x64 exe → GitHub Release artifact)
- [ ] First release: `v0.1.0`

## Backlog (post v0.1.0)
- Additional transcription backends (OpenAI direct, local Whisper, Google Speech...)
- Auto-start with Windows
- Transcription history

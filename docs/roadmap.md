# Roadmap

## Phase 1 — Bootstrap ✅
> Project skeleton, everything builds and CI runs

- [x] Create WPF solution + test project (`speech2text` / `speech2text.Tests`)
- [x] Set up folder structure (Domain, Application, Adapters, UI)
- [x] Add NuGet packages (NAudio, Azure.AI.OpenAI, NHotkey.Wpf, InputSimulatorStandard, Moq, xUnit)
- [x] GitHub Actions `ci.yml` (push to develop/main → restore + build + test)
- [x] Empty app launches without crashing

## Phase 2 — Domain core ✅
> Pure C#, no external dependencies, fully unit tested

- [x] `AudioDevice` value object
- [x] `TranscriptionProfile` entity + `TranscriptionServiceType` enum
- [x] `AppSettings` entity
- [x] All port interfaces (`IAudioCapture`, `ITranscriptionBackend`, `ITranscriptionBackendFactory`, `ITextOutput`, `ISettingsRepository`, `IHotkeyRegistration`)
- [x] `RecordingSession` aggregate — state machine (Idle → Recording → Transcribing → Idle / Cancelled)
- [x] Domain events (`RecordingStartedEvent`, `RecordingStoppedEvent`, `RecordingCancelledEvent`, `TranscriptionCompletedEvent`)
- [x] Unit tests — `RecordingSessionTests` (all state transitions and events)

## Phase 3 — Application layer ✅
> Orchestration logic, tested with mocked ports

- [x] `TranscriptionBackendFactory` (enum-based, switch pattern)
- [x] `RecordingOrchestrator` — coordinates session, audio capture, transcription, text injection
- [x] Unit tests — `RecordingOrchestratorTests` (all scenarios: normal flow, cancel, transcription error)
- [x] Unit tests — `TranscriptionBackendFactoryTests`

## Phase 4 — Infrastructure adapters ✅
> Real implementations, manually testable

- [x] `NAudioCaptureAdapter` — microphone recording, device selection
- [x] `AzureOpenAITranscriptionAdapter` — Whisper API call
- [x] `SendInputTextAdapter` — text injection at cursor
- [x] `JsonSettingsRepository` — load/save `%AppData%\speech2text\settings.json`
- [x] `NHotkeyAdapter` — global hotkey registration (`Ctrl+Shift+R` default)

## Phase 5 — UI ✅
> Overlay + settings window, wired to the domain

- [x] `OverlayWindow` — minimal floating window, recording indicator
- [x] `OverlayViewModel` — recording state, active profile selector, audio device selector
- [x] `SettingsWindow` — full settings panel
- [x] `SettingsViewModel` — manage profiles (create, edit, delete), hotkey config
- [x] DI container setup in `App.xaml.cs` — wire all ports to adapters
- [x] Escape key cancels recording (when overlay has focus)

## Phase 6 — Polish & error handling ✅
> Stable and usable

- [x] Error handling (transcription failure, no microphone, network error)
  - `NAudioCaptureAdapter`: catch `MmException` on `StartRecording()` — thrown when the device is in exclusive mode (e.g. locked by another app). Convert to a domain-level error and expose it via `RecordingOrchestrator` so the UI can display a meaningful message. Note: shared mode (default) allows concurrent access, so this only affects exclusive-mode drivers.
  - `NHotkeyAdapter`: catch `HotkeyAlreadyRegisteredException` — thrown when the requested hotkey is already registered by another application. Surface to the user with a suggestion to change the binding in settings.
  - `AzureOpenAITranscriptionAdapter`: handle network errors (`HttpRequestException`) and auth failures (401/403) with distinct user-facing messages.
- [x] `AzureOpenAITranscriptionAdapter` disposal — implements `IDisposable`; `TranscriptionBackendFactory` tracks and disposes the previous instance before creating a new one.

## Phase 7 — UI polish
> Visually finished

- [ ] Define a consistent visual style (colors, typography, spacing)
- [ ] `OverlayWindow` — refined layout, animated recording indicator
- [ ] `SettingsWindow` — improved form layout and visual hierarchy
- [ ] Overall coherence (fonts, button styles, dark/light mode ?)

## Phase 8 — First release
> Published

- [ ] GitHub Actions `release.yml` (tag `v*` → self-contained x64 exe → GitHub Release artifact)
- [ ] First release: `v0.1.0`

## Backlog (post v0.1.0)
- Default profile created on first launch (prompts for Azure credentials)
- Additional transcription backends (OpenAI direct, local Whisper, Google Speech...)
- Auto-start with Windows
- Transcription history

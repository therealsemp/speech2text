# Functional Specifications

## Vision
A lightweight Windows application that captures voice from the microphone and transcribes it to text using a cloud-based AI service. It is designed as a superior replacement for the native Windows dictation (Win+H), offering better transcription quality with minimal friction.

## Target users
Windows users who dictate text regularly and are dissatisfied with the native Windows speech recognition quality.

## Core behavior
- Activated and deactivated via a global keyboard shortcut (system-wide, works in any application)
- On activation: starts recording audio from the default microphone
- On deactivation: stops recording, sends audio to the transcription service, injects the result at the current cursor position
- The transcribed text is typed at the cursor position as if entered from the keyboard

## User interface

### Overlay
- A minimal floating overlay appears when recording starts
- Displays current recording state (e.g. recording indicator)
- Shows the active transcription configuration and audio input device, both switchable directly from the overlay (change takes effect on next recording)
- When the user presses the shortcut again to stop recording: transcription runs, text is injected, but **the overlay stays open**
- The overlay has a manual close button (X)
- No auto-close after transcription

### Settings panel
- Full configuration interface, accessible separately (e.g. from overlay or system tray)
- Audio input device selection
- Keyboard shortcut configuration
- Management of transcription service configurations (see below)

## Transcription service configurations
- The app supports multiple named configurations (profiles), selectable from the overlay or the settings panel
- Each configuration has:
  - A display name
  - Service type (v1: Azure OpenAI / Whisper only — architecture must remain extensible)
  - API key
  - Azure endpoint URL
  - Transcription language
- One configuration is marked as active at any time
- Typical use case: one configuration per language, even if they point to the same backend service

## Keyboard shortcut
- Global hotkey, configurable by the user
- Default: `Ctrl+Shift+R` (R for Record)
- Toggle behavior: press once to start recording, press again to stop and transcribe
- Pressing `Escape` while recording cancels the capture — no transcription is sent, no text is injected, overlay stays open

## Out of scope (v1)
- Local/offline transcription
- Multiple microphone selection
- Transcription history
- Support for operating systems other than Windows

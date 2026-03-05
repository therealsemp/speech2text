# text2speech

A lightweight Windows dictation app that replaces the native Win+H shortcut with AI-powered transcription.

## Motivation

Windows has a built-in dictation feature triggered by `Win+H`. While the concept is great — press a shortcut, speak, get text injected wherever your cursor is — the transcription quality is poor. In 2026, with the maturity of AI-based speech recognition models, there is no excuse for mediocre results. This app keeps the same frictionless experience while leveraging modern transcription services (Azure OpenAI / Whisper) for dramatically better accuracy.

## What it does

- Global keyboard shortcut (`Ctrl+Shift+R` by default) to start/stop recording
- Transcribed text is injected at the cursor position, in any application
- Minimal floating overlay with quick access to configuration
- Supports multiple named transcription profiles (e.g. one per language)
- Configurable audio input device
- Press `Escape` to cancel a recording without transcribing

## Status

Work in progress — specifications phase.

using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using speech2text.Adapters.Audio;
using speech2text.Adapters.Hotkey;
using speech2text.Adapters.Settings;
using speech2text.Adapters.TextOutput;
using speech2text.Adapters.Transcription;
using speech2text.Application;
using speech2text.Domain;
using speech2text.Domain.Ports;
using speech2text.UI.ViewModels;
using speech2text.UI.Views;

namespace speech2text;

public partial class App : System.Windows.Application
{
    private ServiceProvider? _services;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _services = BuildServices();

        var overlay    = _services.GetRequiredService<OverlayWindow>();
        var settings   = _services.GetRequiredService<SettingsWindow>();
        var orchestrator = _services.GetRequiredService<RecordingOrchestrator>();
        var hotkey     = _services.GetRequiredService<IHotkeyRegistration>();
        var settingsVm = _services.GetRequiredService<SettingsViewModel>();
        var overlayVm  = _services.GetRequiredService<OverlayViewModel>();

        // Open settings window on request from overlay
        overlayVm.OpenSettingsRequested += () => settings.Show();

        // Re-register hotkey when settings are saved
        settingsVm.SettingsSaved += newSettings =>
        {
            hotkey.Unregister(newSettings.HotkeyBinding);
            RegisterToggleHotkey(hotkey, orchestrator, newSettings.HotkeyBinding);
        };

        // Register the global toggle hotkey
        var appSettings = _services.GetRequiredService<ISettingsRepository>().Load();
        RegisterToggleHotkey(hotkey, orchestrator, appSettings.HotkeyBinding);

        MainWindow = overlay;
        overlay.Show();
    }

    private static void RegisterToggleHotkey(
        IHotkeyRegistration hotkey,
        RecordingOrchestrator orchestrator,
        string binding)
    {
        hotkey.Register(binding, () =>
        {
            if (orchestrator.State == RecordingState.Idle)
                _ = orchestrator.StartRecordingAsync();
            else if (orchestrator.State == RecordingState.Recording)
                orchestrator.StopRecording();
        });
    }

    private static ServiceProvider BuildServices()
    {
        var services = new ServiceCollection();

        // Infrastructure — adapters
        services.AddSingleton<IAudioCapture,          NAudioCaptureAdapter>();
        services.AddSingleton<IAudioDeviceEnumerator, NAudioDeviceEnumerator>();
        services.AddSingleton<ITranscriptionBackendFactory, TranscriptionBackendFactory>();
        services.AddSingleton<ITextOutput,            SendInputTextAdapter>();
        services.AddSingleton<ISettingsRepository,    JsonSettingsRepository>();
        services.AddSingleton<IHotkeyRegistration,    NHotkeyAdapter>();

        // Application
        services.AddSingleton<RecordingOrchestrator>();

        // UI
        services.AddSingleton<OverlayViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<OverlayWindow>();
        services.AddSingleton<SettingsWindow>();

        return services.BuildServiceProvider();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _services?.Dispose();
        base.OnExit(e);
    }
}

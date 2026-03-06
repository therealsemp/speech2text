using System.Collections.ObjectModel;
using System.Windows;
using speech2text.Application;
using speech2text.Domain;
using speech2text.Domain.Ports;

namespace speech2text.UI.ViewModels;

public class OverlayViewModel : ViewModelBase
{
    private readonly RecordingOrchestrator _orchestrator;
    private readonly ISettingsRepository _settingsRepository;

    private RecordingState _state;
    private TranscriptionProfile? _activeProfile;
    private AudioDevice? _selectedDevice;
    private string _errorMessage = string.Empty;

    public ObservableCollection<TranscriptionProfile> Profiles { get; } = [];
    public ObservableCollection<AudioDevice> AudioDevices { get; } = [];

    public RecordingState State
    {
        get => _state;
        private set
        {
            if (SetField(ref _state, value))
            {
                OnPropertyChanged(nameof(IsRecording));
                OnPropertyChanged(nameof(IsTranscribing));
                OnPropertyChanged(nameof(StatusText));
            }
        }
    }

    public bool IsRecording => State == RecordingState.Recording;
    public bool IsTranscribing => State == RecordingState.Transcribing;

    public string StatusText => State switch
    {
        RecordingState.Recording    => "Recording...",
        RecordingState.Transcribing => "Transcribing...",
        _                           => "Ready"
    };

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (SetField(ref _errorMessage, value))
                OnPropertyChanged(nameof(HasError));
        }
    }

    public bool HasError => !string.IsNullOrEmpty(_errorMessage);

    public TranscriptionProfile? ActiveProfile
    {
        get => _activeProfile;
        set
        {
            if (SetField(ref _activeProfile, value) && value != null)
                SaveActiveProfile(value.Id);
        }
    }

    public AudioDevice? SelectedDevice
    {
        get => _selectedDevice;
        set
        {
            if (SetField(ref _selectedDevice, value))
                SaveSelectedDevice(value);
        }
    }

    public RelayCommand OpenSettingsCommand { get; }
    public RelayCommand CloseCommand { get; }
    public RelayCommand DismissErrorCommand { get; }

    /// <summary>Raised when the user requests to open the settings window.</summary>
    public event Action? OpenSettingsRequested;

    public OverlayViewModel(
        RecordingOrchestrator orchestrator,
        ISettingsRepository settingsRepository,
        IAudioDeviceEnumerator deviceEnumerator)
    {
        _orchestrator = orchestrator;
        _settingsRepository = settingsRepository;

        _orchestrator.StateChanged += OnOrchestratorStateChanged;
        _orchestrator.ErrorOccurred += OnErrorOccurred;

        OpenSettingsCommand = new RelayCommand(() => OpenSettingsRequested?.Invoke());
        CloseCommand = new RelayCommand(() => System.Windows.Application.Current.MainWindow?.Hide());
        DismissErrorCommand = new RelayCommand(() => ErrorMessage = string.Empty);

        LoadFromSettings(deviceEnumerator);
    }

    private void OnOrchestratorStateChanged(RecordingState state)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            if (state == RecordingState.Recording)
                ErrorMessage = string.Empty;
            State = state;
        });
    }

    private void OnErrorOccurred(string message)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() => ErrorMessage = message);
    }

    private void LoadFromSettings(IAudioDeviceEnumerator deviceEnumerator)
    {
        var settings = _settingsRepository.Load();

        foreach (var p in settings.Profiles)
            Profiles.Add(p);

        _activeProfile = Profiles.FirstOrDefault(p => p.Id == settings.ActiveProfileId)
                         ?? Profiles.FirstOrDefault();

        foreach (var d in deviceEnumerator.GetDevices())
            AudioDevices.Add(d);

        _selectedDevice = AudioDevices.FirstOrDefault(d => d.Id == settings.SelectedAudioDevice?.Id)
                          ?? AudioDevices.FirstOrDefault();
    }

    private void SaveActiveProfile(Guid profileId)
    {
        var settings = _settingsRepository.Load();
        settings.ActiveProfileId = profileId;
        _settingsRepository.Save(settings);
    }

    private void SaveSelectedDevice(AudioDevice? device)
    {
        var settings = _settingsRepository.Load();
        settings.SelectedAudioDevice = device;
        _settingsRepository.Save(settings);
    }
}

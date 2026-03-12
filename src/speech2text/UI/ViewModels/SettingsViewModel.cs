using System.Collections.ObjectModel;
using speech2text.Domain;
using speech2text.Domain.Ports;

namespace speech2text.UI.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsRepository _settingsRepository;

    private TranscriptionProfile? _selectedProfile;
    private string _profileName = string.Empty;
    private string _profileApiKey = string.Empty;
    private string _profileEndpointUrl = string.Empty;
    private string _profileLanguage = string.Empty;
    private string _profileDeploymentName = string.Empty;
    private string _hotkeyBinding = string.Empty;

    public ObservableCollection<TranscriptionProfile> Profiles { get; } = [];

    public TranscriptionProfile? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            SetField(ref _selectedProfile, value);
            LoadProfileFields(value);
            DeleteProfileCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(HasSelectedProfile));
        }
    }

    public bool HasSelectedProfile => _selectedProfile != null;

    public string ProfileName
    {
        get => _profileName;
        set
        {
            SetField(ref _profileName, value);
            if (_selectedProfile != null)
                _selectedProfile.Name = value;
        }
    }
    public string ProfileApiKey        { get => _profileApiKey;        set => SetField(ref _profileApiKey, value); }
    public string ProfileEndpointUrl   { get => _profileEndpointUrl;   set => SetField(ref _profileEndpointUrl, value); }
    public string ProfileLanguage      { get => _profileLanguage;      set => SetField(ref _profileLanguage, value); }
    public string ProfileDeploymentName { get => _profileDeploymentName; set => SetField(ref _profileDeploymentName, value); }
    public string HotkeyBinding        { get => _hotkeyBinding;        set => SetField(ref _hotkeyBinding, value); }

    public RelayCommand AddProfileCommand    { get; }
    public RelayCommand DeleteProfileCommand { get; }
    public RelayCommand SaveCommand          { get; }

    /// <summary>Raised after settings are saved, carrying the updated AppSettings.</summary>
    public event Action<AppSettings>? SettingsSaved;

    public SettingsViewModel(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;

        AddProfileCommand    = new RelayCommand(AddProfile);
        DeleteProfileCommand = new RelayCommand(DeleteProfile, () => SelectedProfile != null);
        SaveCommand          = new RelayCommand(Save);

        var settings = _settingsRepository.Load();
        HotkeyBinding = settings.HotkeyBinding;

        foreach (var p in settings.Profiles)
            Profiles.Add(p);

        SelectedProfile = Profiles.FirstOrDefault();
    }

    private void LoadProfileFields(TranscriptionProfile? profile)
    {
        ProfileName           = profile?.Name        ?? string.Empty;
        ProfileApiKey         = profile?.ApiKey      ?? string.Empty;
        ProfileEndpointUrl    = profile?.EndpointUrl ?? string.Empty;
        ProfileLanguage       = profile?.Language    ?? string.Empty;
        ProfileDeploymentName = profile?.ExtraParameters.GetValueOrDefault("deploymentName") ?? string.Empty;
    }

    private void AddProfile()
    {
        var profile = new TranscriptionProfile { Name = "New Profile" };
        Profiles.Add(profile);
        SelectedProfile = profile;
    }

    private void DeleteProfile()
    {
        if (SelectedProfile == null) return;
        Profiles.Remove(SelectedProfile);
        SelectedProfile = Profiles.FirstOrDefault();
    }

    private void Save()
    {
        if (SelectedProfile != null)
        {
            SelectedProfile.Name        = ProfileName;
            SelectedProfile.ApiKey      = ProfileApiKey;
            SelectedProfile.EndpointUrl = ProfileEndpointUrl;
            SelectedProfile.Language    = ProfileLanguage;
            if (!string.IsNullOrWhiteSpace(ProfileDeploymentName))
                SelectedProfile.ExtraParameters["deploymentName"] = ProfileDeploymentName;
            else
                SelectedProfile.ExtraParameters.Remove("deploymentName");
        }

        var settings = _settingsRepository.Load();
        settings.HotkeyBinding = HotkeyBinding;
        settings.Profiles      = [.. Profiles];
        _settingsRepository.Save(settings);

        SettingsSaved?.Invoke(settings);
    }
}

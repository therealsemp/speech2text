namespace speech2text.Domain.Ports;

/// <summary>
/// Lists the audio input devices available on the system.
/// Used by the UI to populate the device selector in the overlay and settings panel.
/// The domain is unaware of how devices are discovered — the adapter handles the platform specifics.
/// </summary>
public interface IAudioDeviceEnumerator
{
    IReadOnlyList<AudioDevice> GetDevices();
}

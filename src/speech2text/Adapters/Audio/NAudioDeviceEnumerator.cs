using NAudio.Wave;
using speech2text.Domain;
using speech2text.Domain.Ports;

namespace speech2text.Adapters.Audio;

/// <summary>
/// Enumerates microphone input devices using NAudio's WaveInEvent API,
/// which queries the Win32 waveIn device list. Each device is identified
/// by its index (as a string) and its system product name.
/// </summary>
public class NAudioDeviceEnumerator : IAudioDeviceEnumerator
{
    public IReadOnlyList<AudioDevice> GetDevices()
    {
        var devices = new List<AudioDevice>();
        for (int i = 0; i < WaveInEvent.DeviceCount; i++)
        {
            var caps = WaveInEvent.GetCapabilities(i);
            devices.Add(new AudioDevice(i.ToString(), caps.ProductName));
        }
        return devices;
    }
}

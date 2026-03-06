namespace speech2text.Domain;

/// <summary>
/// Value object representing a microphone input device.
/// Identified by its system-level <see cref="Id"/> and displayed to the user via <see cref="Name"/>.
/// Equality is structural (record semantics) — two devices with the same Id and Name are identical.
/// </summary>
public record AudioDevice(string Id, string Name);

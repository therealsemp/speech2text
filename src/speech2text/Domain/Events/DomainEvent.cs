namespace speech2text.Domain.Events;

/// <summary>
/// Base record for all domain events emitted by <see cref="speech2text.Domain.RecordingSession"/>.
/// Events are immutable facts: they describe something that has already happened in the domain.
/// <see cref="OccurredAt"/> is set to UTC at the moment the transition is made.
/// </summary>
public abstract record DomainEvent(DateTimeOffset OccurredAt);

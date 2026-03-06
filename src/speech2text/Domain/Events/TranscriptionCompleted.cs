namespace speech2text.Domain.Events;

public record TranscriptionCompleted(string Text, DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);

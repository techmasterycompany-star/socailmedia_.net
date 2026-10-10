namespace Social_Media_Platform.Application.Common.Exceptions;

public sealed class ConflictException(string message) : Exception(message);

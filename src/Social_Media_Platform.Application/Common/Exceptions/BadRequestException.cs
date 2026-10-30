namespace Social_Media_Platform.Application.Common.Exceptions;

public sealed class BadRequestException(string message) : Exception(message);


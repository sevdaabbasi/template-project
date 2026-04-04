namespace BuildingBlocks.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message) { }
}

public class AlreadyExistsException : DomainException
{
    public AlreadyExistsException(string message) : base(message) { }
}

public class ValidationException : DomainException
{
    public ValidationException(string message) : base(message) { }
}

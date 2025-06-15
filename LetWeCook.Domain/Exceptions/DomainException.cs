namespace LetWeCook.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public ErrorCode ErrorCode { get; } = ErrorCode.UNKNOWN_ERROR;
    public Dictionary<string, string> ContextData { get; } = new();
    protected DomainException(string message, ErrorCode errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    protected DomainException(string message, Exception exception, ErrorCode errorCode)
        : base(message, exception)
    {
        ErrorCode = errorCode;
    }

    public void AddContext(string key, string value)
    {
        if (ContextData.ContainsKey(key))
        {
            ContextData[key] = value;
        }
        else
        {
            ContextData.Add(key, value);
        }
    }
}
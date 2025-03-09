namespace CSharpExtender.Services;

public interface IRedactionService<T>
{
    T Redact(T obj);
    T Redact(string text);
    string RedactToString(T obj);
    string RedactToString(string text);
}

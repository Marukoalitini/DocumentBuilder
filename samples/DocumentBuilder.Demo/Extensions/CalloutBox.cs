using DocumentBuilder.Elements;

namespace DocumentBuilder.Demo.Extensions;

/// <summary>
/// Custom document element defined in the client project to demonstrate Open/Closed Principle (OCP) extensibility.
/// Represents a severity-tagged callout box without modifying the core library source code.
/// </summary>
public sealed class CalloutBox : IDocumentElement
{
    public string Severity { get; }
    public string Message { get; }

    public CalloutBox(string severity, string message)
    {
        ArgumentNullException.ThrowIfNull(severity);
        ArgumentNullException.ThrowIfNull(message);
        Severity = severity;
        Message = message;
    }

    public string Render(int indentationLevel = 0)
    {
        string indent = new(' ', Math.Max(0, indentationLevel) * 2);
        return $"{indent}[{Severity.ToUpperInvariant()}] {Message}";
    }

    public override string ToString() => Render();
}

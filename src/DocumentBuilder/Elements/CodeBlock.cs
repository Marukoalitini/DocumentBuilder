using System.Text;

namespace DocumentBuilder.Elements;

/// <summary>
/// Represents a formatted block of source code or preformatted configuration text.
/// Acts as a leaf node in the composite pattern.
/// </summary>
public sealed class CodeBlock : IDocumentElement
{
    /// <summary>
    /// Gets the raw source code content.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the optional programming or markup language identifier (e.g., "csharp", "json", "bash").
    /// </summary>
    public string? Language { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeBlock"/> class.
    /// </summary>
    /// <param name="code">The source code content.</param>
    /// <param name="language">Optional language identifier for syntax highlighting.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="code"/> is null.</exception>
    public CodeBlock(string code, string? language = null)
    {
        ArgumentNullException.ThrowIfNull(code);
        Code = code;
        Language = language;
    }

    /// <inheritdoc />
    public string Render(int indentationLevel = 0)
    {
        string indent = new(' ', Math.Max(0, indentationLevel) * 2);
        var sb = new StringBuilder();
        string header = string.IsNullOrWhiteSpace(Language) ? "[Code]" : $"[Code: {Language}]";
        sb.AppendLine($"{indent}{header}");

        foreach (string line in Code.Split('\n'))
        {
            sb.AppendLine($"{indent}  {line.TrimEnd('\r')}");
        }

        return sb.ToString().TrimEnd();
    }

    /// <inheritdoc />
    public override string ToString() => Render();
}

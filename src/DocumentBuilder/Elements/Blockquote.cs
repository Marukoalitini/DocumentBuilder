namespace DocumentBuilder.Elements;

/// <summary>
/// Represents a quotation block with an optional citation or author attribution.
/// Acts as a leaf node in the composite pattern.
/// </summary>
public sealed class Blockquote : IDocumentElement
{
    /// <summary>
    /// Gets the quoted text content.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the optional author or source attribution of the quote.
    /// </summary>
    public string? Citation { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Blockquote"/> class.
    /// </summary>
    /// <param name="text">The quote text.</param>
    /// <param name="citation">Optional author or source attribution.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
    public Blockquote(string text, string? citation = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        Text = text;
        Citation = citation;
    }

    /// <inheritdoc />
    public string Render(int indentationLevel = 0)
    {
        string indent = new(' ', Math.Max(0, indentationLevel) * 2);
        return string.IsNullOrWhiteSpace(Citation)
            ? $"{indent}\"{Text}\""
            : $"{indent}\"{Text}\" — {Citation}";
    }

    /// <inheritdoc />
    public override string ToString() => Render();
}

namespace DocumentBuilder.Elements;

/// <summary>
/// Represents a paragraph of text within a document.
/// Acts as a leaf node in the composite pattern.
/// </summary>
public sealed class Paragraph : IDocumentElement
{
    /// <summary>
    /// Gets the text content of the paragraph.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Paragraph"/> class.
    /// </summary>
    /// <param name="text">The paragraph text.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
    public Paragraph(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        Text = text;
    }

    /// <summary>
    /// Renders the paragraph with the specified indentation level.
    /// </summary>
    /// <param name="indentationLevel">The zero-based indentation level.</param>
    /// <returns>A formatted paragraph string.</returns>
    public string Render(int indentationLevel = 0)
    {
        // Wrap or indent each line if multiple lines exist
        string indent = new string(' ', Math.Max(0, indentationLevel) * 2);
        if (!Text.Contains('\n'))
        {
            return $"{indent}{Text}";
        }

        string[] lines = Text.Split('\n');
        return string.Join(Environment.NewLine, lines.Select(line => $"{indent}{line.TrimEnd('\r')}"));
    }

    /// <inheritdoc />
    public override string ToString() => Render();
}

namespace DocumentBuilder.Elements;

/// <summary>
/// Represents a heading element in a document (e.g., Level 1, Level 2, Level 3).
/// Acts as a leaf node in the composite pattern.
/// </summary>
public sealed class Title : IDocumentElement
{
    /// <summary>
    /// Gets the textual content of the title.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the hierarchical level of the title, between 1 and 6.
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Title"/> class.
    /// </summary>
    /// <param name="text">The title text.</param>
    /// <param name="level">The heading level (between 1 and 6). Defaults to 1.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="level"/> is not between 1 and 6.</exception>
    public Title(string text, int level = 1)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (level is < 1 or > 6)
        {
            throw new ArgumentOutOfRangeException(nameof(level), level, "Heading level must be between 1 and 6.");
        }

        Text = text;
        Level = level;
    }

    /// <summary>
    /// Renders the title with the specified indentation level.
    /// </summary>
    /// <param name="indentationLevel">The zero-based indentation level.</param>
    /// <returns>A formatted title string.</returns>
    public string Render(int indentationLevel = 0)
    {
        string indent = new string(' ', Math.Max(0, indentationLevel) * 2);
        return $"{indent}{Text}";
    }

    /// <inheritdoc />
    public override string ToString() => Render();
}

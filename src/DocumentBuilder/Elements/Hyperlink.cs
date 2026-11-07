namespace DocumentBuilder.Elements;

/// <summary>
/// Represents a hyperlink element pointing to an external URL or document anchor.
/// Acts as a leaf node in the composite pattern.
/// </summary>
public sealed class Hyperlink : IDocumentElement
{
    /// <summary>
    /// Gets the display text of the hyperlink.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the target URL or URI of the hyperlink.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Gets the optional tooltip title of the hyperlink.
    /// </summary>
    public string? Tooltip { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Hyperlink"/> class.
    /// </summary>
    /// <param name="text">The display text for the link.</param>
    /// <param name="url">The destination URL.</param>
    /// <param name="tooltip">Optional tooltip text.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> or <paramref name="url"/> is null.</exception>
    public Hyperlink(string text, string url, string? tooltip = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(url);
        Text = text;
        Url = url;
        Tooltip = tooltip;
    }

    /// <inheritdoc />
    public string Render(int indentationLevel = 0)
    {
        string indent = new(' ', Math.Max(0, indentationLevel) * 2);
        return $"{indent}{Text} ({Url})";
    }

    /// <inheritdoc />
    public override string ToString() => Render();
}

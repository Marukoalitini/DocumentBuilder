namespace DocumentBuilder.Elements;

/// <summary>
/// Represents an image element within a document.
/// Acts as a leaf node in the composite pattern.
/// </summary>
public sealed class Image : IDocumentElement
{
    /// <summary>
    /// Gets the source URI or file path of the image.
    /// </summary>
    public string Source { get; }

    /// <summary>
    /// Gets the optional caption displayed beneath the image.
    /// </summary>
    public string? Caption { get; }

    /// <summary>
    /// Gets the optional alternative text describing the image.
    /// </summary>
    public string? AltText { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Image"/> class.
    /// </summary>
    /// <param name="source">The image source URL or local path.</param>
    /// <param name="caption">Optional caption text.</param>
    /// <param name="altText">Optional alternative text description.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    public Image(string source, string? caption = null, string? altText = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        Source = source;
        Caption = caption;
        AltText = altText;
    }

    /// <summary>
    /// Renders the image element into a textual representation.
    /// </summary>
    /// <param name="indentationLevel">The zero-based indentation level.</param>
    /// <returns>A formatted representation of the image element.</returns>
    public string Render(int indentationLevel = 0)
    {
        string indent = new string(' ', Math.Max(0, indentationLevel) * 2);
        string captionPart = string.IsNullOrWhiteSpace(Caption) ? string.Empty : $" - \"{Caption}\"";
        string altPart = string.IsNullOrWhiteSpace(AltText) ? string.Empty : $" [alt: {AltText}]";

        return $"{indent}[Image: {Source}{altPart}{captionPart}]";
    }

    /// <inheritdoc />
    public override string ToString() => Render();
}

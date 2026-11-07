using System.Text;

namespace DocumentBuilder.Elements;

/// <summary>
/// Represents the root composite element of a document hierarchy.
/// Contains document metadata and a collection of child document elements.
/// </summary>
public sealed class Document : CompositeElement
{
    private readonly Dictionary<string, string> _metadata = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets or sets the document title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the document author.
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// Gets the timestamp when the document was initialized.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets a read-only dictionary of the document metadata.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata => _metadata.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="Document"/> class.
    /// </summary>
    /// <param name="title">The document title.</param>
    /// <param name="author">Optional author name.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="title"/> is null.</exception>
    public Document(string title, string? author = null)
    {
        ArgumentNullException.ThrowIfNull(title);
        Title = title;
        Author = author;
    }

    /// <summary>
    /// Adds a metadata key-value pair to the document.
    /// </summary>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>The current document instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> or <paramref name="value"/> is null.</exception>
    public Document SetMetadata(string key, string value)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);
        _metadata[key] = value;
        return this;
    }

    /// <summary>
    /// Adds a child element to the document root.
    /// </summary>
    /// <param name="element">The document element to add.</param>
    /// <returns>The current document instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> is null.</exception>
    public Document Add(IDocumentElement element)
    {
        AddChild(element);
        return this;
    }

    /// <summary>
    /// Renders the document and all its child elements.
    /// </summary>
    /// <param name="indentationLevel">The zero-based indentation level.</param>
    /// <returns>A formatted string of the entire document.</returns>
    public override string Render(int indentationLevel = 0)
    {
        var sb = new StringBuilder();

        sb.AppendLine(Title);
        if (!string.IsNullOrWhiteSpace(Author))
        {
            sb.AppendLine($"Author: {Author}");
        }

        if (_metadata.Count > 0)
        {
            foreach (var (key, value) in _metadata)
            {
                sb.AppendLine($"{key}: {value}");
            }
        }

        sb.AppendLine();

        foreach (IDocumentElement element in Elements)
        {
            sb.AppendLine(element.Render(indentationLevel));
            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }
}

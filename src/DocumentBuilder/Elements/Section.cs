using System.Text;

namespace DocumentBuilder.Elements;

/// <summary>
/// Represents a section containing child elements and nested sub-sections.
/// Acts as a composite node in the composite pattern.
/// </summary>
public sealed class Section : IDocumentElement
{
    private readonly List<IDocumentElement> _elements = [];

    /// <summary>
    /// Gets the title of the section.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets a read-only collection of the child document elements in this section.
    /// </summary>
    public IReadOnlyList<IDocumentElement> Elements => _elements.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="Section"/> class.
    /// </summary>
    /// <param name="title">The section title.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="title"/> is null.</exception>
    public Section(string title)
    {
        ArgumentNullException.ThrowIfNull(title);
        Title = title;
    }

    /// <summary>
    /// Adds a child element to the section.
    /// </summary>
    /// <param name="element">The document element to add.</param>
    /// <returns>The current section instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> is null.</exception>
    public Section Add(IDocumentElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        _elements.Add(element);
        return this;
    }

    /// <summary>
    /// Removes a child element from the section.
    /// </summary>
    /// <param name="element">The document element to remove.</param>
    /// <returns><c>true</c> if the element was successfully removed; otherwise, <c>false</c>.</returns>
    public bool Remove(IDocumentElement element)
    {
        return _elements.Remove(element);
    }

    /// <summary>
    /// Clears all elements from the section.
    /// </summary>
    public void Clear()
    {
        _elements.Clear();
    }

    /// <summary>
    /// Renders the section header and recursively renders all child elements.
    /// </summary>
    /// <param name="indentationLevel">The zero-based indentation level.</param>
    /// <returns>A formatted string of the section and all its children.</returns>
    public string Render(int indentationLevel = 0)
    {
        string indent = new string(' ', Math.Max(0, indentationLevel) * 2);
        var sb = new StringBuilder();

        sb.AppendLine($"{indent}{Title}");

        foreach (IDocumentElement element in _elements)
        {
            sb.AppendLine(element.Render(indentationLevel + 1));
        }

        return sb.ToString().TrimEnd();
    }

    /// <inheritdoc />
    public override string ToString() => Render();
}

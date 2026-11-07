using System.Text;

namespace DocumentBuilder.Elements;

/// <summary>
/// Represents a section containing child elements and nested sub-sections.
/// Acts as a composite node in the composite pattern.
/// </summary>
public sealed class Section : CompositeElement
{
    /// <summary>
    /// Gets the title of the section.
    /// </summary>
    public string Title { get; }

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
        AddChild(element);
        return this;
    }

    /// <summary>
    /// Renders the section header and recursively renders all child elements.
    /// </summary>
    /// <param name="indentationLevel">The zero-based indentation level.</param>
    /// <returns>A formatted string of the section and all its children.</returns>
    public override string Render(int indentationLevel = 0)
    {
        string indent = new(' ', Math.Max(0, indentationLevel) * 2);
        var sb = new StringBuilder();

        sb.AppendLine($"{indent}{Title}");

        foreach (IDocumentElement element in Elements)
        {
            sb.AppendLine(element.Render(indentationLevel + 1));
        }

        return sb.ToString().TrimEnd();
    }
}

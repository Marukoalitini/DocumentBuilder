using DocumentBuilder.Elements;

namespace DocumentBuilder.Decorators;

/// <summary>
/// Serves as the abstract base class for all document element decorators.
/// Wraps an existing <see cref="IDocumentElement"/> to dynamically attach additional formatting or visual behavior
/// without altering the underlying element class.
/// </summary>
public abstract class DocumentElementDecorator : IDocumentElement
{
    /// <summary>
    /// Gets the wrapped inner document element.
    /// </summary>
    public IDocumentElement InnerElement { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentElementDecorator"/> class.
    /// </summary>
    /// <param name="innerElement">The document element to be decorated.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="innerElement"/> is null.</exception>
    protected DocumentElementDecorator(IDocumentElement innerElement)
    {
        ArgumentNullException.ThrowIfNull(innerElement);
        InnerElement = innerElement;
    }

    /// <summary>
    /// Applies the decorator's transformation to already formatted inner content.
    /// Custom user-defined decorators override this method to integrate seamlessly with all exporters.
    /// </summary>
    /// <param name="innerContent">The formatted string representation of the inner element.</param>
    /// <param name="indentationLevel">The zero-based indentation level.</param>
    /// <returns>The decorated string content.</returns>
    public virtual string FormatDecoratedContent(string innerContent, int indentationLevel = 0)
    {
        string indent = new(' ', Math.Max(0, indentationLevel) * 2);
        return $"{indent}{innerContent}";
    }

    /// <inheritdoc />
    public virtual string Render(int indentationLevel = 0)
    {
        string inner = InnerElement.Render(0).Trim();
        return FormatDecoratedContent(inner, indentationLevel);
    }

    /// <inheritdoc />
    public override string ToString() => Render();
}

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

    /// <inheritdoc />
    public virtual string Render(int indentationLevel = 0)
    {
        return InnerElement.Render(indentationLevel);
    }

    /// <inheritdoc />
    public override string ToString() => Render();
}

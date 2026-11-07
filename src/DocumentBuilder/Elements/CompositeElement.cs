namespace DocumentBuilder.Elements;

/// <summary>
/// Serves as the abstract base class for composite nodes in the document hierarchy
/// that can contain child <see cref="IDocumentElement"/> instances.
/// Implements the Composite participant of the GoF Composite design pattern.
/// </summary>
public abstract class CompositeElement : IDocumentElement
{
    private readonly List<IDocumentElement> _elements = [];

    /// <summary>
    /// Gets a read-only collection of the child document elements contained in this composite node.
    /// </summary>
    public IReadOnlyList<IDocumentElement> Elements => _elements.AsReadOnly();

    /// <summary>
    /// Adds a child element to the internal composite collection.
    /// </summary>
    /// <param name="element">The document element to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> is null.</exception>
    protected void AddChild(IDocumentElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        _elements.Add(element);
    }

    /// <summary>
    /// Removes a child element from this composite container.
    /// </summary>
    /// <param name="element">The document element to remove.</param>
    /// <returns><c>true</c> if the element was successfully removed; otherwise, <c>false</c>.</returns>
    public bool Remove(IDocumentElement element)
    {
        return _elements.Remove(element);
    }

    /// <summary>
    /// Removes all child elements from this composite container.
    /// </summary>
    public void Clear()
    {
        _elements.Clear();
    }

    /// <inheritdoc />
    public abstract string Render(int indentationLevel = 0);

    /// <inheritdoc />
    public override string ToString() => Render();
}

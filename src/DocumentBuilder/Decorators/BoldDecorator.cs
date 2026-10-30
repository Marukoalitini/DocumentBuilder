using DocumentBuilder.Elements;

namespace DocumentBuilder.Decorators;

/// <summary>
/// Decorates a <see cref="IDocumentElement"/> with bold text emphasis.
/// Part of the Decorator design pattern.
/// </summary>
public sealed class BoldDecorator : DocumentElementDecorator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoldDecorator"/> class.
    /// </summary>
    /// <param name="innerElement">The document element to decorate with bold emphasis.</param>
    public BoldDecorator(IDocumentElement innerElement)
        : base(innerElement)
    {
    }

    /// <inheritdoc />
    public override string Render(int indentationLevel = 0)
    {
        string indent = new(' ', Math.Max(0, indentationLevel) * 2);
        string inner = InnerElement.Render(0).Trim();
        return $"{indent}**{inner}**";
    }
}

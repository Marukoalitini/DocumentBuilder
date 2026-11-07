using DocumentBuilder.Elements;

namespace DocumentBuilder.Decorators;

/// <summary>
/// Decorates a <see cref="IDocumentElement"/> with background highlight emphasis.
/// Part of the Decorator design pattern.
/// </summary>
public sealed class HighlightDecorator : DocumentElementDecorator
{
    /// <summary>
    /// Gets the optional highlight color (e.g., "#fef08a" or "yellow").
    /// </summary>
    public string Color { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightDecorator"/> class.
    /// </summary>
    /// <param name="innerElement">The document element to highlight.</param>
    /// <param name="color">The optional highlight color. Defaults to "#fef08a".</param>
    public HighlightDecorator(IDocumentElement innerElement, string color = "#fef08a")
        : base(innerElement)
    {
        ArgumentNullException.ThrowIfNull(color);
        Color = color;
    }

    /// <inheritdoc />
    public override string FormatDecoratedContent(string innerContent, int indentationLevel = 0)
    {
        string indent = new(' ', Math.Max(0, indentationLevel) * 2);
        return $"{indent}=={innerContent}==";
    }
}

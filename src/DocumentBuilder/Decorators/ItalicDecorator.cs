using DocumentBuilder.Elements;

namespace DocumentBuilder.Decorators;

/// <summary>
/// Decorates a <see cref="IDocumentElement"/> with italic text emphasis.
/// Part of the Decorator design pattern.
/// </summary>
public sealed class ItalicDecorator : DocumentElementDecorator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItalicDecorator"/> class.
    /// </summary>
    /// <param name="innerElement">The document element to decorate with italic emphasis.</param>
    public ItalicDecorator(IDocumentElement innerElement)
        : base(innerElement)
    {
    }

    /// <inheritdoc />
    public override string FormatDecoratedContent(string innerContent, int indentationLevel = 0)
    {
        string indent = new(' ', Math.Max(0, indentationLevel) * 2);
        return $"{indent}*{innerContent}*";
    }
}

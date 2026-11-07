using DocumentBuilder.Elements;

namespace DocumentBuilder.Decorators;

/// <summary>
/// Decorates a <see cref="IDocumentElement"/> with underline formatting.
/// Part of the Decorator design pattern.
/// </summary>
public sealed class UnderlineDecorator : DocumentElementDecorator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnderlineDecorator"/> class.
    /// </summary>
    /// <param name="innerElement">The document element to decorate with an underline.</param>
    public UnderlineDecorator(IDocumentElement innerElement)
        : base(innerElement)
    {
    }

    /// <inheritdoc />
    public override string FormatDecoratedContent(string innerContent, int indentationLevel = 0)
    {
        string indent = new(' ', Math.Max(0, indentationLevel) * 2);
        return $"{indent}<u>{innerContent}</u>";
    }
}

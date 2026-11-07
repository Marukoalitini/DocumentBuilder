using DocumentBuilder.Elements;

namespace DocumentBuilder.Decorators;

/// <summary>
/// Specifies horizontal text alignment options for decorated document elements.
/// </summary>
public enum TextAlignment
{
    /// <summary>
    /// Aligns content to the left margin.
    /// </summary>
    Left,

    /// <summary>
    /// Centers content horizontally.
    /// </summary>
    Center,

    /// <summary>
    /// Aligns content to the right margin.
    /// </summary>
    Right,

    /// <summary>
    /// Justifies content evenly between margins.
    /// </summary>
    Justify
}

/// <summary>
/// Decorates a <see cref="IDocumentElement"/> with horizontal alignment metadata (Left, Center, Right, Justify).
/// Part of the Decorator design pattern.
/// </summary>
public sealed class AlignmentDecorator : DocumentElementDecorator
{
    /// <summary>
    /// Gets the horizontal alignment applied to the wrapped element.
    /// </summary>
    public TextAlignment Alignment { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlignmentDecorator"/> class.
    /// </summary>
    /// <param name="innerElement">The document element to align.</param>
    /// <param name="alignment">The desired horizontal alignment.</param>
    public AlignmentDecorator(IDocumentElement innerElement, TextAlignment alignment)
        : base(innerElement)
    {
        Alignment = alignment;
    }

    /// <inheritdoc />
    public override string FormatDecoratedContent(string innerContent, int indentationLevel = 0)
    {
        string indent = new(' ', Math.Max(0, indentationLevel) * 2);
        return Alignment switch
        {
            TextAlignment.Center => $"{indent}[CENTER] {innerContent}",
            TextAlignment.Right => $"{indent}[RIGHT] {innerContent}",
            TextAlignment.Justify => $"{indent}[JUSTIFY] {innerContent}",
            _ => $"{indent}{innerContent}"
        };
    }
}

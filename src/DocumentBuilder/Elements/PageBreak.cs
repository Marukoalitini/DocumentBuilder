namespace DocumentBuilder.Elements;

/// <summary>
/// Represents an explicit page break instruction for paginated or printable export formats.
/// Acts as a leaf node in the composite pattern.
/// </summary>
public sealed class PageBreak : IDocumentElement
{
    /// <inheritdoc />
    public string Render(int indentationLevel = 0)
    {
        string indent = new(' ', Math.Max(0, indentationLevel) * 2);
        return $"{indent}==================== [PAGE BREAK] ====================";
    }

    /// <inheritdoc />
    public override string ToString() => Render();
}

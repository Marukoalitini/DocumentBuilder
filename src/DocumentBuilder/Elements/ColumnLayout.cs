using System.Text;

namespace DocumentBuilder.Elements;

/// <summary>
/// Represents a multi-column layout container that arranges child elements side-by-side.
/// Acts as a composite node in the GoF Composite pattern.
/// </summary>
public sealed class ColumnLayout : CompositeElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnLayout"/> class with the specified column elements.
    /// </summary>
    /// <param name="columns">The column elements to include in this layout.</param>
    public ColumnLayout(params IDocumentElement[] columns)
        : this((IEnumerable<IDocumentElement>)columns)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnLayout"/> class.
    /// </summary>
    /// <param name="columns">Optional initial collection of column elements.</param>
    public ColumnLayout(IEnumerable<IDocumentElement>? columns = null)
    {
        if (columns != null)
        {
            foreach (IDocumentElement column in columns)
            {
                AddChild(column);
            }
        }
    }

    /// <summary>
    /// Adds a child element as a column in this layout.
    /// </summary>
    /// <param name="columnElement">The document element representing the column content.</param>
    /// <returns>The current <see cref="ColumnLayout"/> instance for chaining.</returns>
    public ColumnLayout AddColumn(IDocumentElement columnElement)
    {
        AddChild(columnElement);
        return this;
    }

    /// <inheritdoc />
    public override string Render(int indentationLevel = 0)
    {
        if (Elements.Count == 0)
        {
            return string.Empty;
        }

        string indent = new(' ', Math.Max(0, indentationLevel) * 2);
        var sb = new StringBuilder();

        for (int i = 0; i < Elements.Count; i++)
        {
            sb.AppendLine($"{indent}[Column {i + 1}/{Elements.Count}]");
            sb.AppendLine(Elements[i].Render(indentationLevel + 1));
        }

        return sb.ToString().TrimEnd();
    }
}

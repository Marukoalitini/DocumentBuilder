namespace DocumentBuilder.Elements;

/// <summary>
/// Represents a component in the document composite hierarchy.
/// Both leaf elements and composite containers implement this contract.
/// </summary>
public interface IDocumentElement
{
    /// <summary>
    /// Renders the document element into its textual representation with the specified indentation level.
    /// </summary>
    /// <param name="indentationLevel">The zero-based indentation level used for hierarchical nesting.</param>
    /// <returns>A formatted string representation of the element.</returns>
    string Render(int indentationLevel = 0);
}

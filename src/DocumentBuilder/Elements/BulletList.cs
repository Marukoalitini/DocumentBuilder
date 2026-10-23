using System.Text;

namespace DocumentBuilder.Elements;

/// <summary>
/// Represents an unordered bullet list of textual items.
/// Acts as a leaf node in the composite pattern.
/// </summary>
public sealed class BulletList : IDocumentElement
{
    private readonly List<string> _items = [];

    /// <summary>
    /// Gets the list of items contained in this bullet list.
    /// </summary>
    public IReadOnlyList<string> Items => _items.AsReadOnly();

    /// <summary>
    /// Gets the character or string used as the bullet marker (e.g., "-", "*", "•").
    /// </summary>
    public string BulletMarker { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BulletList"/> class.
    /// </summary>
    /// <param name="items">Optional initial collection of list items.</param>
    /// <param name="bulletMarker">The bullet marker string. Defaults to "-".</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="bulletMarker"/> is null.</exception>
    public BulletList(IEnumerable<string>? items = null, string bulletMarker = "-")
    {
        ArgumentNullException.ThrowIfNull(bulletMarker);
        BulletMarker = bulletMarker;

        if (items != null)
        {
            _items.AddRange(items);
        }
    }

    /// <summary>
    /// Adds an item to the bullet list.
    /// </summary>
    /// <param name="item">The text item to add.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    public BulletList AddItem(string item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
        return this;
    }

    /// <summary>
    /// Adds multiple items to the bullet list.
    /// </summary>
    /// <param name="items">The items to add.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is null.</exception>
    public BulletList AddItems(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items.AddRange(items);
        return this;
    }

    /// <summary>
    /// Renders the bullet list with the specified indentation level.
    /// </summary>
    /// <param name="indentationLevel">The zero-based indentation level.</param>
    /// <returns>A formatted string containing each item prefixed with the bullet marker.</returns>
    public string Render(int indentationLevel = 0)
    {
        if (_items.Count == 0)
        {
            return string.Empty;
        }

        string indent = new string(' ', Math.Max(0, indentationLevel) * 2);
        var sb = new StringBuilder();

        for (int i = 0; i < _items.Count; i++)
        {
            sb.Append($"{indent}{BulletMarker} {_items[i]}");
            if (i < _items.Count - 1)
            {
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    /// <inheritdoc />
    public override string ToString() => Render();
}

using System.Text;

namespace DocumentBuilder.Elements;

/// <summary>
/// Represents a numbered sequential list of textual items.
/// Acts as a leaf node in the composite pattern.
/// </summary>
public sealed class OrderedList : IDocumentElement
{
    private readonly List<string> _items = [];

    /// <summary>
    /// Gets the list of items contained in this ordered list.
    /// </summary>
    public IReadOnlyList<string> Items => _items.AsReadOnly();

    /// <summary>
    /// Gets the starting number of the sequence.
    /// </summary>
    public int StartNumber { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderedList"/> class.
    /// </summary>
    /// <param name="items">Optional initial collection of list items.</param>
    /// <param name="startNumber">The starting number of the sequence. Defaults to 1.</param>
    public OrderedList(IEnumerable<string>? items = null, int startNumber = 1)
    {
        StartNumber = startNumber;

        if (items != null)
        {
            _items.AddRange(items);
        }
    }

    /// <summary>
    /// Adds an item to the ordered list.
    /// </summary>
    /// <param name="item">The text item to add.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    public OrderedList AddItem(string item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
        return this;
    }

    /// <summary>
    /// Adds multiple items to the ordered list.
    /// </summary>
    /// <param name="items">The items to add.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is null.</exception>
    public OrderedList AddItems(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items.AddRange(items);
        return this;
    }

    /// <summary>
    /// Renders the ordered list with the specified indentation level and sequential numbers.
    /// </summary>
    /// <param name="indentationLevel">The zero-based indentation level.</param>
    /// <returns>A formatted string with sequentially numbered items.</returns>
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
            int number = StartNumber + i;
            sb.Append($"{indent}{number}. {_items[i]}");
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

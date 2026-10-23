using System.Text;

namespace DocumentBuilder.Elements;

/// <summary>
/// Represents a structured table with columns and rows.
/// Acts as a leaf node in the composite pattern.
/// </summary>
public sealed class Table : IDocumentElement
{
    private readonly List<string> _headers = [];
    private readonly List<List<string>> _rows = [];

    /// <summary>
    /// Gets a read-only list of column headers.
    /// </summary>
    public IReadOnlyList<string> Headers => _headers.AsReadOnly();

    /// <summary>
    /// Gets a read-only list of rows, where each row is a list of cell values.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<string>> Rows => _rows.Select(r => (IReadOnlyList<string>)r.AsReadOnly()).ToList().AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="Table"/> class with specified column headers.
    /// </summary>
    /// <param name="headers">The collection of column headers.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="headers"/> is null.</exception>
    public Table(IEnumerable<string> headers)
    {
        ArgumentNullException.ThrowIfNull(headers);
        _headers.AddRange(headers);
    }

    /// <summary>
    /// Adds a row of cells to the table.
    /// </summary>
    /// <param name="cells">The cell values for the row.</param>
    /// <returns>The current table instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="cells"/> is null.</exception>
    public Table AddRow(params string[] cells)
    {
        ArgumentNullException.ThrowIfNull(cells);
        _rows.Add([.. cells]);
        return this;
    }

    /// <summary>
    /// Adds a row of cells to the table from an enumerable collection.
    /// </summary>
    /// <param name="cells">The cell values for the row.</param>
    /// <returns>The current table instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="cells"/> is null.</exception>
    public Table AddRow(IEnumerable<string> cells)
    {
        ArgumentNullException.ThrowIfNull(cells);
        _rows.Add([.. cells]);
        return this;
    }

    /// <summary>
    /// Renders the table into an aligned ASCII representation.
    /// </summary>
    /// <param name="indentationLevel">The zero-based indentation level.</param>
    /// <returns>A formatted ASCII table string.</returns>
    public string Render(int indentationLevel = 0)
    {
        string indent = new string(' ', Math.Max(0, indentationLevel) * 2);

        // Determine column count and widths
        int columnCount = _headers.Count;
        foreach (var row in _rows)
        {
            if (row.Count > columnCount)
            {
                columnCount = row.Count;
            }
        }

        if (columnCount == 0)
        {
            return $"{indent}[Empty Table]";
        }

        int[] colWidths = new int[columnCount];

        for (int i = 0; i < columnCount; i++)
        {
            string header = i < _headers.Count ? _headers[i] : string.Empty;
            colWidths[i] = Math.Max(colWidths[i], header.Length);
        }

        foreach (var row in _rows)
        {
            for (int i = 0; i < columnCount; i++)
            {
                string cell = i < row.Count ? row[i] : string.Empty;
                colWidths[i] = Math.Max(colWidths[i], cell.Length);
            }
        }

        var sb = new StringBuilder();

        // Helper line separator
        string CreateSeparator()
        {
            var parts = colWidths.Select(w => new string('-', w + 2));
            return $"{indent}+{string.Join("+", parts)}+";
        }

        string FormatRow(IReadOnlyList<string> rowCells)
        {
            var parts = new List<string>();
            for (int i = 0; i < columnCount; i++)
            {
                string cell = i < rowCells.Count ? rowCells[i] : string.Empty;
                parts.Add($" {cell.PadRight(colWidths[i])} ");
            }
            return $"{indent}|{string.Join("|", parts)}|";
        }

        sb.AppendLine(CreateSeparator());

        if (_headers.Count > 0)
        {
            sb.AppendLine(FormatRow(_headers));
            sb.AppendLine(CreateSeparator());
        }

        foreach (var row in _rows)
        {
            sb.AppendLine(FormatRow(row));
        }

        sb.Append(CreateSeparator());

        return sb.ToString();
    }

    /// <inheritdoc />
    public override string ToString() => Render();
}

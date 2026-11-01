using DocumentBuilder.Elements;

namespace DocumentBuilder.Observers;

/// <summary>
/// Provides detailed state and progress data for C# events raised during the document export lifecycle.
/// </summary>
public sealed class ExportProgressEventArgs : EventArgs
{
    /// <summary>
    /// Gets the title of the document currently being exported.
    /// </summary>
    public string DocumentTitle { get; }

    /// <summary>
    /// Gets the current export lifecycle stage (e.g., "Started", "ElementProcessed", "Completed").
    /// </summary>
    public string Stage { get; }

    /// <summary>
    /// Gets the number of top-level elements processed so far.
    /// </summary>
    public int ProcessedElements { get; }

    /// <summary>
    /// Gets the total number of top-level elements in the document.
    /// </summary>
    public int TotalElements { get; }

    /// <summary>
    /// Gets the element currently being processed, if applicable.
    /// </summary>
    public IDocumentElement? CurrentElement { get; }

    /// <summary>
    /// Gets the completion percentage of the export process (from 0 to 100).
    /// </summary>
    public double ProgressPercentage => TotalElements == 0 ? 100.0 : Math.Round((double)ProcessedElements / TotalElements * 100.0, 1);

    /// <summary>
    /// Gets the UTC timestamp when the progress event occurred.
    /// </summary>
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportProgressEventArgs"/> class.
    /// </summary>
    /// <param name="documentTitle">The title of the document being exported.</param>
    /// <param name="stage">The current stage description.</param>
    /// <param name="processedElements">The count of processed elements.</param>
    /// <param name="totalElements">The total count of elements.</param>
    /// <param name="currentElement">The element currently being exported, if any.</param>
    public ExportProgressEventArgs(
        string documentTitle,
        string stage,
        int processedElements,
        int totalElements,
        IDocumentElement? currentElement = null)
    {
        ArgumentNullException.ThrowIfNull(documentTitle);
        ArgumentNullException.ThrowIfNull(stage);

        DocumentTitle = documentTitle;
        Stage = stage;
        ProcessedElements = processedElements;
        TotalElements = totalElements;
        CurrentElement = currentElement;
    }
}

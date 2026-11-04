using DocumentBuilder.Elements;

namespace DocumentBuilder.Exporters;

/// <summary>
/// Represents the Context participant in the GoF Strategy design pattern.
/// Maintains a reference to an <see cref="IDocumentExporter"/> strategy and allows switching
/// the active export algorithm dynamically at runtime.
/// </summary>
public sealed class DocumentExportContext
{
    /// <summary>
    /// Gets the currently configured export strategy.
    /// </summary>
    public IDocumentExporter Strategy { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentExportContext"/> class.
    /// Defaults to <see cref="MarkdownDocumentExporter"/> when no strategy is specified.
    /// </summary>
    /// <param name="strategy">Optional initial export strategy. Defaults to <see cref="MarkdownDocumentExporter"/>.</param>
    public DocumentExportContext(IDocumentExporter? strategy = null)
    {
        Strategy = strategy ?? new MarkdownDocumentExporter();
    }

    /// <summary>
    /// Replaces the active export strategy at runtime.
    /// </summary>
    /// <param name="strategy">The new export strategy to use.</param>
    /// <returns>The current context instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="strategy"/> is null.</exception>
    public DocumentExportContext SetStrategy(IDocumentExporter strategy)
    {
        ArgumentNullException.ThrowIfNull(strategy);
        Strategy = strategy;
        return this;
    }

    /// <summary>
    /// Exports the specified document using the currently configured strategy.
    /// </summary>
    /// <param name="document">The document to export.</param>
    /// <returns>The formatted string output produced by the active strategy.</returns>
    public string Export(Document document)
    {
        return Strategy.Export(document);
    }

    /// <summary>
    /// Exports the specified document to a file on disk using the currently configured strategy.
    /// </summary>
    /// <param name="document">The document to export.</param>
    /// <param name="filePath">The destination file path.</param>
    public void ExportToFile(Document document, string filePath)
    {
        Strategy.ExportToFile(document, filePath);
    }

    /// <summary>
    /// Asynchronously exports the specified document to a file on disk using the currently configured strategy.
    /// </summary>
    /// <param name="document">The document to export.</param>
    /// <param name="filePath">The destination file path.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ExportToFileAsync(Document document, string filePath, CancellationToken cancellationToken = default)
    {
        return Strategy.ExportToFileAsync(document, filePath, cancellationToken);
    }
}

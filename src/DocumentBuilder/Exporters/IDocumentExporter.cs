using DocumentBuilder.Elements;

namespace DocumentBuilder.Exporters;

/// <summary>
/// Defines the contract for exporting a <see cref="Document"/> into various output formats.
/// Serves as the strategy interface for document exporters.
/// </summary>
public interface IDocumentExporter
{
    /// <summary>
    /// Gets the file extension associated with this exporter (e.g., ".txt", ".html").
    /// </summary>
    string FileExtension { get; }

    /// <summary>
    /// Exports the specified document into a formatted string.
    /// </summary>
    /// <param name="document">The document to export.</param>
    /// <returns>The exported string representation of the document.</returns>
    string Export(Document document);

    /// <summary>
    /// Exports the specified document and saves the result to a file on disk.
    /// </summary>
    /// <param name="document">The document to export.</param>
    /// <param name="filePath">The absolute or relative path to the destination file.</param>
    void ExportToFile(Document document, string filePath);

    /// <summary>
    /// Asynchronously exports the specified document and saves the result to a file on disk.
    /// </summary>
    /// <param name="document">The document to export.</param>
    /// <param name="filePath">The target file path.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous export operation.</returns>
    Task ExportToFileAsync(Document document, string filePath, CancellationToken cancellationToken = default);
}

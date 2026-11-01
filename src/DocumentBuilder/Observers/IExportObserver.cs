using DocumentBuilder.Elements;

namespace DocumentBuilder.Observers;

/// <summary>
/// Defines the GoF Observer contract for receiving notifications about the lifecycle and progress
/// of a document export operation.
/// </summary>
public interface IExportObserver
{
    /// <summary>
    /// Invoked when the export process starts for a document.
    /// </summary>
    /// <param name="document">The document being exported.</param>
    /// <param name="fileExtension">The target file extension of the exporter (e.g., ".md", ".html").</param>
    void OnExportStarted(Document document, string fileExtension);

    /// <summary>
    /// Invoked each time a top-level document element is processed during export.
    /// </summary>
    /// <param name="document">The document being exported.</param>
    /// <param name="element">The document element that was just processed.</param>
    /// <param name="currentIndex">The 1-based index of the processed element.</param>
    /// <param name="totalCount">The total number of top-level elements in the document.</param>
    void OnElementExported(Document document, IDocumentElement element, int currentIndex, int totalCount);

    /// <summary>
    /// Invoked when the export operation finishes and the final output has been generated.
    /// </summary>
    /// <param name="document">The exported document.</param>
    /// <param name="exportedContent">The complete exported string output.</param>
    void OnExportCompleted(Document document, string exportedContent);
}

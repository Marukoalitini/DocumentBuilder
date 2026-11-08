using System.Text;
using DocumentBuilder.Decorators;
using DocumentBuilder.Elements;
using DocumentBuilder.Observers;

namespace DocumentBuilder.Exporters;

/// <summary>
/// Defines the skeleton of the document export algorithm using the Template Method design pattern.
/// Concrete exporters inherit from this class to customize format-specific steps while preserving
/// the overall export lifecycle, file I/O handling, and observer notifications.
/// </summary>
public abstract class DocumentExporterTemplate : IDocumentExporter
{
    private readonly List<IExportObserver> _observers = [];

    /// <inheritdoc />
    public abstract string FileExtension { get; }

    /// <summary>
    /// Gets the text encoding used when writing the exported output to a file.
    /// Defaults to <see cref="Encoding.UTF8"/>.
    /// </summary>
    protected virtual Encoding OutputEncoding => Encoding.UTF8;

    /// <summary>
    /// Occurs when the document export operation begins.
    /// </summary>
    public event EventHandler<ExportProgressEventArgs>? ExportStarted;

    /// <summary>
    /// Occurs each time a top-level document element is processed during export.
    /// </summary>
    public event EventHandler<ExportProgressEventArgs>? ElementExported;

    /// <summary>
    /// Occurs when the document export operation finishes.
    /// </summary>
    public event EventHandler<ExportProgressEventArgs>? ExportCompleted;

    /// <summary>
    /// Attaches an <see cref="IExportObserver"/> to receive lifecycle notifications during export.
    /// </summary>
    /// <param name="observer">The observer instance to attach.</param>
    /// <returns>The current exporter instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="observer"/> is null.</exception>
    public DocumentExporterTemplate Attach(IExportObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
        return this;
    }

    /// <summary>
    /// Detaches a previously registered <see cref="IExportObserver"/>.
    /// </summary>
    /// <param name="observer">The observer instance to remove.</param>
    /// <returns><c>true</c> if the observer was removed; otherwise, <c>false</c>.</returns>
    public bool Detach(IExportObserver observer)
    {
        return _observers.Remove(observer);
    }

    /// <summary>
    /// Executes the template method defining the invariant steps of the document export algorithm:
    /// validation, header formatting, metadata formatting, body traversal, footer formatting, and observer notifications.
    /// </summary>
    /// <param name="document">The document to export.</param>
    /// <returns>The complete formatted string representation of the document.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
    public string Export(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);

        int totalElements = document.Elements.Count;
        NotifyExportStarted(document, totalElements);

        var sb = new StringBuilder();

        // Step 1: Format document header
        string header = FormatHeader(document);
        if (!string.IsNullOrEmpty(header))
        {
            sb.Append(header);
        }

        // Step 2: Format document metadata
        string metadata = FormatMetadata(document);
        if (!string.IsNullOrEmpty(metadata))
        {
            sb.Append(metadata);
        }

        // Step 3: Format document body elements and notify progress
        for (int i = 0; i < totalElements; i++)
        {
            IDocumentElement element = document.Elements[i];
            string formattedElement = FormatElement(element, currentHeadingLevel: 2);
            if (!string.IsNullOrEmpty(formattedElement))
            {
                sb.AppendLine(formattedElement.TrimEnd());
                sb.AppendLine();
            }

            NotifyElementExported(document, element, i + 1, totalElements);
        }

        // Step 4: Format document footer
        string footer = FormatFooter(document);
        if (!string.IsNullOrEmpty(footer))
        {
            sb.Append(footer);
        }

        string aggregatedOutput = sb.ToString().TrimEnd() + Environment.NewLine;
        string finalOutput = FinalizeExport(document, aggregatedOutput);
        NotifyExportCompleted(document, totalElements, finalOutput);

        return finalOutput;
    }

    /// <inheritdoc />
    public void ExportToFile(Document document, string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        string content = Export(document);
        EnsureDirectoryExists(filePath);
        File.WriteAllText(filePath, content, OutputEncoding);
    }

    /// <inheritdoc />
    public async Task ExportToFileAsync(Document document, string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        string content = Export(document);
        EnsureDirectoryExists(filePath);
        await File.WriteAllTextAsync(filePath, content, OutputEncoding, cancellationToken);
    }

    /// <summary>
    /// Dispatches a document element (including composite nodes and decorators) to its corresponding formatting step.
    /// </summary>
    /// <param name="element">The element to format.</param>
    /// <param name="currentHeadingLevel">The current hierarchical heading level.</param>
    /// <returns>The formatted string for the element.</returns>
    protected virtual string FormatElement(IDocumentElement element, int currentHeadingLevel)
    {
        return element switch
        {
            Title title => FormatTitle(title, currentHeadingLevel),
            Paragraph paragraph => FormatParagraph(paragraph, currentHeadingLevel),
            Image image => FormatImage(image, currentHeadingLevel),
            Table table => FormatTable(table, currentHeadingLevel),
            BulletList bulletList => FormatBulletList(bulletList, currentHeadingLevel),
            OrderedList orderedList => FormatOrderedList(orderedList, currentHeadingLevel),
            HorizontalSeparator separator => FormatHorizontalSeparator(separator, currentHeadingLevel),
            CodeBlock codeBlock => FormatCodeBlock(codeBlock, currentHeadingLevel),
            Blockquote blockquote => FormatBlockquote(blockquote, currentHeadingLevel),
            Hyperlink hyperlink => FormatHyperlink(hyperlink, currentHeadingLevel),
            PageBreak pageBreak => FormatPageBreak(pageBreak, currentHeadingLevel),
            ColumnLayout columnLayout => FormatColumnLayout(columnLayout, currentHeadingLevel),
            Section section => FormatSection(section, currentHeadingLevel),
            BoldDecorator bold => FormatBold(bold, FormatElement(bold.InnerElement, currentHeadingLevel).Trim(), currentHeadingLevel),
            ItalicDecorator italic => FormatItalic(italic, FormatElement(italic.InnerElement, currentHeadingLevel).Trim(), currentHeadingLevel),
            UnderlineDecorator underline => FormatUnderline(underline, FormatElement(underline.InnerElement, currentHeadingLevel).Trim(), currentHeadingLevel),
            StrikethroughDecorator strikethrough => FormatStrikethrough(strikethrough, FormatElement(strikethrough.InnerElement, currentHeadingLevel).Trim(), currentHeadingLevel),
            HighlightDecorator highlight => FormatHighlight(highlight, FormatElement(highlight.InnerElement, currentHeadingLevel).Trim(), currentHeadingLevel),
            AlignmentDecorator alignment => FormatAlignment(alignment, FormatElement(alignment.InnerElement, currentHeadingLevel).Trim(), currentHeadingLevel),
            BorderDecorator border => FormatBorder(border, FormatElement(border.InnerElement, currentHeadingLevel).Trim(), currentHeadingLevel),
            DocumentElementDecorator customDecorator => FormatCustomDecorator(customDecorator, FormatElement(customDecorator.InnerElement, currentHeadingLevel).Trim(), currentHeadingLevel),
            _ => FormatCustomElement(element, currentHeadingLevel)
        };
    }

    /// <summary>
    /// Formats the document header section (e.g., title banner or HTML document preamble).
    /// </summary>
    /// <param name="document">The document being exported.</param>
    /// <returns>The formatted header string.</returns>
    protected abstract string FormatHeader(Document document);

    /// <summary>
    /// Formats the document author, creation timestamp, and custom metadata block.
    /// </summary>
    /// <param name="document">The document being exported.</param>
    /// <returns>The formatted metadata string.</returns>
    protected abstract string FormatMetadata(Document document);

    /// <summary>
    /// Formats a <see cref="Title"/> leaf element.
    /// </summary>
    protected abstract string FormatTitle(Title title, int currentHeadingLevel);

    /// <summary>
    /// Formats a <see cref="Paragraph"/> leaf element.
    /// </summary>
    protected abstract string FormatParagraph(Paragraph paragraph, int currentHeadingLevel);

    /// <summary>
    /// Formats an <see cref="Image"/> leaf element.
    /// </summary>
    protected abstract string FormatImage(Image image, int currentHeadingLevel);

    /// <summary>
    /// Formats a <see cref="Table"/> leaf element.
    /// </summary>
    protected abstract string FormatTable(Table table, int currentHeadingLevel);

    /// <summary>
    /// Formats a <see cref="BulletList"/> leaf element.
    /// </summary>
    protected abstract string FormatBulletList(BulletList bulletList, int currentHeadingLevel);

    /// <summary>
    /// Formats an <see cref="OrderedList"/> leaf element.
    /// </summary>
    protected abstract string FormatOrderedList(OrderedList orderedList, int currentHeadingLevel);

    /// <summary>
    /// Formats a <see cref="HorizontalSeparator"/> leaf element.
    /// </summary>
    protected abstract string FormatHorizontalSeparator(HorizontalSeparator separator, int currentHeadingLevel);

    /// <summary>
    /// Formats a <see cref="CodeBlock"/> leaf element.
    /// </summary>
    protected virtual string FormatCodeBlock(CodeBlock codeBlock, int currentHeadingLevel)
    {
        return codeBlock.Render(Math.Max(0, currentHeadingLevel - 2));
    }

    /// <summary>
    /// Formats a <see cref="Blockquote"/> leaf element.
    /// </summary>
    protected virtual string FormatBlockquote(Blockquote blockquote, int currentHeadingLevel)
    {
        return blockquote.Render(Math.Max(0, currentHeadingLevel - 2));
    }

    /// <summary>
    /// Formats a <see cref="Hyperlink"/> leaf element.
    /// </summary>
    protected virtual string FormatHyperlink(Hyperlink hyperlink, int currentHeadingLevel)
    {
        return hyperlink.Render(Math.Max(0, currentHeadingLevel - 2));
    }

    /// <summary>
    /// Formats a <see cref="PageBreak"/> leaf element.
    /// </summary>
    protected virtual string FormatPageBreak(PageBreak pageBreak, int currentHeadingLevel)
    {
        return pageBreak.Render(Math.Max(0, currentHeadingLevel - 2));
    }

    /// <summary>
    /// Formats a <see cref="ColumnLayout"/> composite container and recursively formats its columns.
    /// </summary>
    protected virtual string FormatColumnLayout(ColumnLayout columnLayout, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < columnLayout.Elements.Count; i++)
        {
            sb.AppendLine(FormatElement(columnLayout.Elements[i], currentHeadingLevel).TrimEnd());
            if (i < columnLayout.Elements.Count - 1)
            {
                sb.AppendLine();
            }
        }
        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Formats a <see cref="Section"/> composite container and recursively formats its children.
    /// </summary>
    protected virtual string FormatSection(Section section, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        int clampedLevel = Math.Clamp(currentHeadingLevel, 1, 6);
        sb.AppendLine(FormatTitle(new Title(section.Title, clampedLevel), clampedLevel).TrimEnd());
        sb.AppendLine();

        foreach (IDocumentElement child in section.Elements)
        {
            string formattedChild = FormatElement(child, currentHeadingLevel + 1);
            if (!string.IsNullOrEmpty(formattedChild))
            {
                sb.AppendLine(formattedChild.TrimEnd());
                sb.AppendLine();
            }
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Formats a <see cref="BoldDecorator"/> wrapping an inner element.
    /// </summary>
    protected virtual string FormatBold(BoldDecorator bold, string innerContent, int currentHeadingLevel)
    {
        return $"**{innerContent}**";
    }

    /// <summary>
    /// Formats an <see cref="ItalicDecorator"/> wrapping an inner element.
    /// </summary>
    protected virtual string FormatItalic(ItalicDecorator italic, string innerContent, int currentHeadingLevel)
    {
        return $"*{innerContent}*";
    }

    /// <summary>
    /// Formats an <see cref="UnderlineDecorator"/> wrapping an inner element.
    /// </summary>
    protected virtual string FormatUnderline(UnderlineDecorator underline, string innerContent, int currentHeadingLevel)
    {
        return underline.FormatDecoratedContent(innerContent, Math.Max(0, currentHeadingLevel - 2));
    }

    /// <summary>
    /// Formats a <see cref="StrikethroughDecorator"/> wrapping an inner element.
    /// </summary>
    protected virtual string FormatStrikethrough(StrikethroughDecorator strikethrough, string innerContent, int currentHeadingLevel)
    {
        return strikethrough.FormatDecoratedContent(innerContent, Math.Max(0, currentHeadingLevel - 2));
    }

    /// <summary>
    /// Formats a <see cref="HighlightDecorator"/> wrapping an inner element.
    /// </summary>
    protected virtual string FormatHighlight(HighlightDecorator highlight, string innerContent, int currentHeadingLevel)
    {
        return highlight.FormatDecoratedContent(innerContent, Math.Max(0, currentHeadingLevel - 2));
    }

    /// <summary>
    /// Formats an <see cref="AlignmentDecorator"/> wrapping an inner element.
    /// </summary>
    protected virtual string FormatAlignment(AlignmentDecorator alignment, string innerContent, int currentHeadingLevel)
    {
        return alignment.FormatDecoratedContent(innerContent, Math.Max(0, currentHeadingLevel - 2));
    }

    /// <summary>
    /// Formats a <see cref="BorderDecorator"/> wrapping an inner element.
    /// </summary>
    protected virtual string FormatBorder(BorderDecorator border, string innerContent, int currentHeadingLevel)
    {
        return border.FormatDecoratedContent(innerContent, Math.Max(0, currentHeadingLevel - 2));
    }

    /// <summary>
    /// Hook method for formatting custom user-defined <see cref="DocumentElementDecorator"/> implementations.
    /// Recursively formats the inner element using the active exporter before applying the custom decoration.
    /// </summary>
    protected virtual string FormatCustomDecorator(DocumentElementDecorator decorator, string innerContent, int currentHeadingLevel)
    {
        return decorator.FormatDecoratedContent(innerContent, Math.Max(0, currentHeadingLevel - 2));
    }

    /// <summary>
    /// Hook method for formatting custom user-defined <see cref="IDocumentElement"/> implementations.
    /// Defaults to invoking the element's <see cref="IDocumentElement.Render"/> method.
    /// </summary>
    protected virtual string FormatCustomElement(IDocumentElement element, int currentHeadingLevel)
    {
        return element.Render(Math.Max(0, currentHeadingLevel - 2));
    }

    /// <summary>
    /// Hook method for formatting the document footer (e.g., closing tags in HTML).
    /// Returns an empty string by default.
    /// </summary>
    /// <param name="document">The document being exported.</param>
    /// <returns>The formatted footer string.</returns>
    protected virtual string FormatFooter(Document document)
    {
        return string.Empty;
    }

    /// <summary>
    /// Hook method invoked after all document sections have been formatted to perform final assembly
    /// or binary/container packaging (such as building a PDF cross-reference table).
    /// Returns <paramref name="aggregatedContent"/> unchanged by default.
    /// </summary>
    /// <param name="document">The document being exported.</param>
    /// <param name="aggregatedContent">The aggregated content produced by the formatting steps.</param>
    /// <returns>The finalized document output string.</returns>
    protected virtual string FinalizeExport(Document document, string aggregatedContent)
    {
        return aggregatedContent;
    }

    private void NotifyExportStarted(Document document, int totalElements)
    {
        foreach (IExportObserver observer in _observers.ToArray())
        {
            observer.OnExportStarted(document, FileExtension);
        }

        ExportStarted?.Invoke(this, new ExportProgressEventArgs(document.Title, "Started", 0, totalElements));
    }

    private void NotifyElementExported(Document document, IDocumentElement element, int currentIndex, int totalElements)
    {
        foreach (IExportObserver observer in _observers.ToArray())
        {
            observer.OnElementExported(document, element, currentIndex, totalElements);
        }

        ElementExported?.Invoke(this, new ExportProgressEventArgs(document.Title, "ElementProcessed", currentIndex, totalElements, element));
    }

    private void NotifyExportCompleted(Document document, int totalElements, string finalOutput)
    {
        foreach (IExportObserver observer in _observers.ToArray())
        {
            observer.OnExportCompleted(document, finalOutput);
        }

        ExportCompleted?.Invoke(this, new ExportProgressEventArgs(document.Title, "Completed", totalElements, totalElements));
    }

    private static void EnsureDirectoryExists(string filePath)
    {
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}

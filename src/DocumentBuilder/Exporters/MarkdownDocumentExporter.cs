using System.Text;
using DocumentBuilder.Elements;

namespace DocumentBuilder.Exporters;

/// <summary>
/// Provides default GitHub Flavored Markdown (.md) export capability for documents.
/// Formats document metadata, hierarchical sections, Markdown tables, and rich elements out-of-the-box.
/// </summary>
public sealed class MarkdownDocumentExporter : IDocumentExporter
{
    /// <inheritdoc />
    public string FileExtension => ".md";

    /// <inheritdoc />
    public string Export(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var sb = new StringBuilder();

        // Document header title
        sb.AppendLine($"# {document.Title}");
        sb.AppendLine();

        // Metadata block formatted as clean Markdown blockquote
        sb.AppendLine("> **Document Information**  ");
        if (!string.IsNullOrWhiteSpace(document.Author))
        {
            sb.AppendLine($"> **Author:** {document.Author}  ");
        }
        sb.AppendLine($"> **Generated:** {document.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC  ");

        if (document.Metadata.Count > 0)
        {
            foreach (var (key, value) in document.Metadata)
            {
                sb.AppendLine($"> **{key}:** {value}  ");
            }
        }

        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        // Export top-level elements
        foreach (IDocumentElement element in document.Elements)
        {
            RenderElement(sb, element, currentHeadingLevel: 2);
            sb.AppendLine();
        }

        return sb.ToString().TrimEnd() + Environment.NewLine;
    }

    /// <inheritdoc />
    public void ExportToFile(Document document, string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        string content = Export(document);
        EnsureDirectoryExists(filePath);
        File.WriteAllText(filePath, content, Encoding.UTF8);
    }

    /// <inheritdoc />
    public async Task ExportToFileAsync(Document document, string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        string content = Export(document);
        EnsureDirectoryExists(filePath);
        await File.WriteAllTextAsync(filePath, content, Encoding.UTF8, cancellationToken);
    }

    private static void RenderElement(StringBuilder sb, IDocumentElement element, int currentHeadingLevel)
    {
        switch (element)
        {
            case Title title:
                string prefix = new('#', Math.Clamp(title.Level, 1, 6));
                sb.AppendLine($"{prefix} {title.Text}");
                break;

            case Paragraph paragraph:
                sb.AppendLine(paragraph.Text);
                break;

            case Image image:
                string alt = string.IsNullOrWhiteSpace(image.AltText) ? image.Caption ?? "image" : image.AltText;
                string caption = string.IsNullOrWhiteSpace(image.Caption) ? string.Empty : $" \"{image.Caption}\"";
                sb.AppendLine($"![{alt}]({image.Source}{caption})");
                break;

            case Table table:
                RenderMarkdownTable(sb, table);
                break;

            case BulletList bulletList:
                foreach (string item in bulletList.Items)
                {
                    sb.AppendLine($"- {item}");
                }
                break;

            case OrderedList orderedList:
                for (int i = 0; i < orderedList.Items.Count; i++)
                {
                    int itemNumber = orderedList.StartNumber + i;
                    sb.AppendLine($"{itemNumber}. {orderedList.Items[i]}");
                }
                break;

            case HorizontalSeparator:
                sb.AppendLine("---");
                break;

            case Section section:
                int headingLevel = Math.Clamp(currentHeadingLevel, 1, 6);
                string sectionPrefix = new('#', headingLevel);
                sb.AppendLine($"{sectionPrefix} {section.Title}");
                sb.AppendLine();

                foreach (IDocumentElement child in section.Elements)
                {
                    RenderElement(sb, child, currentHeadingLevel + 1);
                    sb.AppendLine();
                }
                break;

            default:
                sb.AppendLine(element.Render());
                break;
        }
    }

    private static void RenderMarkdownTable(StringBuilder sb, Table table)
    {
        int columnCount = table.Headers.Count;
        foreach (var row in table.Rows)
        {
            if (row.Count > columnCount)
            {
                columnCount = row.Count;
            }
        }

        if (columnCount == 0)
        {
            return;
        }

        var headerParts = new List<string>();
        var separatorParts = new List<string>();

        for (int i = 0; i < columnCount; i++)
        {
            string headerText = i < table.Headers.Count ? table.Headers[i] : $"Col {i + 1}";
            headerParts.Add($" {headerText} ");
            separatorParts.Add(" :--- ");
        }

        sb.AppendLine($"|{string.Join("|", headerParts)}|");
        sb.AppendLine($"|{string.Join("|", separatorParts)}|");

        foreach (var row in table.Rows)
        {
            var rowParts = new List<string>();
            for (int i = 0; i < columnCount; i++)
            {
                string cellText = i < row.Count ? row[i] : string.Empty;
                rowParts.Add($" {cellText} ");
            }
            sb.AppendLine($"|{string.Join("|", rowParts)}|");
        }
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

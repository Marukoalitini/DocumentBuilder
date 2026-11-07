using System.Text;
using DocumentBuilder.Decorators;
using DocumentBuilder.Elements;

namespace DocumentBuilder.Exporters;

/// <summary>
/// Concrete export strategy that generates GitHub Flavored Markdown (.md) documents.
/// Inherits the export lifecycle skeleton from <see cref="DocumentExporterTemplate"/>.
/// </summary>
public sealed class MarkdownDocumentExporter : DocumentExporterTemplate
{
    /// <inheritdoc />
    public override string FileExtension => ".md";

    /// <inheritdoc />
    protected override string FormatHeader(Document document)
    {
        return $"# {document.Title}{Environment.NewLine}{Environment.NewLine}";
    }

    /// <inheritdoc />
    protected override string FormatMetadata(Document document)
    {
        var sb = new StringBuilder();
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
        return sb.ToString();
    }

    /// <inheritdoc />
    protected override string FormatTitle(Title title, int currentHeadingLevel)
    {
        string prefix = new('#', Math.Clamp(title.Level, 1, 6));
        return $"{prefix} {title.Text}";
    }

    /// <inheritdoc />
    protected override string FormatParagraph(Paragraph paragraph, int currentHeadingLevel)
    {
        return paragraph.Text;
    }

    /// <inheritdoc />
    protected override string FormatImage(Image image, int currentHeadingLevel)
    {
        string alt = string.IsNullOrWhiteSpace(image.AltText) ? image.Caption ?? "image" : image.AltText;
        string caption = string.IsNullOrWhiteSpace(image.Caption) ? string.Empty : $" \"{image.Caption}\"";
        return $"![{alt}]({image.Source}{caption})";
    }

    /// <inheritdoc />
    protected override string FormatTable(Table table, int currentHeadingLevel)
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
            return string.Empty;
        }

        var sb = new StringBuilder();
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

        return sb.ToString().TrimEnd();
    }

    /// <inheritdoc />
    protected override string FormatBulletList(BulletList bulletList, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        foreach (string item in bulletList.Items)
        {
            sb.AppendLine($"- {item}");
        }
        return sb.ToString().TrimEnd();
    }

    /// <inheritdoc />
    protected override string FormatOrderedList(OrderedList orderedList, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < orderedList.Items.Count; i++)
        {
            int itemNumber = orderedList.StartNumber + i;
            sb.AppendLine($"{itemNumber}. {orderedList.Items[i]}");
        }
        return sb.ToString().TrimEnd();
    }

    /// <inheritdoc />
    protected override string FormatHorizontalSeparator(HorizontalSeparator separator, int currentHeadingLevel)
    {
        return "---";
    }

    /// <inheritdoc />
    protected override string FormatCodeBlock(CodeBlock codeBlock, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        string lang = codeBlock.Language ?? string.Empty;
        sb.AppendLine($"```{lang}");
        sb.AppendLine(codeBlock.Code.TrimEnd());
        sb.Append("```");
        return sb.ToString();
    }

    /// <inheritdoc />
    protected override string FormatBlockquote(Blockquote blockquote, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        foreach (string line in blockquote.Text.Split('\n'))
        {
            sb.AppendLine($"> {line.TrimEnd('\r')}");
        }

        if (!string.IsNullOrWhiteSpace(blockquote.Citation))
        {
            sb.AppendLine($"> — *{blockquote.Citation}*");
        }

        return sb.ToString().TrimEnd();
    }

    /// <inheritdoc />
    protected override string FormatHyperlink(Hyperlink hyperlink, int currentHeadingLevel)
    {
        string tooltip = string.IsNullOrWhiteSpace(hyperlink.Tooltip) ? string.Empty : $" \"{hyperlink.Tooltip}\"";
        return $"[{hyperlink.Text}]({hyperlink.Url}{tooltip})";
    }

    /// <inheritdoc />
    protected override string FormatPageBreak(PageBreak pageBreak, int currentHeadingLevel)
    {
        return $"<div style=\"page-break-after: always;\"></div>{Environment.NewLine}{Environment.NewLine}---";
    }

    /// <inheritdoc />
    protected override string FormatAlignment(AlignmentDecorator alignment, string innerContent, int currentHeadingLevel)
    {
        string alignAttr = alignment.Alignment.ToString().ToLowerInvariant();
        return $"<div align=\"{alignAttr}\">{Environment.NewLine}{innerContent}{Environment.NewLine}</div>";
    }

    /// <inheritdoc />
    protected override string FormatBorder(BorderDecorator border, string innerContent, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(border.Label))
        {
            sb.AppendLine($"> **[{border.Label.ToUpperInvariant()}]**  ");
        }

        foreach (string line in innerContent.Split('\n'))
        {
            sb.AppendLine($"> {line.TrimEnd('\r')}");
        }

        return sb.ToString().TrimEnd();
    }
}

using System.Net;
using System.Text;
using DocumentBuilder.Decorators;
using DocumentBuilder.Elements;

namespace DocumentBuilder.Exporters;

/// <summary>
/// Concrete export strategy that generates standalone, styled HTML5 (.html) documents.
/// Inherits the export lifecycle skeleton from <see cref="DocumentExporterTemplate"/>.
/// </summary>
public class HtmlDocumentExporter : DocumentExporterTemplate
{
    /// <inheritdoc />
    public override string FileExtension => ".html";

    /// <inheritdoc />
    protected override string FormatHeader(Document document)
    {
        string escapedTitle = WebUtility.HtmlEncode(document.Title);
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("  <meta charset=\"utf-8\" />");
        sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />");
        sb.AppendLine($"  <title>{escapedTitle}</title>");
        sb.AppendLine("  <style>");
        sb.AppendLine("    body { font-family: system-ui, -apple-system, sans-serif; line-height: 1.6; max-width: 900px; margin: 2rem auto; padding: 0 1.5rem; color: #1f2937; }");
        sb.AppendLine("    h1, h2, h3, h4 { color: #111827; margin-top: 1.5rem; }");
        sb.AppendLine("    .metadata { background: #f3f4f6; border-left: 4px solid #3b82f6; padding: 1rem; margin-bottom: 2rem; border-radius: 0.25rem; }");
        sb.AppendLine("    table { width: 100%; border-collapse: collapse; margin: 1.25rem 0; }");
        sb.AppendLine("    th, td { border: 1px solid #d1d5db; padding: 0.6rem 0.8rem; text-align: left; }");
        sb.AppendLine("    th { background: #f9fafb; font-weight: 600; }");
        sb.AppendLine("    figure { margin: 1.5rem 0; text-align: center; }");
        sb.AppendLine("    img { max-width: 100%; height: auto; border-radius: 0.375rem; }");
        sb.AppendLine("    figcaption { font-size: 0.875rem; color: #6b7280; margin-top: 0.5rem; }");
        sb.AppendLine("    .bordered-box { border: 2px solid #4b5563; padding: 1rem; margin: 1rem 0; border-radius: 0.375rem; background: #fafafa; }");
        sb.AppendLine("    .bordered-label { font-weight: 700; text-transform: uppercase; font-size: 0.85rem; color: #374151; margin-bottom: 0.4rem; }");
        sb.AppendLine("    pre { background: #1e293b; color: #f8fafc; padding: 1rem; border-radius: 0.375rem; overflow-x: auto; font-family: ui-monospace, monospace; font-size: 0.9rem; }");
        sb.AppendLine("    blockquote { border-left: 4px solid #6b7280; margin: 1.25rem 0; padding: 0.5rem 1rem; background: #f9fafb; font-style: italic; }");
        sb.AppendLine("    .columns { display: grid; grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); gap: 1.25rem; margin: 1.25rem 0; }");
        sb.AppendLine("    .column { border: 1px solid #e5e7eb; padding: 0.85rem; border-radius: 0.375rem; }");
        sb.AppendLine("    .page-break { page-break-after: always; border-top: 2px dashed #d1d5db; margin: 2rem 0; }");
        sb.AppendLine("    hr { border: 0; border-top: 1px solid #e5e7eb; margin: 1.5rem 0; }");
        sb.AppendLine("  </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine($"  <h1>{escapedTitle}</h1>");
        return sb.ToString();
    }

    /// <inheritdoc />
    protected override string FormatMetadata(Document document)
    {
        var sb = new StringBuilder();
        sb.AppendLine("  <div class=\"metadata\">");
        if (!string.IsNullOrWhiteSpace(document.Author))
        {
            sb.AppendLine($"    <div><strong>Author:</strong> {WebUtility.HtmlEncode(document.Author)}</div>");
        }
        sb.AppendLine($"    <div><strong>Generated:</strong> {document.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC</div>");

        foreach (var (key, value) in document.Metadata)
        {
            sb.AppendLine($"    <div><strong>{WebUtility.HtmlEncode(key)}:</strong> {WebUtility.HtmlEncode(value)}</div>");
        }

        sb.AppendLine("  </div>");
        return sb.ToString();
    }

    /// <inheritdoc />
    protected override string FormatTitle(Title title, int currentHeadingLevel)
    {
        int level = Math.Clamp(title.Level, 1, 6);
        return $"  <h{level}>{WebUtility.HtmlEncode(title.Text)}</h{level}>";
    }

    /// <inheritdoc />
    protected override string FormatParagraph(Paragraph paragraph, int currentHeadingLevel)
    {
        return $"  <p>{WebUtility.HtmlEncode(paragraph.Text)}</p>";
    }

    /// <inheritdoc />
    protected override string FormatImage(Image image, int currentHeadingLevel)
    {
        string src = WebUtility.HtmlEncode(image.Source);
        string alt = WebUtility.HtmlEncode(image.AltText ?? image.Caption ?? "image");
        var sb = new StringBuilder();
        sb.AppendLine("  <figure>");
        sb.AppendLine($"    <img src=\"{src}\" alt=\"{alt}\" />");
        if (!string.IsNullOrWhiteSpace(image.Caption))
        {
            sb.AppendLine($"    <figcaption>{WebUtility.HtmlEncode(image.Caption)}</figcaption>");
        }
        sb.Append("  </figure>");
        return sb.ToString();
    }

    /// <inheritdoc />
    protected override string FormatTable(Table table, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        sb.AppendLine("  <table>");

        if (table.Headers.Count > 0)
        {
            sb.AppendLine("    <thead>");
            sb.AppendLine("      <tr>");
            foreach (string header in table.Headers)
            {
                sb.AppendLine($"        <th>{WebUtility.HtmlEncode(header)}</th>");
            }
            sb.AppendLine("      </tr>");
            sb.AppendLine("    </thead>");
        }

        sb.AppendLine("    <tbody>");
        foreach (var row in table.Rows)
        {
            sb.AppendLine("      <tr>");
            foreach (string cell in row)
            {
                sb.AppendLine($"        <td>{WebUtility.HtmlEncode(cell)}</td>");
            }
            sb.AppendLine("      </tr>");
        }
        sb.AppendLine("    </tbody>");
        sb.Append("  </table>");
        return sb.ToString();
    }

    /// <inheritdoc />
    protected override string FormatBulletList(BulletList bulletList, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        sb.AppendLine("  <ul>");
        foreach (string item in bulletList.Items)
        {
            sb.AppendLine($"    <li>{WebUtility.HtmlEncode(item)}</li>");
        }
        sb.Append("  </ul>");
        return sb.ToString();
    }

    /// <inheritdoc />
    protected override string FormatOrderedList(OrderedList orderedList, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"  <ol start=\"{orderedList.StartNumber}\">");
        foreach (string item in orderedList.Items)
        {
            sb.AppendLine($"    <li>{WebUtility.HtmlEncode(item)}</li>");
        }
        sb.Append("  </ol>");
        return sb.ToString();
    }

    /// <inheritdoc />
    protected override string FormatHorizontalSeparator(HorizontalSeparator separator, int currentHeadingLevel)
    {
        return "  <hr />";
    }

    /// <inheritdoc />
    protected override string FormatCodeBlock(CodeBlock codeBlock, int currentHeadingLevel)
    {
        string langAttr = string.IsNullOrWhiteSpace(codeBlock.Language)
            ? string.Empty
            : $" class=\"language-{WebUtility.HtmlEncode(codeBlock.Language)}\"";
        return $"  <pre><code{langAttr}>{WebUtility.HtmlEncode(codeBlock.Code.TrimEnd())}</code></pre>";
    }

    /// <inheritdoc />
    protected override string FormatBlockquote(Blockquote blockquote, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        sb.AppendLine("  <blockquote>");
        sb.AppendLine($"    <p>{WebUtility.HtmlEncode(blockquote.Text)}</p>");
        if (!string.IsNullOrWhiteSpace(blockquote.Citation))
        {
            sb.AppendLine($"    <footer>— <cite>{WebUtility.HtmlEncode(blockquote.Citation)}</cite></footer>");
        }
        sb.Append("  </blockquote>");
        return sb.ToString();
    }

    /// <inheritdoc />
    protected override string FormatHyperlink(Hyperlink hyperlink, int currentHeadingLevel)
    {
        string url = WebUtility.HtmlEncode(hyperlink.Url);
        string text = WebUtility.HtmlEncode(hyperlink.Text);
        string titleAttr = string.IsNullOrWhiteSpace(hyperlink.Tooltip)
            ? string.Empty
            : $" title=\"{WebUtility.HtmlEncode(hyperlink.Tooltip)}\"";
        return $"  <p><a href=\"{url}\"{titleAttr}>{text}</a></p>";
    }

    /// <inheritdoc />
    protected override string FormatPageBreak(PageBreak pageBreak, int currentHeadingLevel)
    {
        return "  <div class=\"page-break\"></div>";
    }

    /// <inheritdoc />
    protected override string FormatColumnLayout(ColumnLayout columnLayout, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        sb.AppendLine("  <div class=\"columns\">");
        foreach (IDocumentElement col in columnLayout.Elements)
        {
            sb.AppendLine("    <div class=\"column\">");
            sb.AppendLine($"      {FormatElement(col, currentHeadingLevel).Trim()}");
            sb.AppendLine("    </div>");
        }
        sb.Append("  </div>");
        return sb.ToString();
    }

    /// <inheritdoc />
    protected override string FormatBold(BoldDecorator bold, string innerContent, int currentHeadingLevel)
    {
        if (innerContent.StartsWith("<p>", StringComparison.Ordinal) && innerContent.EndsWith("</p>", StringComparison.Ordinal))
        {
            return $"  <p><strong>{innerContent[3..^4]}</strong></p>";
        }
        return $"  <strong>{innerContent}</strong>";
    }

    /// <inheritdoc />
    protected override string FormatItalic(ItalicDecorator italic, string innerContent, int currentHeadingLevel)
    {
        if (innerContent.StartsWith("<p>", StringComparison.Ordinal) && innerContent.EndsWith("</p>", StringComparison.Ordinal))
        {
            return $"  <p><em>{innerContent[3..^4]}</em></p>";
        }
        return $"  <em>{innerContent}</em>";
    }

    /// <inheritdoc />
    protected override string FormatUnderline(UnderlineDecorator underline, string innerContent, int currentHeadingLevel)
    {
        if (innerContent.StartsWith("<p>", StringComparison.Ordinal) && innerContent.EndsWith("</p>", StringComparison.Ordinal))
        {
            return $"  <p><u>{innerContent[3..^4]}</u></p>";
        }
        return $"  <u>{innerContent}</u>";
    }

    /// <inheritdoc />
    protected override string FormatStrikethrough(StrikethroughDecorator strikethrough, string innerContent, int currentHeadingLevel)
    {
        if (innerContent.StartsWith("<p>", StringComparison.Ordinal) && innerContent.EndsWith("</p>", StringComparison.Ordinal))
        {
            return $"  <p><del>{innerContent[3..^4]}</del></p>";
        }
        return $"  <del>{innerContent}</del>";
    }

    /// <inheritdoc />
    protected override string FormatHighlight(HighlightDecorator highlight, string innerContent, int currentHeadingLevel)
    {
        string color = WebUtility.HtmlEncode(highlight.Color);
        if (innerContent.StartsWith("<p>", StringComparison.Ordinal) && innerContent.EndsWith("</p>", StringComparison.Ordinal))
        {
            return $"  <p><mark style=\"background-color: {color};\">{innerContent[3..^4]}</mark></p>";
        }
        return $"  <mark style=\"background-color: {color};\">{innerContent}</mark>";
    }

    /// <inheritdoc />
    protected override string FormatAlignment(AlignmentDecorator alignment, string innerContent, int currentHeadingLevel)
    {
        string align = alignment.Alignment.ToString().ToLowerInvariant();
        return $"  <div style=\"text-align: {align};\">{innerContent}</div>";
    }

    /// <inheritdoc />
    protected override string FormatBorder(BorderDecorator border, string innerContent, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        sb.AppendLine("  <div class=\"bordered-box\">");
        if (!string.IsNullOrWhiteSpace(border.Label))
        {
            sb.AppendLine($"    <div class=\"bordered-label\">{WebUtility.HtmlEncode(border.Label)}</div>");
        }
        sb.AppendLine($"    {innerContent}");
        sb.Append("  </div>");
        return sb.ToString();
    }

    /// <inheritdoc />
    protected override string FormatFooter(Document document)
    {
        return $"</body>{Environment.NewLine}</html>";
    }
}

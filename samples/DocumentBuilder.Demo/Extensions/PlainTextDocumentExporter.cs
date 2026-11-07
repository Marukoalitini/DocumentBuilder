using System.Text;
using DocumentBuilder.Decorators;
using DocumentBuilder.Elements;
using DocumentBuilder.Exporters;

namespace DocumentBuilder.Demo.Extensions;

/// <summary>
/// Custom export strategy implemented in the client application by inheriting from <see cref="DocumentExporterTemplate"/>.
/// Demonstrates Open/Closed Principle (OCP) extensibility by adding a new output format (.txt)
/// without modifying the core library source code.
/// </summary>
public sealed class PlainTextDocumentExporter : DocumentExporterTemplate
{
    private const int BannerWidth = 64;

    public override string FileExtension => ".txt";

    protected override string FormatHeader(Document document)
    {
        string border = new('=', BannerWidth);
        var sb = new StringBuilder();
        sb.AppendLine(border);
        sb.AppendLine($"  {document.Title.ToUpperInvariant()}");
        sb.AppendLine(border);
        return sb.ToString();
    }

    protected override string FormatMetadata(Document document)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(document.Author))
        {
            sb.AppendLine($"Author    : {document.Author}");
        }
        sb.AppendLine($"Generated : {document.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC");

        foreach (var (key, value) in document.Metadata)
        {
            sb.AppendLine($"{key,-10}: {value}");
        }

        sb.AppendLine(new string('-', BannerWidth));
        sb.AppendLine();
        return sb.ToString();
    }

    protected override string FormatTitle(Title title, int currentHeadingLevel)
    {
        char underlineChar = title.Level == 1 ? '=' : '-';
        string underline = new(underlineChar, title.Text.Length);
        return $"{title.Text.ToUpperInvariant()}{Environment.NewLine}{underline}";
    }

    protected override string FormatParagraph(Paragraph paragraph, int currentHeadingLevel)
    {
        return paragraph.Render(Math.Max(0, currentHeadingLevel - 2));
    }

    protected override string FormatImage(Image image, int currentHeadingLevel)
    {
        return image.Render(Math.Max(0, currentHeadingLevel - 2));
    }

    protected override string FormatTable(Table table, int currentHeadingLevel)
    {
        return table.Render(Math.Max(0, currentHeadingLevel - 2));
    }

    protected override string FormatBulletList(BulletList bulletList, int currentHeadingLevel)
    {
        return bulletList.Render(Math.Max(0, currentHeadingLevel - 2));
    }

    protected override string FormatOrderedList(OrderedList orderedList, int currentHeadingLevel)
    {
        return orderedList.Render(Math.Max(0, currentHeadingLevel - 2));
    }

    protected override string FormatHorizontalSeparator(HorizontalSeparator separator, int currentHeadingLevel)
    {
        return separator.Render(Math.Max(0, currentHeadingLevel - 2));
    }

    protected override string FormatBold(BoldDecorator bold, string innerContent, int currentHeadingLevel)
    {
        string indent = new(' ', Math.Max(0, currentHeadingLevel - 2) * 2);
        return $"{indent}[BOLD] {innerContent}";
    }

    protected override string FormatItalic(ItalicDecorator italic, string innerContent, int currentHeadingLevel)
    {
        string indent = new(' ', Math.Max(0, currentHeadingLevel - 2) * 2);
        return $"{indent}[ITALIC] {innerContent}";
    }

    protected override string FormatFooter(Document document)
    {
        string border = new('=', BannerWidth);
        return $"{border}{Environment.NewLine}  END OF REPORT{Environment.NewLine}{border}";
    }
}

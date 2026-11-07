using System.Globalization;
using System.Text;
using DocumentBuilder.Decorators;
using DocumentBuilder.Elements;

namespace DocumentBuilder.Exporters;

/// <summary>
/// Concrete export strategy that generates a standalone, multi-page PDF 1.4 document (.pdf)
/// using native PostScript Type1 fonts and vector graphics without external dependencies.
/// Participates in both the Strategy (<see cref="IDocumentExporter"/>) and
/// Template Method (<see cref="DocumentExporterTemplate"/>) patterns.
/// </summary>
public sealed class PdfDocumentExporter : DocumentExporterTemplate
{
    private const double PageWidth = 595.0;   // A4 width in points
    private const double PageHeight = 842.0;  // A4 height in points
    private const double MarginLeft = 50.0;
    private const double MarginRight = 50.0;
    private const double MarginTop = 55.0;
    private const double MarginBottom = 55.0;
    private const double ContentWidth = PageWidth - MarginLeft - MarginRight;

    /// <inheritdoc />
    public override string FileExtension => ".pdf";

    /// <inheritdoc />
    protected override Encoding OutputEncoding => Encoding.Latin1;

    /// <inheritdoc />
    protected override string FormatHeader(Document document)
    {
        return $"@@DOC_TITLE|{SanitizeInline(document.Title)}{Environment.NewLine}";
    }

    /// <inheritdoc />
    protected override string FormatMetadata(Document document)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(document.Author))
        {
            sb.AppendLine($"@@META|Author|{SanitizeInline(document.Author)}");
        }

        sb.AppendLine($"@@META|Generated|{document.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC");

        foreach (var (key, value) in document.Metadata)
        {
            sb.AppendLine($"@@META|{SanitizeInline(key)}|{SanitizeInline(value)}");
        }

        sb.AppendLine("@@META_END");
        return sb.ToString();
    }

    /// <inheritdoc />
    protected override string FormatTitle(Title title, int currentHeadingLevel)
    {
        int effectiveLevel = Math.Clamp(Math.Max(title.Level, currentHeadingLevel), 1, 6);
        return $"@@HEADING|{effectiveLevel}|{SanitizeInline(title.Text)}";
    }

    /// <inheritdoc />
    protected override string FormatParagraph(Paragraph paragraph, int currentHeadingLevel)
    {
        return $"@@PARAGRAPH|F1|10.5|LEFT|NONE|{SanitizeInline(paragraph.Text)}";
    }

    /// <inheritdoc />
    protected override string FormatImage(Image image, int currentHeadingLevel)
    {
        string caption = (string.IsNullOrWhiteSpace(image.Caption) ? image.AltText : image.Caption) ?? "Image";
        return $"@@IMAGE|{SanitizeInline(image.Source)}|{SanitizeInline(caption)}";
    }

    /// <inheritdoc />
    protected override string FormatTable(Table table, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        sb.Append("@@TABLE_START|");
        sb.AppendLine(string.Join("\t", table.Headers.Select(SanitizeInline)));

        foreach (IReadOnlyList<string> row in table.Rows)
        {
            sb.Append("@@TABLE_ROW|");
            sb.AppendLine(string.Join("\t", row.Select(SanitizeInline)));
        }

        sb.Append("@@TABLE_END");
        return sb.ToString();
    }

    /// <inheritdoc />
    protected override string FormatBulletList(BulletList bulletList, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        foreach (string item in bulletList.Items)
        {
            sb.AppendLine($"@@LIST_ITEM|-|{SanitizeInline(item)}");
        }
        return sb.ToString().TrimEnd();
    }

    /// <inheritdoc />
    protected override string FormatOrderedList(OrderedList orderedList, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < orderedList.Items.Count; i++)
        {
            int number = orderedList.StartNumber + i;
            sb.AppendLine($"@@LIST_ITEM|{number}.|{SanitizeInline(orderedList.Items[i])}");
        }
        return sb.ToString().TrimEnd();
    }

    /// <inheritdoc />
    protected override string FormatHorizontalSeparator(HorizontalSeparator separator, int currentHeadingLevel)
    {
        return "@@RULE";
    }

    /// <inheritdoc />
    protected override string FormatCodeBlock(CodeBlock codeBlock, int currentHeadingLevel)
    {
        var sb = new StringBuilder();
        string lang = string.IsNullOrWhiteSpace(codeBlock.Language) ? "code" : SanitizeInline(codeBlock.Language);
        sb.AppendLine($"@@CODE_START|{lang}");

        string[] lines = codeBlock.Code.Replace("\r\n", "\n").Split('\n');
        foreach (string line in lines)
        {
            sb.AppendLine($"@@CODE_LINE|{SanitizeInline(line)}");
        }

        sb.Append("@@CODE_END");
        return sb.ToString();
    }

    /// <inheritdoc />
    protected override string FormatBlockquote(Blockquote blockquote, int currentHeadingLevel)
    {
        string citation = blockquote.Citation is not null ? SanitizeInline(blockquote.Citation) : string.Empty;
        return $"@@BLOCKQUOTE|{SanitizeInline(blockquote.Text)}|{citation}";
    }

    /// <inheritdoc />
    protected override string FormatHyperlink(Hyperlink hyperlink, int currentHeadingLevel)
    {
        return $"@@LINK|{SanitizeInline(hyperlink.Text)}|{SanitizeInline(hyperlink.Url)}";
    }

    /// <inheritdoc />
    protected override string FormatPageBreak(PageBreak pageBreak, int currentHeadingLevel)
    {
        return "@@PAGE_BREAK";
    }

    /// <inheritdoc />
    protected override string FormatColumnLayout(ColumnLayout columnLayout, int currentHeadingLevel)
    {
        if (columnLayout.Elements.Count == 0)
        {
            return string.Empty;
        }

        var columnsText = new List<string>();
        foreach (IDocumentElement col in columnLayout.Elements)
        {
            string formatted = FormatElement(col, currentHeadingLevel).Trim();
            columnsText.Add(ExtractPlainTextFromDirective(formatted));
        }

        return $"@@COLUMNS|{string.Join("\t", columnsText)}";
    }

    /// <inheritdoc />
    protected override string FormatBold(BoldDecorator bold, string innerContent, int currentHeadingLevel)
    {
        return ApplyParagraphStyle(innerContent, font: "F2");
    }

    /// <inheritdoc />
    protected override string FormatItalic(ItalicDecorator italic, string innerContent, int currentHeadingLevel)
    {
        return ApplyParagraphStyle(innerContent, font: "F3");
    }

    /// <inheritdoc />
    protected override string FormatUnderline(UnderlineDecorator underline, string innerContent, int currentHeadingLevel)
    {
        return ApplyParagraphStyle(innerContent, decoration: "UNDERLINE");
    }

    /// <inheritdoc />
    protected override string FormatStrikethrough(StrikethroughDecorator strikethrough, string innerContent, int currentHeadingLevel)
    {
        return ApplyParagraphStyle(innerContent, decoration: "STRIKE");
    }

    /// <inheritdoc />
    protected override string FormatHighlight(HighlightDecorator highlight, string innerContent, int currentHeadingLevel)
    {
        return ApplyParagraphStyle(innerContent, decoration: "HIGHLIGHT");
    }

    /// <inheritdoc />
    protected override string FormatAlignment(AlignmentDecorator alignment, string innerContent, int currentHeadingLevel)
    {
        string align = alignment.Alignment.ToString().ToUpperInvariant();
        return ApplyParagraphStyle(innerContent, align: align);
    }

    /// <inheritdoc />
    protected override string FormatBorder(BorderDecorator border, string innerContent, int currentHeadingLevel)
    {
        string label = border.Label is not null ? SanitizeInline(border.Label) : "Notice";
        string text = ExtractPlainTextFromDirective(innerContent);
        return $"@@BOX|{label}|{text}";
    }

    /// <inheritdoc />
    protected override string FinalizeExport(Document document, string aggregatedContent)
    {
        var renderer = new PdfDocumentRenderer(SanitizeInline(document.Title));
        renderer.RenderDirectives(aggregatedContent);
        return renderer.BuildPdfDocument();
    }

    private static string ApplyParagraphStyle(
        string directive,
        string? font = null,
        string? align = null,
        string? decoration = null)
    {
        if (directive.StartsWith("@@PARAGRAPH|", StringComparison.Ordinal))
        {
            string[] parts = directive.Split('|', 6);
            if (parts.Length == 6)
            {
                string currentFont = font ?? parts[1];
                string currentSize = parts[2];
                string currentAlign = align ?? parts[3];
                string currentDecoration = decoration ?? parts[4];
                string text = parts[5];
                return $"@@PARAGRAPH|{currentFont}|{currentSize}|{currentAlign}|{currentDecoration}|{text}";
            }
        }

        string plain = ExtractPlainTextFromDirective(directive);
        return $"@@PARAGRAPH|{font ?? "F1"}|10.5|{align ?? "LEFT"}|{decoration ?? "NONE"}|{plain}";
    }

    private static string ExtractPlainTextFromDirective(string directive)
    {
        if (directive.StartsWith("@@PARAGRAPH|", StringComparison.Ordinal))
        {
            string[] parts = directive.Split('|', 6);
            if (parts.Length == 6)
            {
                return parts[5];
            }
        }

        if (directive.StartsWith("@@BOX|", StringComparison.Ordinal))
        {
            string[] parts = directive.Split('|', 3);
            if (parts.Length == 3)
            {
                return $"[{parts[1]}] {parts[2]}";
            }
        }

        return SanitizeInline(directive);
    }

    private static string SanitizeInline(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(input.Length);
        foreach (char c in input)
        {
            switch (c)
            {
                case '\r':
                case '\n':
                    sb.Append(' ');
                    break;
                case '\u2014': // em-dash
                case '\u2013': // en-dash
                    sb.Append('-');
                    break;
                case '\u2018':
                case '\u2019':
                    sb.Append('\'');
                    break;
                case '\u201C':
                case '\u201D':
                    sb.Append('"');
                    break;
                case '\u2022': // bullet
                    sb.Append('*');
                    break;
                default:
                    sb.Append(c <= 255 ? c : '?');
                    break;
            }
        }

        return sb.ToString();
    }

    private sealed class PdfDocumentRenderer
    {
        private readonly string _documentTitle;
        private readonly List<StringBuilder> _pages = [];
        private StringBuilder _currentStream = new();
        private double _cursorY;

        public PdfDocumentRenderer(string documentTitle)
        {
            _documentTitle = documentTitle;
            StartNewPage();
        }

        public void RenderDirectives(string content)
        {
            string[] lines = content.Replace("\r\n", "\n").Split('\n');
            int index = 0;

            while (index < lines.Length)
            {
                string line = lines[index].Trim();
                if (string.IsNullOrEmpty(line))
                {
                    index++;
                    continue;
                }

                if (line.StartsWith("@@DOC_TITLE|", StringComparison.Ordinal))
                {
                    string title = line["@@DOC_TITLE|".Length..];
                    DrawDocumentHeader(title);
                    index++;
                }
                else if (line.StartsWith("@@META|", StringComparison.Ordinal))
                {
                    var metaItems = new List<(string Key, string Value)>();
                    while (index < lines.Length && lines[index].Trim().StartsWith("@@META|", StringComparison.Ordinal))
                    {
                        string[] parts = lines[index].Trim().Split('|', 3);
                        if (parts.Length == 3)
                        {
                            metaItems.Add((parts[1], parts[2]));
                        }
                        index++;
                    }
                    if (index < lines.Length && lines[index].Trim() == "@@META_END")
                    {
                        index++;
                    }
                    DrawMetadataBox(metaItems);
                }
                else if (line.StartsWith("@@HEADING|", StringComparison.Ordinal))
                {
                    string[] parts = line.Split('|', 3);
                    int level = int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int l) ? l : 2;
                    DrawHeading(parts[2], level);
                    index++;
                }
                else if (line.StartsWith("@@PARAGRAPH|", StringComparison.Ordinal))
                {
                    string[] parts = line.Split('|', 6);
                    if (parts.Length == 6)
                    {
                        double fontSize = double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double fs) ? fs : 10.5;
                        DrawParagraph(parts[5], parts[1], fontSize, parts[3], parts[4]);
                    }
                    index++;
                }
                else if (line.StartsWith("@@IMAGE|", StringComparison.Ordinal))
                {
                    string[] parts = line.Split('|', 3);
                    DrawImagePlaceholder(parts[1], parts.Length > 2 ? parts[2] : string.Empty);
                    index++;
                }
                else if (line.StartsWith("@@TABLE_START|", StringComparison.Ordinal))
                {
                    string[] headers = line["@@TABLE_START|".Length..].Split('\t');
                    var rows = new List<string[]>();
                    index++;
                    while (index < lines.Length && lines[index].Trim().StartsWith("@@TABLE_ROW|", StringComparison.Ordinal))
                    {
                        rows.Add(lines[index].Trim()["@@TABLE_ROW|".Length..].Split('\t'));
                        index++;
                    }
                    if (index < lines.Length && lines[index].Trim() == "@@TABLE_END")
                    {
                        index++;
                    }
                    DrawTable(headers, rows);
                }
                else if (line.StartsWith("@@LIST_ITEM|", StringComparison.Ordinal))
                {
                    string[] parts = line.Split('|', 3);
                    DrawListItem(parts[1], parts[2]);
                    index++;
                }
                else if (line == "@@RULE")
                {
                    DrawHorizontalRule();
                    index++;
                }
                else if (line.StartsWith("@@CODE_START|", StringComparison.Ordinal))
                {
                    string lang = line["@@CODE_START|".Length..];
                    var codeLines = new List<string>();
                    index++;
                    while (index < lines.Length && lines[index].Trim().StartsWith("@@CODE_LINE|", StringComparison.Ordinal))
                    {
                        codeLines.Add(lines[index].Trim()["@@CODE_LINE|".Length..]);
                        index++;
                    }
                    if (index < lines.Length && lines[index].Trim() == "@@CODE_END")
                    {
                        index++;
                    }
                    DrawCodeBlock(lang, codeLines);
                }
                else if (line.StartsWith("@@BLOCKQUOTE|", StringComparison.Ordinal))
                {
                    string[] parts = line.Split('|', 3);
                    DrawBlockquote(parts[1], parts.Length > 2 ? parts[2] : string.Empty);
                    index++;
                }
                else if (line.StartsWith("@@LINK|", StringComparison.Ordinal))
                {
                    string[] parts = line.Split('|', 3);
                    DrawHyperlink(parts[1], parts.Length > 2 ? parts[2] : string.Empty);
                    index++;
                }
                else if (line == "@@PAGE_BREAK")
                {
                    StartNewPage();
                    index++;
                }
                else if (line.StartsWith("@@COLUMNS|", StringComparison.Ordinal))
                {
                    string[] cols = line["@@COLUMNS|".Length..].Split('\t');
                    DrawColumns(cols);
                    index++;
                }
                else if (line.StartsWith("@@BOX|", StringComparison.Ordinal))
                {
                    string[] parts = line.Split('|', 3);
                    DrawBox(parts[1], parts[2]);
                    index++;
                }
                else
                {
                    DrawParagraph(line, "F1", 10.5, "LEFT", "NONE");
                    index++;
                }
            }
        }

        public string BuildPdfDocument()
        {
            int totalPages = _pages.Count;
            for (int i = 0; i < totalPages; i++)
            {
                AppendPageFooter(_pages[i], i + 1, totalPages);
            }

            // Object layout:
            // 1: Catalog
            // 2: Pages
            // 3: Font F1 (Helvetica)
            // 4: Font F2 (Helvetica-Bold)
            // 5: Font F3 (Helvetica-Oblique)
            // 6: Font F4 (Courier)
            // 7 .. 6 + totalPages*2: Page & Content Stream pairs
            var objects = new List<string>
            {
                "<< /Type /Catalog /Pages 2 0 R >>",
                BuildPagesDictionary(totalPages),
                "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>",
                "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>",
                "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Oblique /Encoding /WinAnsiEncoding >>",
                "<< /Type /Font /Subtype /Type1 /BaseFont /Courier /Encoding /WinAnsiEncoding >>"
            };

            for (int i = 0; i < totalPages; i++)
            {
                int pageObjNum = 7 + (i * 2);
                int contentObjNum = pageObjNum + 1;
                string streamData = _pages[i].ToString();

                string pageDict =
                    $"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {Fmt(PageWidth)} {Fmt(PageHeight)}] " +
                    $"/Resources << /Font << /F1 3 0 R /F2 4 0 R /F3 5 0 R /F4 6 0 R >> >> " +
                    $"/Contents {contentObjNum} 0 R >>";

                string contentObj =
                    $"<< /Length {streamData.Length} >>\nstream\n{streamData}endstream";

                objects.Add(pageDict);
                objects.Add(contentObj);
            }

            var pdf = new StringBuilder();
            pdf.Append("%PDF-1.4\n");

            var offsets = new List<int> { 0 }; // Object 0 is free
            for (int i = 0; i < objects.Count; i++)
            {
                offsets.Add(pdf.Length);
                int objNumber = i + 1;
                pdf.Append($"{objNumber} 0 obj\n{objects[i]}\nendobj\n");
            }

            int xrefOffset = pdf.Length;
            int totalObjects = objects.Count + 1;
            pdf.Append($"xref\n0 {totalObjects}\n");
            pdf.Append("0000000000 65535 f \n");

            for (int i = 1; i < totalObjects; i++)
            {
                pdf.Append($"{offsets[i]:D10} 00000 n \n");
            }

            pdf.Append($"trailer\n<< /Size {totalObjects} /Root 1 0 R >>\n");
            pdf.Append($"startxref\n{xrefOffset}\n%%EOF\n");

            return pdf.ToString();
        }

        private static string BuildPagesDictionary(int totalPages)
        {
            var kids = new StringBuilder();
            for (int i = 0; i < totalPages; i++)
            {
                if (i > 0)
                {
                    kids.Append(' ');
                }
                kids.Append($"{7 + (i * 2)} 0 R");
            }
            return $"<< /Type /Pages /Kids [{kids}] /Count {totalPages} >>";
        }

        private void StartNewPage()
        {
            _currentStream = new StringBuilder();
            _pages.Add(_currentStream);
            _cursorY = PageHeight - MarginTop;
        }

        private void EnsureVerticalSpace(double requiredHeight)
        {
            if (_cursorY - requiredHeight < MarginBottom)
            {
                StartNewPage();
            }
        }

        private void DrawDocumentHeader(string title)
        {
            List<string> wrapped = WrapText(title, maxCharsPerLine: 46);
            double height = (wrapped.Count * 22.0) + 14.0;
            EnsureVerticalSpace(height);

            SetFillRgb(0.06, 0.09, 0.16);
            foreach (string line in wrapped)
            {
                DrawTextLine(line, MarginLeft, _cursorY, "F2", 18.0);
                _cursorY -= 22.0;
            }

            // Blue accent bar under document title
            SetStrokeRgb(0.15, 0.39, 0.92);
            _currentStream.Append($"2 w {Fmt(MarginLeft)} {Fmt(_cursorY + 6)} m {Fmt(PageWidth - MarginRight)} {Fmt(_cursorY + 6)} l S\n");
            _cursorY -= 12.0;
        }

        private void DrawMetadataBox(List<(string Key, string Value)> metaItems)
        {
            if (metaItems.Count == 0)
            {
                return;
            }

            double lineHeight = 13.5;
            double boxHeight = (metaItems.Count * lineHeight) + 14.0;
            EnsureVerticalSpace(boxHeight + 10.0);

            double boxTop = _cursorY + 8.0;
            double boxBottom = boxTop - boxHeight;

            // Shaded metadata box
            SetFillRgb(0.97, 0.98, 0.99);
            SetStrokeRgb(0.82, 0.86, 0.90);
            _currentStream.Append($"0.6 w {Fmt(MarginLeft)} {Fmt(boxBottom)} {Fmt(ContentWidth)} {Fmt(boxHeight)} re B\n");

            double textY = _cursorY - 4.0;
            foreach (var (key, value) in metaItems)
            {
                SetFillRgb(0.20, 0.25, 0.33);
                DrawTextLine($"{key}:", MarginLeft + 10.0, textY, "F2", 9.0);
                SetFillRgb(0.30, 0.35, 0.43);
                DrawTextLine(value, MarginLeft + 95.0, textY, "F1", 9.0);
                textY -= lineHeight;
            }

            _cursorY = boxBottom - 16.0;
        }

        private void DrawHeading(string text, int level)
        {
            double fontSize = level switch
            {
                1 => 15.0,
                2 => 13.0,
                3 => 11.5,
                _ => 10.5
            };

            int maxChars = (int)(ContentWidth / (fontSize * 0.52));
            List<string> lines = WrapText(text, maxChars);
            double lineHeight = fontSize * 1.35;
            EnsureVerticalSpace((lines.Count * lineHeight) + 18.0);

            _cursorY -= 6.0;
            SetFillRgb(0.12, 0.16, 0.23);

            foreach (string line in lines)
            {
                DrawTextLine(line, MarginLeft, _cursorY, "F2", fontSize);
                _cursorY -= lineHeight;
            }

            if (level <= 2)
            {
                SetStrokeRgb(0.86, 0.89, 0.93);
                _currentStream.Append($"0.7 w {Fmt(MarginLeft)} {Fmt(_cursorY + 6)} m {Fmt(PageWidth - MarginRight)} {Fmt(_cursorY + 6)} l S\n");
                _cursorY -= 6.0;
            }
        }

        private void DrawParagraph(string text, string font, double fontSize, string align, string decoration)
        {
            int maxChars = (int)(ContentWidth / (fontSize * 0.50));
            List<string> lines = WrapText(text, maxChars);
            double lineHeight = fontSize * 1.42;
            EnsureVerticalSpace((lines.Count * lineHeight) + 8.0);

            foreach (string line in lines)
            {
                double estimatedWidth = line.Length * fontSize * 0.48;
                double x = align switch
                {
                    "CENTER" => MarginLeft + Math.Max(0, (ContentWidth - estimatedWidth) / 2.0),
                    "RIGHT" => Math.Max(MarginLeft, PageWidth - MarginRight - estimatedWidth),
                    _ => MarginLeft
                };

                if (decoration == "HIGHLIGHT")
                {
                    SetFillRgb(0.99, 0.94, 0.54); // #fef08a yellow highlight
                    _currentStream.Append($"{Fmt(x - 2)} {Fmt(_cursorY - 3)} {Fmt(Math.Min(ContentWidth, estimatedWidth + 6))} {Fmt(fontSize + 3)} re f\n");
                }

                SetFillRgb(0.12, 0.16, 0.23);
                DrawTextLine(line, x, _cursorY, font, fontSize);

                if (decoration == "UNDERLINE")
                {
                    SetStrokeRgb(0.12, 0.16, 0.23);
                    _currentStream.Append($"0.6 w {Fmt(x)} {Fmt(_cursorY - 2)} m {Fmt(x + estimatedWidth)} {Fmt(_cursorY - 2)} l S\n");
                }
                else if (decoration == "STRIKE")
                {
                    SetStrokeRgb(0.12, 0.16, 0.23);
                    double strikeY = _cursorY + (fontSize * 0.30);
                    _currentStream.Append($"0.6 w {Fmt(x)} {Fmt(strikeY)} m {Fmt(x + estimatedWidth)} {Fmt(strikeY)} l S\n");
                }

                _cursorY -= lineHeight;
            }

            _cursorY -= 4.0;
        }

        private void DrawListItem(string marker, string itemText)
        {
            int maxChars = 82;
            List<string> lines = WrapText(itemText, maxChars);
            double lineHeight = 14.5;
            EnsureVerticalSpace((lines.Count * lineHeight) + 4.0);

            SetFillRgb(0.15, 0.39, 0.92);
            DrawTextLine(marker, MarginLeft + 8.0, _cursorY, "F2", 10.0);

            SetFillRgb(0.12, 0.16, 0.23);
            for (int i = 0; i < lines.Count; i++)
            {
                DrawTextLine(lines[i], MarginLeft + 26.0, _cursorY, "F1", 10.0);
                _cursorY -= lineHeight;
            }
        }

        private void DrawHorizontalRule()
        {
            EnsureVerticalSpace(18.0);
            _cursorY -= 4.0;
            SetStrokeRgb(0.80, 0.84, 0.88);
            _currentStream.Append($"0.8 w {Fmt(MarginLeft)} {Fmt(_cursorY)} m {Fmt(PageWidth - MarginRight)} {Fmt(_cursorY)} l S\n");
            _cursorY -= 12.0;
        }

        private void DrawCodeBlock(string language, List<string> codeLines)
        {
            double lineHeight = 12.5;
            double boxHeight = ((codeLines.Count + 1) * lineHeight) + 14.0;
            EnsureVerticalSpace(boxHeight + 10.0);

            double boxTop = _cursorY + 6.0;
            double boxBottom = boxTop - boxHeight;

            SetFillRgb(0.95, 0.96, 0.98);
            SetStrokeRgb(0.79, 0.83, 0.88);
            _currentStream.Append($"0.6 w {Fmt(MarginLeft)} {Fmt(boxBottom)} {Fmt(ContentWidth)} {Fmt(boxHeight)} re B\n");

            double textY = _cursorY - 5.0;
            SetFillRgb(0.39, 0.45, 0.55);
            DrawTextLine($"[{language.ToUpperInvariant()}]", MarginLeft + 10.0, textY, "F2", 8.0);
            textY -= lineHeight;

            SetFillRgb(0.06, 0.09, 0.16);
            foreach (string codeLine in codeLines)
            {
                string clipped = codeLine.Length > 84 ? codeLine[..84] + "..." : codeLine;
                DrawTextLine(clipped, MarginLeft + 10.0, textY, "F4", 8.8);
                textY -= lineHeight;
            }

            _cursorY = boxBottom - 14.0;
        }

        private void DrawBlockquote(string quoteText, string citation)
        {
            List<string> lines = WrapText($"\"{quoteText}\"", 80);
            double lineHeight = 14.0;
            int totalLines = lines.Count + (string.IsNullOrWhiteSpace(citation) ? 0 : 1);
            double blockHeight = (totalLines * lineHeight) + 10.0;
            EnsureVerticalSpace(blockHeight + 8.0);

            double barTop = _cursorY + 8.0;
            double barBottom = barTop - blockHeight;

            // Blue vertical quote bar
            SetFillRgb(0.15, 0.39, 0.92);
            _currentStream.Append($"{Fmt(MarginLeft + 4.0)} {Fmt(barBottom)} 3 {Fmt(blockHeight)} re f\n");

            SetFillRgb(0.28, 0.33, 0.41);
            foreach (string line in lines)
            {
                DrawTextLine(line, MarginLeft + 16.0, _cursorY, "F3", 10.0);
                _cursorY -= lineHeight;
            }

            if (!string.IsNullOrWhiteSpace(citation))
            {
                SetFillRgb(0.39, 0.45, 0.55);
                DrawTextLine($"- {citation}", MarginLeft + 16.0, _cursorY, "F2", 9.0);
                _cursorY -= lineHeight;
            }

            _cursorY -= 6.0;
        }

        private void DrawHyperlink(string text, string url)
        {
            EnsureVerticalSpace(20.0);
            SetFillRgb(0.15, 0.39, 0.92);
            string display = $"{text} ({url})";
            List<string> lines = WrapText(display, 85);
            foreach (string line in lines)
            {
                DrawTextLine(line, MarginLeft, _cursorY, "F2", 9.5);
                _cursorY -= 14.0;
            }
            _cursorY -= 4.0;
        }

        private void DrawImagePlaceholder(string source, string caption)
        {
            double boxHeight = 36.0;
            EnsureVerticalSpace(boxHeight + 12.0);

            double boxTop = _cursorY + 6.0;
            double boxBottom = boxTop - boxHeight;

            SetFillRgb(0.97, 0.98, 1.00);
            SetStrokeRgb(0.75, 0.82, 0.95);
            _currentStream.Append($"0.6 w {Fmt(MarginLeft)} {Fmt(boxBottom)} {Fmt(ContentWidth)} {Fmt(boxHeight)} re B\n");

            SetFillRgb(0.15, 0.39, 0.92);
            DrawTextLine($"[Image] {caption}", MarginLeft + 10.0, _cursorY - 5.0, "F2", 9.5);
            SetFillRgb(0.39, 0.45, 0.55);
            DrawTextLine($"Source: {source}", MarginLeft + 10.0, _cursorY - 18.0, "F3", 8.5);

            _cursorY = boxBottom - 14.0;
        }

        private void DrawColumns(string[] columns)
        {
            if (columns.Length == 0)
            {
                return;
            }

            int colCount = columns.Length;
            double gap = 14.0;
            double colWidth = (ContentWidth - (gap * (colCount - 1))) / colCount;
            int maxCharsPerCol = Math.Max(15, (int)(colWidth / 5.2));

            var wrappedCols = columns.Select(c => WrapText(c, maxCharsPerCol)).ToList();
            int maxLines = wrappedCols.Max(c => c.Count);
            double lineHeight = 13.5;
            double totalHeight = (maxLines * lineHeight) + 16.0;
            EnsureVerticalSpace(totalHeight + 8.0);

            double startY = _cursorY;
            for (int c = 0; c < colCount; c++)
            {
                double colX = MarginLeft + (c * (colWidth + gap));
                double boxTop = startY + 8.0;
                double boxBottom = boxTop - totalHeight;

                SetFillRgb(0.98, 0.98, 0.99);
                SetStrokeRgb(0.86, 0.89, 0.93);
                _currentStream.Append($"0.5 w {Fmt(colX)} {Fmt(boxBottom)} {Fmt(colWidth)} {Fmt(totalHeight)} re B\n");

                SetFillRgb(0.12, 0.16, 0.23);
                double lineY = startY - 5.0;
                foreach (string line in wrappedCols[c])
                {
                    DrawTextLine(line, colX + 8.0, lineY, "F1", 9.2);
                    lineY -= lineHeight;
                }
            }

            _cursorY = startY - totalHeight - 10.0;
        }

        private void DrawBox(string label, string text)
        {
            List<string> lines = WrapText(text, 82);
            double lineHeight = 13.5;
            double boxHeight = ((lines.Count + 1) * lineHeight) + 14.0;
            EnsureVerticalSpace(boxHeight + 10.0);

            double boxTop = _cursorY + 6.0;
            double boxBottom = boxTop - boxHeight;

            SetFillRgb(0.96, 0.97, 0.99);
            SetStrokeRgb(0.15, 0.39, 0.92);
            _currentStream.Append($"0.8 w {Fmt(MarginLeft)} {Fmt(boxBottom)} {Fmt(ContentWidth)} {Fmt(boxHeight)} re B\n");

            double textY = _cursorY - 5.0;
            SetFillRgb(0.15, 0.39, 0.92);
            DrawTextLine($"[{label.ToUpperInvariant()}]", MarginLeft + 10.0, textY, "F2", 9.0);
            textY -= lineHeight;

            SetFillRgb(0.12, 0.16, 0.23);
            foreach (string line in lines)
            {
                DrawTextLine(line, MarginLeft + 10.0, textY, "F1", 9.5);
                textY -= lineHeight;
            }

            _cursorY = boxBottom - 14.0;
        }

        private void DrawTable(string[] headers, List<string[]> rows)
        {
            int colCount = Math.Max(1, headers.Length);
            double colWidth = ContentWidth / colCount;
            double rowHeight = 18.0;
            double totalHeight = (rows.Count + 1) * rowHeight;
            EnsureVerticalSpace(totalHeight + 14.0);

            int maxCellChars = Math.Max(8, (int)(colWidth / 5.4));
            double tableTop = _cursorY + 6.0;

            // Header row background
            SetFillRgb(0.92, 0.94, 0.97);
            SetStrokeRgb(0.75, 0.80, 0.86);
            _currentStream.Append($"0.6 w {Fmt(MarginLeft)} {Fmt(tableTop - rowHeight)} {Fmt(ContentWidth)} {Fmt(rowHeight)} re B\n");

            SetFillRgb(0.06, 0.09, 0.16);
            for (int c = 0; c < colCount; c++)
            {
                double cellX = MarginLeft + (c * colWidth);
                string cellText = TruncateCell(headers[c], maxCellChars);
                DrawTextLine(cellText, cellX + 6.0, tableTop - 12.5, "F2", 8.8);
            }

            double currentRowTop = tableTop - rowHeight;
            for (int r = 0; r < rows.Count; r++)
            {
                double rowBottom = currentRowTop - rowHeight;
                if (r % 2 == 1)
                {
                    SetFillRgb(0.98, 0.99, 1.00);
                    _currentStream.Append($"{Fmt(MarginLeft)} {Fmt(rowBottom)} {Fmt(ContentWidth)} {Fmt(rowHeight)} re f\n");
                }

                SetStrokeRgb(0.82, 0.86, 0.90);
                _currentStream.Append($"0.5 w {Fmt(MarginLeft)} {Fmt(rowBottom)} {Fmt(ContentWidth)} {Fmt(rowHeight)} re S\n");

                SetFillRgb(0.15, 0.20, 0.28);
                string[] row = rows[r];
                for (int c = 0; c < Math.Min(colCount, row.Length); c++)
                {
                    double cellX = MarginLeft + (c * colWidth);
                    string cellText = TruncateCell(row[c], maxCellChars);
                    DrawTextLine(cellText, cellX + 6.0, currentRowTop - 12.5, "F1", 8.8);
                }

                currentRowTop = rowBottom;
            }

            _cursorY = currentRowTop - 16.0;
        }

        private void AppendPageFooter(StringBuilder pageStream, int pageNumber, int totalPages)
        {
            SetStrokeRgb(pageStream, 0.86, 0.89, 0.93);
            pageStream.Append($"0.5 w {Fmt(MarginLeft)} 42 m {Fmt(PageWidth - MarginRight)} 42 l S\n");

            SetFillRgb(pageStream, 0.45, 0.50, 0.58);
            DrawTextLine(pageStream, _documentTitle, MarginLeft, 30.0, "F1", 8.0);

            string pageLabel = $"Page {pageNumber} of {totalPages}";
            DrawTextLine(pageStream, pageLabel, PageWidth - MarginRight - 55.0, 30.0, "F2", 8.0);
        }

        private void DrawTextLine(string text, double x, double y, string fontName, double fontSize)
        {
            DrawTextLine(_currentStream, text, x, y, fontName, fontSize);
        }

        private static void DrawTextLine(StringBuilder stream, string text, double x, double y, string fontName, double fontSize)
        {
            stream.Append($"BT /{fontName} {Fmt(fontSize)} Tf {Fmt(x)} {Fmt(y)} Td ({EscapePdfString(text)}) Tj ET\n");
        }

        private void SetFillRgb(double r, double g, double b)
        {
            SetFillRgb(_currentStream, r, g, b);
        }

        private static void SetFillRgb(StringBuilder stream, double r, double g, double b)
        {
            stream.Append($"{Fmt(r)} {Fmt(g)} {Fmt(b)} rg\n");
        }

        private void SetStrokeRgb(double r, double g, double b)
        {
            SetStrokeRgb(_currentStream, r, g, b);
        }

        private static void SetStrokeRgb(StringBuilder stream, double r, double g, double b)
        {
            stream.Append($"{Fmt(r)} {Fmt(g)} {Fmt(b)} RG\n");
        }

        private static string EscapePdfString(string text)
        {
            var sb = new StringBuilder(text.Length + 8);
            foreach (char c in text)
            {
                if (c is '(' or ')' or '\\')
                {
                    sb.Append('\\');
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        private static List<string> WrapText(string text, int maxCharsPerLine)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(text))
            {
                result.Add(string.Empty);
                return result;
            }

            string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var currentLine = new StringBuilder();

            foreach (string word in words)
            {
                if (currentLine.Length == 0)
                {
                    currentLine.Append(word);
                }
                else if (currentLine.Length + 1 + word.Length <= maxCharsPerLine)
                {
                    currentLine.Append(' ').Append(word);
                }
                else
                {
                    result.Add(currentLine.ToString());
                    currentLine.Clear();
                    currentLine.Append(word);
                }
            }

            if (currentLine.Length > 0)
            {
                result.Add(currentLine.ToString());
            }

            return result;
        }

        private static string TruncateCell(string text, int maxChars)
        {
            if (text.Length <= maxChars)
            {
                return text;
            }
            return text[..Math.Max(1, maxChars - 2)] + "..";
        }

        private static string Fmt(double value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }
    }
}

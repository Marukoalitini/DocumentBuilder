using DocumentBuilder.Decorators;
using DocumentBuilder.Elements;
using DocumentBuilder.Exporters;
using DocBuilder = DocumentBuilder.Builders.DocumentBuilder;

namespace DocumentBuilder.Tests;

public class StrategyAndTemplateTests
{
    private sealed class BadgeElement(string text) : IDocumentElement
    {
        public string Text { get; } = text;

        public string Render(int indentationLevel = 0) => $"[{Text}]";
    }

    private sealed class BadgeHtmlExporter : HtmlDocumentExporter
    {
        protected override string FormatCustomElement(IDocumentElement element, int currentHeadingLevel)
        {
            return element is BadgeElement badge
                ? $"<span class=\"badge\">{badge.Text}</span>"
                : base.FormatCustomElement(element, currentHeadingLevel);
        }
    }

    [Fact]
    public void BuiltInExporter_CanBeInherited_ToFormatCustomElementsNatively()
    {
        // Arrange
        Document doc = new DocBuilder("Badge").AddElement(new BadgeElement("New")).Build();

        // Act
        string html = new BadgeHtmlExporter().Export(doc);

        // Assert
        Assert.Contains("<span class=\"badge\">New</span>", html);
    }

    [Fact]
    public void HtmlDocumentExporter_Export_ProducesValidStyledHtml5()
    {
        // Arrange
        Document doc = new DocBuilder("System Report", "Ops Team")
            .WithMetadata("Region", "us-east-1")
            .AddTitle("Section Heading", 2)
            .AddBoldParagraph("Bold statement")
            .AddItalicParagraph("Italic note")
            .AddBorderedParagraph("Boxed content", "Notice")
            .AddTable(["Host", "Status"], [["srv-01", "Online"]])
            .AddBulletList("Item 1", "Item 2")
            .AddOrderedList("Step 1", "Step 2")
            .AddHorizontalSeparator()
            .Build();

        var exporter = new HtmlDocumentExporter();

        // Act
        string html = exporter.Export(doc);

        // Assert
        Assert.Contains("<!DOCTYPE html>", html);
        Assert.Contains("<h1>System Report</h1>", html);
        Assert.Contains("<strong>Author:</strong> Ops Team", html);
        Assert.Contains("<h2>Section Heading</h2>", html);
        Assert.Contains("<p><strong>Bold statement</strong></p>", html);
        Assert.Contains("<p><em>Italic note</em></p>", html);
        Assert.Contains("class=\"bordered-box\"", html);
        Assert.Contains("<th>Host</th>", html);
        Assert.Contains("<td>srv-01</td>", html);
        Assert.Contains("<ul>", html);
        Assert.Contains("<ol start=\"1\">", html);
        Assert.Contains("<hr />", html);
        Assert.Contains("</html>", html);
    }

    [Fact]
    public void DocumentExportContext_SetStrategy_SwitchesFormatAtRuntime()
    {
        // Arrange
        Document doc = new DocBuilder("Strategy Test")
            .AddParagraph("Sample body.")
            .Build();

        var context = new DocumentExportContext(new MarkdownDocumentExporter());

        // Act
        string markdownOutput = context.Export(doc);
        context.SetStrategy(new HtmlDocumentExporter());
        string htmlOutput = context.Export(doc);

        // Assert
        Assert.Contains("# Strategy Test", markdownOutput);
        Assert.Contains("<!DOCTYPE html>", htmlOutput);
        Assert.Contains("<h1>Strategy Test</h1>", htmlOutput);
    }

    [Fact]
    public void MarkdownAndHtmlExporters_ExportRichElementsAndDecorators_ProduceExpectedMarkup()
    {
        // Arrange
        Document doc = new DocBuilder("Rich Export Test")
            .AddCodeBlock("var x = 42;", "csharp")
            .AddBlockquote("Always code as if the guy who ends up maintaining your code will be a violent psychopath.", "John Woods")
            .AddHyperlink("GitHub", "https://github.com", "Code Hosting")
            .AddColumns(new Paragraph("Left"), new Paragraph("Right"))
            .AddPageBreak()
            .AddElement(new Paragraph("Underlined").AsUnderlined())
            .AddElement(new Paragraph("Obsolete").AsStrikethrough())
            .AddElement(new Paragraph("Highlighted").WithHighlight("#fef08a").WithAlignment(TextAlignment.Center))
            .Build();

        var mdExporter = new MarkdownDocumentExporter();
        var htmlExporter = new HtmlDocumentExporter();

        // Act
        string md = mdExporter.Export(doc);
        string html = htmlExporter.Export(doc);

        // Assert Markdown
        Assert.Contains("```csharp", md);
        Assert.Contains("> Always code as if", md);
        Assert.Contains("[GitHub](https://github.com \"Code Hosting\")", md);
        Assert.Contains("page-break-after: always;", md);
        Assert.Contains("~~Obsolete~~", md);

        // Assert HTML
        Assert.Contains("<pre><code class=\"language-csharp\">var x = 42;</code></pre>", html);
        Assert.Contains("<blockquote>", html);
        Assert.Contains("<footer>— <cite>John Woods</cite></footer>", html);
        Assert.Contains("<a href=\"https://github.com\" title=\"Code Hosting\">GitHub</a>", html);
        Assert.Contains("class=\"columns\"", html);
        Assert.Contains("class=\"page-break\"", html);
        Assert.Contains("<p><u>Underlined</u></p>", html);
        Assert.Contains("<p><del>Obsolete</del></p>", html);
        Assert.Contains("text-align: center;", html);
        Assert.Contains("<mark style=\"background-color: #fef08a;", html);
    }

    [Fact]
    public void PdfDocumentExporter_Export_ProducesValidMultiPagePdf14Structure()
    {
        // Arrange
        Document doc = new DocBuilder("PDF Audit Report", "Security Team")
            .WithMetadata("Classification", "Confidential")
            .AddTitle("Executive Overview", 1)
            .AddBoldParagraph("All systems operational.")
            .AddTable(["Service", "Latency"], [["Auth API", "4.2ms"]])
            .AddCodeBlock("dotnet test --configuration Release", "bash")
            .AddBlockquote("Reliability is our primary feature.", "VP of Engineering")
            .AddPageBreak()
            .AddSection("Second Page Section", s => s
                .AddBulletList("Check A", "Check B")
                .AddHorizontalSeparator()
                .AddHyperlink("Status Page", "https://status.example.com")
            )
            .Build();

        var pdfExporter = new PdfDocumentExporter();

        // Act
        string pdfContent = pdfExporter.Export(doc);

        // Assert
        Assert.Equal(".pdf", pdfExporter.FileExtension);
        Assert.StartsWith("%PDF-1.4\n", pdfContent);
        Assert.Contains("/Type /Catalog", pdfContent);
        Assert.Contains("/Type /Pages /Kids [7 0 R 9 0 R] /Count 2", pdfContent);
        Assert.Contains("(PDF Audit Report) Tj", pdfContent);
        Assert.Contains("(All systems operational.) Tj", pdfContent);
        Assert.Contains("(Page 1 of 2) Tj", pdfContent);
        Assert.Contains("(Page 2 of 2) Tj", pdfContent);
        Assert.Contains("\nxref\n", pdfContent);
        Assert.EndsWith("%%EOF\n", pdfContent);
    }
}

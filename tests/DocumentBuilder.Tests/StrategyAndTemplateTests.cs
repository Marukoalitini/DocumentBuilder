using DocumentBuilder.Elements;
using DocumentBuilder.Exporters;
using DocBuilder = DocumentBuilder.Builders.DocumentBuilder;

namespace DocumentBuilder.Tests;

public class StrategyAndTemplateTests
{
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
        Assert.Contains("<strong><p>Bold statement</p></strong>", html);
        Assert.Contains("<em><p>Italic note</p></em>", html);
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
}

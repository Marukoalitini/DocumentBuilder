using DocumentBuilder.Decorators;
using DocumentBuilder.Elements;
using DocumentBuilder.Exporters;
using DocBuilder = DocumentBuilder.Builders.DocumentBuilder;

namespace DocumentBuilder.Tests;

public class DecoratorTests
{
    private sealed class CustomHighlightDecorator(IDocumentElement innerElement)
        : DocumentElementDecorator(innerElement)
    {
        public override string FormatDecoratedContent(string innerContent, int indentationLevel = 0)
        {
            string indent = new(' ', Math.Max(0, indentationLevel) * 2);
            return $"{indent}=={innerContent}==";
        }
    }
    [Fact]
    public void BoldDecorator_ShouldWrapElementContentInDoubleAsterisks()
    {
        // Arrange
        IDocumentElement element = new Paragraph("Important Notice").AsBold();

        // Act
        string rendered = element.Render();

        // Assert
        Assert.Equal("**Important Notice**", rendered);
    }

    [Fact]
    public void ItalicDecorator_ShouldWrapElementContentInSingleAsterisks()
    {
        // Arrange
        IDocumentElement element = new Paragraph("Subtle Note").AsItalic();

        // Act
        string rendered = element.Render();

        // Assert
        Assert.Equal("*Subtle Note*", rendered);
    }

    [Fact]
    public void BorderDecorator_ShouldFrameContentWithLabelAndBorderCharacter()
    {
        // Arrange
        IDocumentElement element = new Paragraph("System warning message.").WithBorder("Warning", '=');

        // Act
        string rendered = element.Render();

        // Assert
        Assert.Contains("[WARNING]", rendered);
        Assert.Contains("System warning message.", rendered);
        Assert.Contains("====", rendered);
    }

    [Fact]
    public void Decorators_CanBeNestedDynamically()
    {
        // Arrange
        IDocumentElement decorated = new Paragraph("Critical alert").AsBold().AsItalic().WithBorder("Alert");

        // Act
        string rendered = decorated.Render();

        // Assert
        Assert.Contains("[ALERT]", rendered);
        Assert.Contains("***Critical alert***", rendered);
    }

    [Fact]
    public void DocumentBuilder_DecoratorHelperMethods_AddDecoratedParagraphs()
    {
        // Act
        Document doc = new DocBuilder("Decorated Doc")
            .AddBoldParagraph("Bold line")
            .AddItalicParagraph("Italic line")
            .AddBorderedParagraph("Bordered line", "Box")
            .Build();

        // Assert
        Assert.Equal(3, doc.Elements.Count);
        Assert.IsType<BoldDecorator>(doc.Elements[0]);
        Assert.IsType<ItalicDecorator>(doc.Elements[1]);
        Assert.IsType<BorderDecorator>(doc.Elements[2]);
    }

    [Fact]
    public void CustomUserDecorator_ShouldIntegrateWithExportersRecursively()
    {
        // Arrange
        Document doc = new DocBuilder("Custom Decorator Test")
            .AddElement(new CustomHighlightDecorator(new Paragraph("Highlighted text").AsBold()))
            .Build();

        var htmlExporter = new HtmlDocumentExporter();

        // Act
        string htmlOutput = htmlExporter.Export(doc);

        // Assert
        Assert.Contains("==<p><strong>Highlighted text</strong></p>==", htmlOutput);
    }
}

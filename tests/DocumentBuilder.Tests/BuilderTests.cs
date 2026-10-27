using DocumentBuilder.Builders;
using DocumentBuilder.Elements;
using DocBuilder = DocumentBuilder.Builders.DocumentBuilder;

namespace DocumentBuilder.Tests;

public class BuilderTests
{
    [Fact]
    public void DocumentBuilder_DefaultValues_ProducesValidDocument()
    {
        // Act
        Document doc = new DocBuilder().Build();

        // Assert
        Assert.NotNull(doc);
        Assert.Equal("Untitled Document", doc.Title);
        Assert.Null(doc.Author);
        Assert.Empty(doc.Elements);
    }

    [Fact]
    public void DocumentBuilder_FluentChain_BuildsFullDocumentHierarchy()
    {
        // Act
        Document doc = new DocBuilder("API Spec", "Engineering Team")
            .WithMetadata("Environment", "Production")
            .AddTitle("Getting Started", 1)
            .AddParagraph("Introductory notes.")
            .AddImage("https://example.com/spec.png", "System Spec")
            .AddTable(["Endpoint", "Method"], [["/users", "GET"], ["/users", "POST"]])
            .AddSection("Authentication", s => s
                .AddParagraph("Use Bearer tokens.")
                .AddSubSection("Token Expiration", sub => sub
                    .AddParagraph("Tokens expire in 3600 seconds.")
                )
            )
            .Build();

        // Assert
        Assert.Equal("API Spec", doc.Title);
        Assert.Equal("Engineering Team", doc.Author);
        Assert.Equal("Production", doc.Metadata["Environment"]);
        Assert.Equal(5, doc.Elements.Count); // Title, Paragraph, Image, Table, Section

        var section = Assert.IsType<Section>(doc.Elements[4]);
        Assert.Equal("Authentication", section.Title);
        Assert.Equal(2, section.Elements.Count); // Paragraph, SubSection

        var subSection = Assert.IsType<Section>(section.Elements[1]);
        Assert.Equal("Token Expiration", subSection.Title);
        Assert.Single(subSection.Elements);
    }

    [Fact]
    public void DocumentBuilder_AddElement_SupportsCustomElementExtensibility()
    {
        // Arrange
        var customElement = new Paragraph("Custom Element Integration");

        // Act
        Document doc = new DocBuilder()
            .AddElement(customElement)
            .Build();

        // Assert
        Assert.Single(doc.Elements);
        Assert.Same(customElement, doc.Elements[0]);
    }

    [Fact]
    public void DocumentBuilder_AddListsAndSeparator_BuildsCorrectElements()
    {
        // Act
        Document doc = new DocBuilder("List Test")
            .AddBulletList("Item A", "Item B")
            .AddHorizontalSeparator()
            .AddOrderedList(["Step 1", "Step 2"], startNumber: 10)
            .Build();

        // Assert
        Assert.Equal(3, doc.Elements.Count);
        var bulletList = Assert.IsType<BulletList>(doc.Elements[0]);
        Assert.Equal(2, bulletList.Items.Count);

        var separator = Assert.IsType<HorizontalSeparator>(doc.Elements[1]);
        Assert.Equal('-', separator.Character);

        var orderedList = Assert.IsType<OrderedList>(doc.Elements[2]);
        Assert.Equal(10, orderedList.StartNumber);
        Assert.Equal(2, orderedList.Items.Count);
    }
}

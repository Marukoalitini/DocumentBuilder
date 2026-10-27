using DocumentBuilder.Elements;

namespace DocumentBuilder.Tests;

public class CompositeTests
{
    [Fact]
    public void Paragraph_ShouldRenderTextWithIndentation()
    {
        // Arrange
        var paragraph = new Paragraph("Hello World");

        // Act
        string renderedLevel0 = paragraph.Render(0);
        string renderedLevel1 = paragraph.Render(1);

        // Assert
        Assert.Equal("Hello World", renderedLevel0);
        Assert.Equal("  Hello World", renderedLevel1);
    }

    [Fact]
    public void Paragraph_NullText_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Paragraph(null!));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Title_ShouldStoreLevelAndRenderText(int level)
    {
        // Arrange
        var title = new Title($"Header {level}", level);

        // Act & Assert
        Assert.Equal(level, title.Level);
        Assert.Equal($"Header {level}", title.Render());
        Assert.Equal($"  Header {level}", title.Render(1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)]
    [InlineData(-1)]
    public void Title_InvalidLevel_ThrowsArgumentOutOfRangeException(int invalidLevel)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Title("Invalid", invalidLevel));
    }

    [Fact]
    public void Image_ShouldRenderWithCaptionAndAlt()
    {
        // Arrange
        var image = new Image("https://example.com/logo.png", caption: "Logo Caption", altText: "Company Logo");

        // Act
        string rendered = image.Render();

        // Assert
        Assert.Contains("https://example.com/logo.png", rendered);
        Assert.Contains("Logo Caption", rendered);
        Assert.Contains("Company Logo", rendered);
    }

    [Fact]
    public void Table_ShouldRenderAlignedAsciiGrid()
    {
        // Arrange
        var table = new Table(["Col 1", "Col 2"]);
        table.AddRow("Value A", "100");
        table.AddRow("Value B Long", "200");

        // Act
        string rendered = table.Render();

        // Assert
        Assert.Contains("Col 1", rendered);
        Assert.Contains("Col 2", rendered);
        Assert.Contains("Value A", rendered);
        Assert.Contains("Value B Long", rendered);
        Assert.Contains("+", rendered);
    }

    [Fact]
    public void Section_ShouldContainChildrenAndRenderRecursively()
    {
        // Arrange
        var section = new Section("Introduction");
        var p1 = new Paragraph("First point.");
        var p2 = new Paragraph("Second point.");

        // Act
        section.Add(p1).Add(p2);

        // Assert
        Assert.Equal(2, section.Elements.Count);
        string rendered = section.Render();

        Assert.Contains("Introduction", rendered);
        Assert.Contains("First point.", rendered);
        Assert.Contains("Second point.", rendered);
    }

    [Fact]
    public void Section_Remove_ShouldRemoveChildElement()
    {
        // Arrange
        var section = new Section("Main");
        var paragraph = new Paragraph("To be removed");
        section.Add(paragraph);

        // Act
        bool removed = section.Remove(paragraph);

        // Assert
        Assert.True(removed);
        Assert.Empty(section.Elements);
    }

    [Fact]
    public void Document_RootComposite_ShouldStoreMetadataAndElements()
    {
        // Arrange
        var doc = new Document("My Document", "Author Name");
        doc.SetMetadata("Version", "1.0");
        var section = new Section("Section 1");
        section.Add(new Paragraph("Content inside section"));
        doc.Add(section);

        // Act
        string rendered = doc.Render();

        // Assert
        Assert.Equal("My Document", doc.Title);
        Assert.Equal("Author Name", doc.Author);
        Assert.True(doc.Metadata.ContainsKey("Version"));
        Assert.Single(doc.Elements);
        Assert.Contains("Author: Author Name", rendered);
        Assert.Contains("Version: 1.0", rendered);
        Assert.Contains("Content inside section", rendered);
    }

    [Fact]
    public void BulletList_ShouldRenderItemsWithBulletMarkerAndIndentation()
    {
        // Arrange
        var list = new BulletList(["First", "Second"], bulletMarker: "*");

        // Act
        string renderedLevel0 = list.Render(0);
        string renderedLevel1 = list.Render(1);

        // Assert
        Assert.Equal("* First\n* Second".Replace("\n", Environment.NewLine), renderedLevel0);
        Assert.Equal("  * First\n  * Second".Replace("\n", Environment.NewLine), renderedLevel1);
    }

    [Fact]
    public void BulletList_Empty_ReturnsEmptyString()
    {
        // Arrange
        var list = new BulletList();

        // Act & Assert
        Assert.Equal(string.Empty, list.Render());
    }

    [Fact]
    public void OrderedList_ShouldRenderNumberedItemsSequentially()
    {
        // Arrange
        var list = new OrderedList(["Alpha", "Beta", "Gamma"], startNumber: 1);

        // Act
        string rendered = list.Render(0);

        // Assert
        Assert.Contains("1. Alpha", rendered);
        Assert.Contains("2. Beta", rendered);
        Assert.Contains("3. Gamma", rendered);
    }

    [Fact]
    public void HorizontalSeparator_ShouldRenderSpecifiedLengthAndCharacter()
    {
        // Arrange
        var separator = new HorizontalSeparator('=', 20);

        // Act
        string rendered = separator.Render(1);

        // Assert
        Assert.Equal("  ====================", rendered);
    }
}

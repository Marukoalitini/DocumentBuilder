using DocumentBuilder.Elements;
using DocumentBuilder.Exporters;
using DocBuilder = DocumentBuilder.Builders.DocumentBuilder;

namespace DocumentBuilder.Tests;

public class MarkdownExporterTests
{
    [Fact]
    public void MarkdownExporter_Export_ProducesFormattedMarkdownOutput()
    {
        // Arrange
        Document doc = new DocBuilder("Release Notes", "Dev Lead")
            .WithMetadata("Sprint", "42")
            .AddTitle("Overview", 1)
            .AddParagraph("Initial release notes.")
            .AddImage("https://example.com/badge.png", "Badge", "Build Badge")
            .AddTable(["Module", "Status"], [["Core", "Passing"], ["Demo", "Ready"]])
            .AddBulletList("Feature 1", "Feature 2")
            .AddHorizontalSeparator()
            .AddOrderedList(["Step A", "Step B"])
            .AddSection("Details", s => s.AddParagraph("Deep dive section."))
            .Build();

        var exporter = new MarkdownDocumentExporter();

        // Act
        string output = exporter.Export(doc);

        // Assert
        Assert.NotNull(output);
        Assert.Contains("# Release Notes", output);
        Assert.Contains("> **Author:** Dev Lead", output);
        Assert.Contains("> **Sprint:** 42", output);
        Assert.Contains("# Overview", output);
        Assert.Contains("Initial release notes.", output);
        Assert.Contains("![Build Badge](https://example.com/badge.png \"Badge\")", output);
        Assert.Contains("| Module | Status |", output);
        Assert.Contains("| :--- | :--- |", output);
        Assert.Contains("- Feature 1", output);
        Assert.Contains("- Feature 2", output);
        Assert.Contains("---", output);
        Assert.Contains("1. Step A", output);
        Assert.Contains("2. Step B", output);
        Assert.Contains("## Details", output);
        Assert.Contains("Deep dive section.", output);
    }

    [Fact]
    public void MarkdownExporter_ExportToFile_WritesDocumentToDisk()
    {
        // Arrange
        Document doc = new DocBuilder("File Test").AddParagraph("Disk write test.").Build();
        var exporter = new MarkdownDocumentExporter();
        string tempFile = Path.Combine(Path.GetTempPath(), $"doc_test_{Guid.NewGuid():N}.md");

        try
        {
            // Act
            exporter.ExportToFile(doc, tempFile);

            // Assert
            Assert.True(File.Exists(tempFile));
            string content = File.ReadAllText(tempFile);
            Assert.Contains("# File Test", content);
            Assert.Contains("Disk write test.", content);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public async Task MarkdownExporter_ExportToFileAsync_WritesDocumentToDiskAsynchronously()
    {
        // Arrange
        Document doc = new DocBuilder("Async File Test").AddParagraph("Async disk write test.").Build();
        var exporter = new MarkdownDocumentExporter();
        string tempFile = Path.Combine(Path.GetTempPath(), $"doc_test_async_{Guid.NewGuid():N}.md");

        try
        {
            // Act
            await exporter.ExportToFileAsync(doc, tempFile);

            // Assert
            Assert.True(File.Exists(tempFile));
            string content = await File.ReadAllTextAsync(tempFile);
            Assert.Contains("# Async File Test", content);
            Assert.Contains("Async disk write test.", content);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}

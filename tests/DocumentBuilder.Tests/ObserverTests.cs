using DocumentBuilder.Elements;
using DocumentBuilder.Exporters;
using DocumentBuilder.Observers;
using DocBuilder = DocumentBuilder.Builders.DocumentBuilder;

namespace DocumentBuilder.Tests;

public class ObserverTests
{
    private sealed class MockExportObserver : IExportObserver
    {
        public bool StartedCalled { get; private set; }
        public bool CompletedCalled { get; private set; }
        public List<IDocumentElement> ExportedElements { get; } = [];
        public string? FinalContent { get; private set; }

        public void OnExportStarted(Document document, string fileExtension)
        {
            StartedCalled = true;
        }

        public void OnElementExported(Document document, IDocumentElement element, int currentIndex, int totalCount)
        {
            ExportedElements.Add(element);
        }

        public void OnExportCompleted(Document document, string exportedContent)
        {
            CompletedCalled = true;
            FinalContent = exportedContent;
        }
    }

    [Fact]
    public void Exporter_AttachObserver_NotifiesLifecycleEventsInOrder()
    {
        // Arrange
        Document doc = new DocBuilder("Observer Doc")
            .AddTitle("Heading", 1)
            .AddParagraph("Paragraph 1")
            .AddParagraph("Paragraph 2")
            .Build();

        var exporter = new MarkdownDocumentExporter();
        var observer = new MockExportObserver();
        exporter.Attach(observer);

        // Act
        string output = exporter.Export(doc);

        // Assert
        Assert.True(observer.StartedCalled);
        Assert.True(observer.CompletedCalled);
        Assert.Equal(3, observer.ExportedElements.Count);
        Assert.Equal(output, observer.FinalContent);
    }

    [Fact]
    public void Exporter_DetachObserver_StopsReceivingNotifications()
    {
        // Arrange
        Document doc = new DocBuilder("Detach Doc").AddParagraph("Content").Build();
        var exporter = new MarkdownDocumentExporter();
        var observer = new MockExportObserver();

        exporter.Attach(observer);
        bool detached = exporter.Detach(observer);

        // Act
        exporter.Export(doc);

        // Assert
        Assert.True(detached);
        Assert.False(observer.StartedCalled);
        Assert.Empty(observer.ExportedElements);
    }

    [Fact]
    public void Exporter_CSharpEvents_RaiseProgressPercentagesCorrectly()
    {
        // Arrange
        Document doc = new DocBuilder("Event Doc")
            .AddParagraph("First")
            .AddParagraph("Second")
            .Build();

        var exporter = new HtmlDocumentExporter();
        var stages = new List<(string Stage, double Percentage)>();

        exporter.ExportStarted += (_, e) => stages.Add((e.Stage, e.ProgressPercentage));
        exporter.ElementExported += (_, e) => stages.Add((e.Stage, e.ProgressPercentage));
        exporter.ExportCompleted += (_, e) => stages.Add((e.Stage, e.ProgressPercentage));

        // Act
        exporter.Export(doc);

        // Assert
        Assert.Equal(4, stages.Count);
        Assert.Equal(("Started", 0.0), stages[0]);
        Assert.Equal(("ElementProcessed", 50.0), stages[1]);
        Assert.Equal(("ElementProcessed", 100.0), stages[2]);
        Assert.Equal(("Completed", 100.0), stages[3]);
    }

    private sealed class SelfDetachingObserver(DocumentExporterTemplate exporter) : IExportObserver
    {
        public void OnExportStarted(Document document, string fileExtension) => exporter.Detach(this);

        public void OnElementExported(Document document, IDocumentElement element, int currentIndex, int totalCount)
        {
        }

        public void OnExportCompleted(Document document, string exportedContent)
        {
        }
    }

    [Fact]
    public void Observer_DetachingItselfDuringNotification_ShouldNotBreakExport()
    {
        // Arrange
        Document doc = new DocBuilder("Detach").AddParagraph("Body").Build();
        var exporter = new MarkdownDocumentExporter();
        exporter.Attach(new SelfDetachingObserver(exporter));

        // Act
        string output = exporter.Export(doc);

        // Assert
        Assert.Contains("# Detach", output);
    }
}

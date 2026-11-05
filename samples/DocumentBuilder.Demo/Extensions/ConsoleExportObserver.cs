using DocumentBuilder.Elements;
using DocumentBuilder.Observers;

namespace DocumentBuilder.Demo.Extensions;

/// <summary>
/// Client-side implementation of <see cref="IExportObserver"/> that prints real-time progress to the console.
/// Demonstrates the GoF Observer pattern in action.
/// </summary>
public sealed class ConsoleExportObserver : IExportObserver
{
    public void OnExportStarted(Document document, string fileExtension)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"  [Observer] Starting export of '{document.Title}' to format '{fileExtension}'...");
        Console.ResetColor();
    }

    public void OnElementExported(Document document, IDocumentElement element, int currentIndex, int totalCount)
    {
        double percentage = Math.Round((double)currentIndex / totalCount * 100.0, 0);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  [Observer] [{percentage,3}%] Processed element {currentIndex}/{totalCount}: {element.GetType().Name}");
        Console.ResetColor();
    }

    public void OnExportCompleted(Document document, string exportedContent)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"  [Observer] Export completed ({exportedContent.Length} characters generated).");
        Console.ResetColor();
    }
}

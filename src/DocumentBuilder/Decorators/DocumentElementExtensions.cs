using DocumentBuilder.Elements;

namespace DocumentBuilder.Decorators;

/// <summary>
/// Provides fluent extension methods for wrapping any <see cref="IDocumentElement"/> with decorators.
/// </summary>
public static class DocumentElementExtensions
{
    /// <summary>
    /// Wraps the current document element with a <see cref="BoldDecorator"/>.
    /// </summary>
    /// <param name="element">The element to decorate.</param>
    /// <returns>A <see cref="BoldDecorator"/> wrapping the specified element.</returns>
    public static BoldDecorator AsBold(this IDocumentElement element)
    {
        return new BoldDecorator(element);
    }

    /// <summary>
    /// Wraps the current document element with an <see cref="ItalicDecorator"/>.
    /// </summary>
    /// <param name="element">The element to decorate.</param>
    /// <returns>An <see cref="ItalicDecorator"/> wrapping the specified element.</returns>
    public static ItalicDecorator AsItalic(this IDocumentElement element)
    {
        return new ItalicDecorator(element);
    }

    /// <summary>
    /// Wraps the current document element with a <see cref="BorderDecorator"/>.
    /// </summary>
    /// <param name="element">The element to decorate.</param>
    /// <param name="label">Optional header label for the border box.</param>
    /// <param name="borderCharacter">The character used for drawing the border. Defaults to '='.</param>
    /// <returns>A <see cref="BorderDecorator"/> wrapping the specified element.</returns>
    public static BorderDecorator WithBorder(this IDocumentElement element, string? label = null, char borderCharacter = '=')
    {
        return new BorderDecorator(element, label, borderCharacter);
    }
}

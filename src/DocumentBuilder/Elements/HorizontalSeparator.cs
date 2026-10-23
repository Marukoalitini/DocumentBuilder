namespace DocumentBuilder.Elements;

/// <summary>
/// Represents a horizontal separator or thematic break between document sections.
/// Acts as a leaf node in the composite pattern.
/// </summary>
public sealed class HorizontalSeparator : IDocumentElement
{
    /// <summary>
    /// Gets the character used to represent the separator.
    /// </summary>
    public char Character { get; }

    /// <summary>
    /// Gets the length or repeat count of the separator character.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HorizontalSeparator"/> class.
    /// </summary>
    /// <param name="character">The character used for the separator line. Defaults to '-'.</param>
    /// <param name="length">The length of the separator. Defaults to 40.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="length"/> is less than 1.</exception>
    public HorizontalSeparator(char character = '-', int length = 40)
    {
        if (length < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(length), length, "Length must be at least 1.");
        }

        Character = character;
        Length = length;
    }

    /// <summary>
    /// Renders the horizontal separator with the specified indentation level.
    /// </summary>
    /// <param name="indentationLevel">The zero-based indentation level.</param>
    /// <returns>A formatted horizontal separator string.</returns>
    public string Render(int indentationLevel = 0)
    {
        string indent = new string(' ', Math.Max(0, indentationLevel) * 2);
        return $"{indent}{new string(Character, Length)}";
    }

    /// <inheritdoc />
    public override string ToString() => Render();
}

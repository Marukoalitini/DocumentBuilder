using System.Text;
using DocumentBuilder.Elements;

namespace DocumentBuilder.Decorators;

/// <summary>
/// Decorates a <see cref="IDocumentElement"/> by enclosing its content inside a framed border or callout block.
/// Part of the Decorator design pattern.
/// </summary>
public sealed class BorderDecorator : DocumentElementDecorator
{
    /// <summary>
    /// Gets the optional header label displayed at the top of the border frame.
    /// </summary>
    public string? Label { get; }

    /// <summary>
    /// Gets the character used to render the horizontal border lines.
    /// </summary>
    public char BorderCharacter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BorderDecorator"/> class.
    /// </summary>
    /// <param name="innerElement">The document element to wrap inside a border.</param>
    /// <param name="label">An optional title or label for the bordered block.</param>
    /// <param name="borderCharacter">The character used to draw the border. Defaults to '='.</param>
    public BorderDecorator(IDocumentElement innerElement, string? label = null, char borderCharacter = '=')
        : base(innerElement)
    {
        Label = label;
        BorderCharacter = borderCharacter;
    }

    /// <inheritdoc />
    public override string FormatDecoratedContent(string innerContent, int indentationLevel = 0)
    {
        string indent = new(' ', Math.Max(0, indentationLevel) * 2);
        string[] lines = innerContent.Split('\n').Select(l => l.TrimEnd('\r')).ToArray();

        int maxLineLength = lines.Length > 0 ? lines.Max(l => l.Length) : 20;
        if (!string.IsNullOrWhiteSpace(Label))
        {
            maxLineLength = Math.Max(maxLineLength, Label.Length + 4);
        }

        int width = Math.Max(maxLineLength + 4, 24);
        string horizontalBorder = new(BorderCharacter, width);

        var sb = new StringBuilder();
        sb.AppendLine($"{indent}{horizontalBorder}");

        if (!string.IsNullOrWhiteSpace(Label))
        {
            sb.AppendLine($"{indent}| [{Label.ToUpperInvariant()}]".PadRight(indent.Length + width - 1) + "|");
            sb.AppendLine($"{indent}{horizontalBorder}");
        }

        foreach (string line in lines)
        {
            sb.AppendLine($"{indent}| {line.PadRight(width - 4)} |");
        }

        sb.Append($"{indent}{horizontalBorder}");
        return sb.ToString();
    }
}

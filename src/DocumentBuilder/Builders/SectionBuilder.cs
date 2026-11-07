using DocumentBuilder.Decorators;
using DocumentBuilder.Elements;

namespace DocumentBuilder.Builders;

/// <summary>
/// Provides a fluent builder interface for constructing nested <see cref="Section"/> composite elements.
/// Part of the Builder design pattern.
/// </summary>
public sealed class SectionBuilder
{
    private readonly Section _section;

    /// <summary>
    /// Initializes a new instance of the <see cref="SectionBuilder"/> class.
    /// </summary>
    /// <param name="title">The title of the section being built.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="title"/> is null.</exception>
    public SectionBuilder(string title)
    {
        ArgumentNullException.ThrowIfNull(title);
        _section = new Section(title);
    }

    /// <summary>
    /// Adds a heading/title element to the section.
    /// </summary>
    /// <param name="text">The title text.</param>
    /// <param name="level">The heading level (defaults to 2 for section titles).</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddTitle(string text, int level = 2)
    {
        _section.Add(new Title(text, level));
        return this;
    }

    /// <summary>
    /// Adds a paragraph of text to the section.
    /// </summary>
    /// <param name="text">The paragraph text.</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddParagraph(string text)
    {
        _section.Add(new Paragraph(text));
        return this;
    }

    /// <summary>
    /// Adds a bold-decorated paragraph of text to the section.
    /// </summary>
    /// <param name="text">The paragraph text.</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddBoldParagraph(string text)
    {
        _section.Add(new Paragraph(text).AsBold());
        return this;
    }

    /// <summary>
    /// Adds an italic-decorated paragraph of text to the section.
    /// </summary>
    /// <param name="text">The paragraph text.</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddItalicParagraph(string text)
    {
        _section.Add(new Paragraph(text).AsItalic());
        return this;
    }

    /// <summary>
    /// Adds a paragraph wrapped inside a framed border to the section.
    /// </summary>
    /// <param name="text">The paragraph text.</param>
    /// <param name="label">Optional header label for the border box.</param>
    /// <param name="borderCharacter">The character used to draw the border. Defaults to '='.</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddBorderedParagraph(string text, string? label = null, char borderCharacter = '=')
    {
        _section.Add(new Paragraph(text).WithBorder(label, borderCharacter));
        return this;
    }

    /// <summary>
    /// Adds an image element to the section.
    /// </summary>
    /// <param name="source">The image source URL or file path.</param>
    /// <param name="caption">Optional caption text.</param>
    /// <param name="altText">Optional alternative text description.</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddImage(string source, string? caption = null, string? altText = null)
    {
        _section.Add(new Image(source, caption, altText));
        return this;
    }

    /// <summary>
    /// Adds a structured table to the section.
    /// </summary>
    /// <param name="headers">The table column headers.</param>
    /// <param name="rows">The table rows, where each row is a collection of cell values.</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddTable(IEnumerable<string> headers, IEnumerable<IEnumerable<string>> rows)
    {
        ArgumentNullException.ThrowIfNull(headers);
        ArgumentNullException.ThrowIfNull(rows);

        var table = new Table(headers);
        foreach (var row in rows)
        {
            table.AddRow(row);
        }

        _section.Add(table);
        return this;
    }

    /// <summary>
    /// Adds an unordered bullet list to the section.
    /// </summary>
    /// <param name="items">The collection of text items.</param>
    /// <param name="bulletMarker">The bullet marker prefix. Defaults to "-".</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddBulletList(IEnumerable<string> items, string bulletMarker = "-")
    {
        ArgumentNullException.ThrowIfNull(items);
        _section.Add(new BulletList(items, bulletMarker));
        return this;
    }

    /// <summary>
    /// Adds an unordered bullet list to the section using variable arguments.
    /// </summary>
    /// <param name="items">The items in the list.</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddBulletList(params string[] items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _section.Add(new BulletList(items));
        return this;
    }

    /// <summary>
    /// Adds a sequentially numbered ordered list to the section.
    /// </summary>
    /// <param name="items">The collection of text items.</param>
    /// <param name="startNumber">The sequence starting number. Defaults to 1.</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddOrderedList(IEnumerable<string> items, int startNumber = 1)
    {
        ArgumentNullException.ThrowIfNull(items);
        _section.Add(new OrderedList(items, startNumber));
        return this;
    }

    /// <summary>
    /// Adds a sequentially numbered ordered list to the section using variable arguments.
    /// </summary>
    /// <param name="items">The items in the list.</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddOrderedList(params string[] items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _section.Add(new OrderedList(items));
        return this;
    }

    /// <summary>
    /// Adds a horizontal separator to the section.
    /// </summary>
    /// <param name="character">The character used to draw the separator line. Defaults to '-'.</param>
    /// <param name="length">The length of the separator line. Defaults to 40.</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddHorizontalSeparator(char character = '-', int length = 40)
    {
        _section.Add(new HorizontalSeparator(character, length));
        return this;
    }

    /// <summary>
    /// Adds a formatted source code block to the section.
    /// </summary>
    /// <param name="code">The source code content.</param>
    /// <param name="language">Optional language identifier (e.g., "csharp", "json", "bash").</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddCodeBlock(string code, string? language = null)
    {
        _section.Add(new CodeBlock(code, language));
        return this;
    }

    /// <summary>
    /// Adds a quotation block with an optional citation to the section.
    /// </summary>
    /// <param name="text">The quote text.</param>
    /// <param name="citation">Optional author or source attribution.</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddBlockquote(string text, string? citation = null)
    {
        _section.Add(new Blockquote(text, citation));
        return this;
    }

    /// <summary>
    /// Adds a hyperlink element to the section.
    /// </summary>
    /// <param name="text">The display text for the link.</param>
    /// <param name="url">The destination URL.</param>
    /// <param name="tooltip">Optional tooltip text.</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddHyperlink(string text, string url, string? tooltip = null)
    {
        _section.Add(new Hyperlink(text, url, tooltip));
        return this;
    }

    /// <summary>
    /// Adds an explicit page break instruction to the section.
    /// </summary>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddPageBreak()
    {
        _section.Add(new PageBreak());
        return this;
    }

    /// <summary>
    /// Adds a multi-column side-by-side layout container to the section.
    /// </summary>
    /// <param name="columns">The child elements representing each column.</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddColumns(params IDocumentElement[] columns)
    {
        ArgumentNullException.ThrowIfNull(columns);
        _section.Add(new ColumnLayout(columns));
        return this;
    }

    /// <summary>
    /// Adds an arbitrary document element to the section (supporting extensibility).
    /// </summary>
    /// <param name="element">The document element to add.</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddElement(IDocumentElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        _section.Add(element);
        return this;
    }

    /// <summary>
    /// Adds a nested subsection configured via a fluent sub-builder action.
    /// </summary>
    /// <param name="title">The title of the subsection.</param>
    /// <param name="configure">The configuration action on the child <see cref="SectionBuilder"/>.</param>
    /// <returns>The current section builder for chaining.</returns>
    public SectionBuilder AddSubSection(string title, Action<SectionBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(configure);

        var subSectionBuilder = new SectionBuilder(title);
        configure(subSectionBuilder);
        _section.Add(subSectionBuilder.Build());
        return this;
    }

    /// <summary>
    /// Builds and returns the configured <see cref="Section"/> composite element.
    /// </summary>
    /// <returns>The constructed section instance.</returns>
    public Section Build() => _section;
}

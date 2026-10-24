using DocumentBuilder.Elements;

namespace DocumentBuilder.Builders;

/// <summary>
/// Provides a fluent Builder API to incrementally construct complex <see cref="Document"/> composite structures.
/// Encapsulates the instantiation and hierarchy management of document nodes.
/// </summary>
public sealed class DocumentBuilder
{
    private readonly Document _document;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentBuilder"/> class with an initial title and optional author.
    /// </summary>
    /// <param name="title">The document title. Defaults to "Untitled Document".</param>
    /// <param name="author">The optional document author name.</param>
    public DocumentBuilder(string title = "Untitled Document", string? author = null)
    {
        _document = new Document(title, author);
    }

    /// <summary>
    /// Sets the title of the document.
    /// </summary>
    /// <param name="title">The document title.</param>
    /// <returns>The current builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="title"/> is null.</exception>
    public DocumentBuilder WithTitle(string title)
    {
        ArgumentNullException.ThrowIfNull(title);
        _document.Title = title;
        return this;
    }

    /// <summary>
    /// Sets the author of the document.
    /// </summary>
    /// <param name="author">The document author name.</param>
    /// <returns>The current builder instance for chaining.</returns>
    public DocumentBuilder WithAuthor(string? author)
    {
        _document.Author = author;
        return this;
    }

    /// <summary>
    /// Adds or updates metadata key-value pairs for the document.
    /// </summary>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>The current builder instance for chaining.</returns>
    public DocumentBuilder WithMetadata(string key, string value)
    {
        _document.SetMetadata(key, value);
        return this;
    }

    /// <summary>
    /// Adds a heading/title element to the root document.
    /// </summary>
    /// <param name="text">The title text.</param>
    /// <param name="level">The heading level (between 1 and 6). Defaults to 1.</param>
    /// <returns>The current builder instance for chaining.</returns>
    public DocumentBuilder AddTitle(string text, int level = 1)
    {
        _document.Add(new Title(text, level));
        return this;
    }

    /// <summary>
    /// Adds a paragraph of text to the root document.
    /// </summary>
    /// <param name="text">The paragraph text.</param>
    /// <returns>The current builder instance for chaining.</returns>
    public DocumentBuilder AddParagraph(string text)
    {
        _document.Add(new Paragraph(text));
        return this;
    }

    /// <summary>
    /// Adds an image element to the root document.
    /// </summary>
    /// <param name="source">The image source URL or file path.</param>
    /// <param name="caption">Optional caption text.</param>
    /// <param name="altText">Optional alternative text description.</param>
    /// <returns>The current builder instance for chaining.</returns>
    public DocumentBuilder AddImage(string source, string? caption = null, string? altText = null)
    {
        _document.Add(new Image(source, caption, altText));
        return this;
    }

    /// <summary>
    /// Adds a structured table to the root document.
    /// </summary>
    /// <param name="headers">The table column headers.</param>
    /// <param name="rows">The table rows, where each row is a collection of cell values.</param>
    /// <returns>The current builder instance for chaining.</returns>
    public DocumentBuilder AddTable(IEnumerable<string> headers, IEnumerable<IEnumerable<string>> rows)
    {
        ArgumentNullException.ThrowIfNull(headers);
        ArgumentNullException.ThrowIfNull(rows);

        var table = new Table(headers);
        foreach (var row in rows)
        {
            table.AddRow(row);
        }

        _document.Add(table);
        return this;
    }

    /// <summary>
    /// Adds an unordered bullet list to the root document.
    /// </summary>
    /// <param name="items">The collection of text items.</param>
    /// <param name="bulletMarker">The bullet marker prefix. Defaults to "-".</param>
    /// <returns>The current builder instance for chaining.</returns>
    public DocumentBuilder AddBulletList(IEnumerable<string> items, string bulletMarker = "-")
    {
        ArgumentNullException.ThrowIfNull(items);
        _document.Add(new BulletList(items, bulletMarker));
        return this;
    }

    /// <summary>
    /// Adds an unordered bullet list to the root document using variable arguments.
    /// </summary>
    /// <param name="items">The items in the list.</param>
    /// <returns>The current builder instance for chaining.</returns>
    public DocumentBuilder AddBulletList(params string[] items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _document.Add(new BulletList(items));
        return this;
    }

    /// <summary>
    /// Adds a sequentially numbered ordered list to the root document.
    /// </summary>
    /// <param name="items">The collection of text items.</param>
    /// <param name="startNumber">The sequence starting number. Defaults to 1.</param>
    /// <returns>The current builder instance for chaining.</returns>
    public DocumentBuilder AddOrderedList(IEnumerable<string> items, int startNumber = 1)
    {
        ArgumentNullException.ThrowIfNull(items);
        _document.Add(new OrderedList(items, startNumber));
        return this;
    }

    /// <summary>
    /// Adds a sequentially numbered ordered list to the root document using variable arguments.
    /// </summary>
    /// <param name="items">The items in the list.</param>
    /// <returns>The current builder instance for chaining.</returns>
    public DocumentBuilder AddOrderedList(params string[] items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _document.Add(new OrderedList(items));
        return this;
    }

    /// <summary>
    /// Adds a horizontal separator to the root document.
    /// </summary>
    /// <param name="character">The character used to draw the separator line. Defaults to '-'.</param>
    /// <param name="length">The length of the separator line. Defaults to 40.</param>
    /// <returns>The current builder instance for chaining.</returns>
    public DocumentBuilder AddHorizontalSeparator(char character = '-', int length = 40)
    {
        _document.Add(new HorizontalSeparator(character, length));
        return this;
    }

    /// <summary>
    /// Adds an arbitrary document element to the root document (supporting extensibility).
    /// </summary>
    /// <param name="element">The document element to add.</param>
    /// <returns>The current builder instance for chaining.</returns>
    public DocumentBuilder AddElement(IDocumentElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        _document.Add(element);
        return this;
    }

    /// <summary>
    /// Adds a section composite node configured via a fluent sub-builder action.
    /// </summary>
    /// <param name="title">The title of the section.</param>
    /// <param name="configureSection">The action configuring the <see cref="SectionBuilder"/>.</param>
    /// <returns>The current builder instance for chaining.</returns>
    public DocumentBuilder AddSection(string title, Action<SectionBuilder> configureSection)
    {
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(configureSection);

        var sectionBuilder = new SectionBuilder(title);
        configureSection(sectionBuilder);
        _document.Add(sectionBuilder.Build());
        return this;
    }

    /// <summary>
    /// Builds and returns the final configured <see cref="Document"/> instance.
    /// </summary>
    /// <returns>The constructed document composite tree.</returns>
    public Document Build() => _document;
}

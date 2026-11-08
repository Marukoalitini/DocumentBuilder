# DocumentBuilder

Biblioteca em C# (.NET 10) para montar documentos em memória e exportar para Markdown, HTML, PDF ou qualquer outro formato criado por quem usa. Não tem dependências externas.

O artefato é uma biblioteca de classes, distribuída como DLL ou pacote NuGet. A pasta `samples` contém apenas um projeto cliente de exemplo.

## Integrantes e divisão de padrões

| Aluno | Padrões de projeto | Escopo |
| :--- | :--- | :--- |
| **Marco Alija Ramos Agostini** | Composite e Builder | Estrutura do documento e construção fluente |
| **Edmund Soares de Sousa** | Decorator e Strategy | Estilização dinâmica e escolha do formato de saída |
| **Richard Mickaell Santos Nascimento** | Template Method e Observer | Sequência de exportação e notificações de progresso |

## Vídeo de apresentação

<!-- Depois do upload, troque o texto da linha abaixo pelo link do vídeo no YouTube. -->
**YouTube:** _link a adicionar depois da gravação_

## Índice

- [Como referenciar a biblioteca](#como-referenciar-a-biblioteca)
- [Guia de uso](#guia-de-uso)
- [Estendendo a biblioteca](#estendendo-a-biblioteca)
- [Arquitetura e padrões de projeto](#arquitetura-e-padrões-de-projeto)
- [Limitações conhecidas](#limitações-conhecidas)
- [Estrutura do repositório](#estrutura-do-repositório)
- [Build, testes e demonstração](#build-testes-e-demonstração)

## Como referenciar a biblioteca

O projeto cliente precisa do [SDK do .NET 10](https://dotnet.microsoft.com/download). Escolha uma das três formas abaixo.

### Opção 1: referência de projeto

É a forma mais simples quando o código da biblioteca está na mesma máquina.

```bash
git clone https://github.com/Marukoalitini/DocumentBuilder.git
dotnet new console -n MeuProjeto
cd MeuProjeto
dotnet add reference ../DocumentBuilder/src/DocumentBuilder/DocumentBuilder.csproj
```

### Opção 2: pacote NuGet local

Gere o pacote na pasta do repositório:

```bash
dotnet pack src/DocumentBuilder/DocumentBuilder.csproj -c Release -o ./nupkg
```

Depois, adicione o pacote no seu projeto, informando o caminho da pasta `nupkg`:

```bash
dotnet add package DocumentBuilder --version 1.0.0-preview --source ../DocumentBuilder/nupkg
```

### Opção 3: DLL compilada

Compile a biblioteca:

```bash
dotnet build src/DocumentBuilder/DocumentBuilder.csproj -c Release
```

O arquivo fica em `src/DocumentBuilder/bin/Release/net10.0/DocumentBuilder.dll`. No `.csproj` do cliente, referencie a DLL:

```xml
<ItemGroup>
  <Reference Include="DocumentBuilder">
    <HintPath>caminho/para/DocumentBuilder.dll</HintPath>
  </Reference>
</ItemGroup>
```

### Namespaces

| Namespace | O que contém |
| :--- | :--- |
| `DocumentBuilder.Builders` | `DocumentBuilder` e `SectionBuilder` |
| `DocumentBuilder.Elements` | `Document`, `Section`, `Paragraph`, `Table` e os demais elementos |
| `DocumentBuilder.Decorators` | Decorators de estilo e os métodos `AsBold`, `WithBorder` etc. |
| `DocumentBuilder.Exporters` | Exportadores, `DocumentExportContext` e `DocumentExporterTemplate` |
| `DocumentBuilder.Observers` | `IExportObserver` e `ExportProgressEventArgs` |

A classe `DocumentBuilder` tem o mesmo nome do namespace raiz. Por isso, os exemplos usam um alias:

```csharp
using DocBuilder = DocumentBuilder.Builders.DocumentBuilder;
```

## Guia de uso

### Início rápido

```csharp
using DocumentBuilder.Elements;
using DocumentBuilder.Exporters;
using DocBuilder = DocumentBuilder.Builders.DocumentBuilder;

Document document = new DocBuilder("Relatório de Auditoria")
    .WithAuthor("Equipe de Operações")
    .WithMetadata("Ambiente", "Produção")
    .AddTitle("Sumário Executivo", level: 1)
    .AddParagraph("Relatório consolidado de disponibilidade e uso de recursos.")
    .AddBoldParagraph("Todos os clusters atingiram a meta de SLA no trimestre.")
    .AddSection("Servidores", section => section
        .AddTable(
            headers: ["Identificador", "Tipo", "Status"],
            rows: [
                ["node-prod-01", "c6i.4xlarge", "Operacional"],
                ["node-prod-02", "c6i.4xlarge", "Operacional"]
            ])
        .AddBorderedParagraph("Nenhuma falha crítica registrada.", label: "Storage"))
    .Build();

var exportContext = new DocumentExportContext(new MarkdownDocumentExporter());
exportContext.ExportToFile(document, "relatorio.md");

exportContext.SetStrategy(new HtmlDocumentExporter());
exportContext.ExportToFile(document, "relatorio.html");

exportContext.SetStrategy(new PdfDocumentExporter());
exportContext.ExportToFile(document, "relatorio.pdf");
```

### Dados do documento

| Método | O que faz |
| :--- | :--- |
| `new DocBuilder(title, author)` | Cria o builder. Os dois parâmetros são opcionais. |
| `WithTitle(title)` | Define o título. |
| `WithAuthor(author)` | Define o autor. |
| `WithMetadata(key, value)` | Adiciona ou atualiza um metadado. |
| `Build()` | Devolve o `Document` montado. |

### Elementos

Os métodos abaixo existem em `DocumentBuilder` e em `SectionBuilder`. Os parâmetros com `?` são opcionais.

| Método | Elemento gerado |
| :--- | :--- |
| `AddTitle(text, level?)` | `Title`, com nível de 1 a 6 |
| `AddParagraph(text)` | `Paragraph` |
| `AddBoldParagraph(text)` | `Paragraph` com negrito |
| `AddItalicParagraph(text)` | `Paragraph` com itálico |
| `AddBorderedParagraph(text, label?, borderCharacter?)` | `Paragraph` com borda |
| `AddImage(source, caption?, altText?)` | `Image` |
| `AddTable(headers, rows)` | `Table` |
| `AddBulletList(items, bulletMarker?)` | `BulletList` |
| `AddOrderedList(items, startNumber?)` | `OrderedList` |
| `AddHorizontalSeparator(character?, length?)` | `HorizontalSeparator` |
| `AddCodeBlock(code, language?)` | `CodeBlock` |
| `AddBlockquote(text, citation?)` | `Blockquote` |
| `AddHyperlink(text, url, tooltip?)` | `Hyperlink` |
| `AddPageBreak()` | `PageBreak` |
| `AddColumns(columns)` | `ColumnLayout`, com um elemento por coluna |
| `AddElement(element)` | Qualquer `IDocumentElement`, inclusive os criados por você |

`AddBulletList` e `AddOrderedList` também aceitam os itens direto, como em `AddBulletList("A", "B")`.

### Seções e subseções

`DocumentBuilder.AddSection` cria uma seção, e `SectionBuilder.AddSubSection` cria subseções em qualquer profundidade.

```csharp
using DocumentBuilder.Elements;
using DocBuilder = DocumentBuilder.Builders.DocumentBuilder;

Document document = new DocBuilder("Manual")
    .AddSection("Instalação", section => section
        .AddParagraph("Siga os passos abaixo.")
        .AddOrderedList("Baixar", "Instalar", "Configurar")
        .AddSubSection("Linux", sub => sub
            .AddCodeBlock("sudo apt install exemplo", language: "bash")))
    .Build();
```

### Decorators

Os decorators envolvem qualquer `IDocumentElement` e podem ser empilhados.

| Método | Decorator | Efeito |
| :--- | :--- | :--- |
| `AsBold()` | `BoldDecorator` | Negrito |
| `AsItalic()` | `ItalicDecorator` | Itálico |
| `AsUnderlined()` | `UnderlineDecorator` | Sublinhado |
| `AsStrikethrough()` | `StrikethroughDecorator` | Tachado |
| `WithHighlight(color?)` | `HighlightDecorator` | Realce |
| `WithAlignment(alignment)` | `AlignmentDecorator` | `Left`, `Center`, `Right` ou `Justify` |
| `WithBorder(label?, borderCharacter?)` | `BorderDecorator` | Caixa com borda e rótulo |

```csharp
using DocumentBuilder.Decorators;
using DocumentBuilder.Elements;
using DocBuilder = DocumentBuilder.Builders.DocumentBuilder;

Document document = new DocBuilder("Avisos")
    .AddElement(new Paragraph("Atenção ao prazo.")
        .AsBold()
        .WithHighlight("#fef08a")
        .WithAlignment(TextAlignment.Center))
    .Build();
```

### Exportação

| Classe | Extensão | Observação |
| :--- | :--- | :--- |
| `MarkdownDocumentExporter` | `.md` | Markdown compatível com GitHub |
| `HtmlDocumentExporter` | `.html` | Página HTML5 completa, com estilos embutidos |
| `PdfDocumentExporter` | `.pdf` | PDF 1.4 com várias páginas, sem bibliotecas externas |

Todo exportador oferece três operações, que também existem em `DocumentExportContext`:

```csharp
using DocumentBuilder.Exporters;

var exporter = new HtmlDocumentExporter();

string html = exporter.Export(document);
exporter.ExportToFile(document, "saida/relatorio.html");
await exporter.ExportToFileAsync(document, "saida/relatorio.html");
```

O `DocumentExportContext` guarda o exportador atual e permite trocar o formato em tempo de execução com `SetStrategy`. Se nenhum exportador for informado, ele usa Markdown. As pastas do caminho de destino são criadas automaticamente.

No PDF, prefira `ExportToFile`: ele grava o arquivo com a codificação correta.

### Acompanhando o progresso

Todo exportador derivado de `DocumentExporterTemplate` aceita observadores:

```csharp
using DocumentBuilder.Elements;
using DocumentBuilder.Exporters;
using DocumentBuilder.Observers;

var exporter = new PdfDocumentExporter();
exporter.Attach(new ObservadorDeConsole());
exporter.ExportToFile(document, "relatorio.pdf");

public sealed class ObservadorDeConsole : IExportObserver
{
    public void OnExportStarted(Document document, string fileExtension) =>
        Console.WriteLine($"Exportando para {fileExtension}");

    public void OnElementExported(Document document, IDocumentElement element, int currentIndex, int totalCount) =>
        Console.WriteLine($"{currentIndex}/{totalCount}: {element.GetType().Name}");

    public void OnExportCompleted(Document document, string exportedContent) =>
        Console.WriteLine("Concluído");
}
```

Para remover um observador, use `Detach`. Quem preferir eventos do C# pode assinar `ExportStarted`, `ElementExported` e `ExportCompleted`:

```csharp
using DocumentBuilder.Exporters;

var exporter = new MarkdownDocumentExporter();
exporter.ElementExported += (_, args) =>
    Console.WriteLine($"{args.ProgressPercentage}% de {args.DocumentTitle}");

exporter.Export(document);
```

## Estendendo a biblioteca

Todas as extensões abaixo ficam no projeto cliente, sem alterar o código da biblioteca.

### Novo formato de saída

Herde de `DocumentExporterTemplate` e implemente os passos de formatação. A sequência da exportação, a gravação em arquivo e as notificações já vêm prontas.

```csharp
using DocumentBuilder.Elements;
using DocumentBuilder.Exporters;

var exportContext = new DocumentExportContext(new TextoSimplesExporter());
exportContext.ExportToFile(document, "relatorio.txt");

public sealed class TextoSimplesExporter : DocumentExporterTemplate
{
    public override string FileExtension => ".txt";

    protected override string FormatHeader(Document document) =>
        document.Title.ToUpperInvariant() + Environment.NewLine + Environment.NewLine;

    protected override string FormatMetadata(Document document) =>
        $"Autor: {document.Author}{Environment.NewLine}{Environment.NewLine}";

    protected override string FormatTitle(Title title, int currentHeadingLevel) => title.Text.ToUpperInvariant();
    protected override string FormatParagraph(Paragraph paragraph, int currentHeadingLevel) => paragraph.Text;
    protected override string FormatImage(Image image, int currentHeadingLevel) => image.Render();
    protected override string FormatTable(Table table, int currentHeadingLevel) => table.Render();
    protected override string FormatBulletList(BulletList bulletList, int currentHeadingLevel) => bulletList.Render();
    protected override string FormatOrderedList(OrderedList orderedList, int currentHeadingLevel) => orderedList.Render();
    protected override string FormatHorizontalSeparator(HorizontalSeparator separator, int currentHeadingLevel) => separator.Render();
}
```

Além dos passos acima, estes métodos podem ser sobrescritos quando necessário:

| Método | Comportamento padrão |
| :--- | :--- |
| `FormatFooter` | Não escreve nada |
| `FinalizeExport` | Devolve o conteúdo sem alteração |
| `FormatCustomElement` | Usa o `Render` do elemento |
| `FormatCustomDecorator` | Usa o `FormatDecoratedContent` do decorator |
| `FormatCodeBlock`, `FormatBlockquote`, `FormatHyperlink`, `FormatPageBreak`, `FormatColumnLayout`, `FormatSection` | Usam uma formatação genérica |
| `FormatBold`, `FormatItalic`, `FormatUnderline`, `FormatStrikethrough`, `FormatHighlight`, `FormatAlignment`, `FormatBorder` | Usam a marcação padrão do decorator |
| `OutputEncoding` | UTF-8 |

Também é possível implementar `IDocumentExporter` do zero, sem herdar da classe base. Nesse caso, `Attach` e os eventos de progresso não estão disponíveis.

```csharp
using DocumentBuilder.Elements;
using DocumentBuilder.Exporters;

var exportContext = new DocumentExportContext(new ResumoJsonExporter());
Console.WriteLine(exportContext.Export(document));

public sealed class ResumoJsonExporter : IDocumentExporter
{
    public string FileExtension => ".json";

    public string Export(Document document) =>
        $"{{\"titulo\":\"{document.Title}\",\"elementos\":{document.Elements.Count}}}";

    public void ExportToFile(Document document, string filePath) =>
        File.WriteAllText(filePath, Export(document));

    public Task ExportToFileAsync(Document document, string filePath, CancellationToken cancellationToken = default) =>
        File.WriteAllTextAsync(filePath, Export(document), cancellationToken);
}
```

### Novo elemento

Implemente `IDocumentElement` e adicione com `AddElement`. Nos exportadores prontos, o elemento aparece com o texto do próprio `Render`.

```csharp
using DocumentBuilder.Elements;
using DocBuilder = DocumentBuilder.Builders.DocumentBuilder;

Document document = new DocBuilder("Avisos")
    .AddElement(new Etiqueta("Novo"))
    .Build();

public sealed class Etiqueta(string texto) : IDocumentElement
{
    public string Texto { get; } = texto;

    public string Render(int indentationLevel = 0) => $"[{Texto}]";
}
```

### Saída específica para o novo elemento

Os exportadores prontos podem ser herdados. Basta sobrescrever `FormatCustomElement`:

```csharp
using System.Net;
using DocumentBuilder.Elements;
using DocumentBuilder.Exporters;

public sealed class Etiqueta(string texto) : IDocumentElement
{
    public string Texto { get; } = texto;

    public string Render(int indentationLevel = 0) => $"[{Texto}]";
}

public sealed class MeuHtmlExporter : HtmlDocumentExporter
{
    protected override string FormatCustomElement(IDocumentElement element, int currentHeadingLevel) =>
        element is Etiqueta etiqueta
            ? $"<span class=\"etiqueta\">{WebUtility.HtmlEncode(etiqueta.Texto)}</span>"
            : base.FormatCustomElement(element, currentHeadingLevel);
}
```

### Novo decorator

Herde de `DocumentElementDecorator` e sobrescreva `FormatDecoratedContent`. O exportador formata o elemento interno e entrega o resultado nesse método.

```csharp
using DocumentBuilder.Decorators;
using DocumentBuilder.Elements;
using DocBuilder = DocumentBuilder.Builders.DocumentBuilder;

Document document = new DocBuilder("Avisos")
    .AddElement(new UrgenteDecorator(new Paragraph("Responder hoje").AsBold()))
    .Build();

public sealed class UrgenteDecorator(IDocumentElement elemento) : DocumentElementDecorator(elemento)
{
    public override string FormatDecoratedContent(string innerContent, int indentationLevel = 0) =>
        $"URGENTE: {innerContent}";
}
```

## Arquitetura e padrões de projeto

```text
  Builder                 Composite                   Strategy
  DocumentBuilder   --->  Document, Section,    --->  DocumentExportContext
  SectionBuilder          ColumnLayout e folhas       IDocumentExporter
                               ^                            |
                               |                            v
  Observer                Decorator                   Template Method
  IExportObserver   <---  DocumentElementDecorator    DocumentExporterTemplate
       ^                  e 7 decorators              Markdown, HTML, PDF
       |                                                    |
       +----------------------------------------------------+
                         notifica cada etapa
```

### Composite (Marco Alija Ramos Agostini)

- **Problema:** um documento é uma árvore. Ele tem seções, subseções e, nas pontas, parágrafos, tabelas e listas. Sem uma abstração comum, o código teria que tratar cada tipo e controlar a profundidade manualmente.
- **Solução:** `IDocumentElement` unifica os nós. `CompositeElement` é a base de `Document`, `Section` e `ColumnLayout`, e as folhas implementam a interface diretamente.

### Builder (Marco Alija Ramos Agostini)

- **Problema:** montar a árvore manualmente exige muitos `new` aninhados e controle de quem é filho de quem.
- **Solução:** `DocumentBuilder` e `SectionBuilder` oferecem uma API fluente, na variante sem Director. O cliente descreve o conteúdo, e os builders cuidam da hierarquia.

### Decorator (Edmund Soares de Sousa)

- **Problema:** criar uma classe para cada combinação de estilo geraria uma explosão de subclasses, como `BoldParagraph` e `BoldItalicParagraph`.
- **Solução:** `DocumentElementDecorator` implementa `IDocumentElement` e envolve outro elemento. Os estilos podem ser combinados em tempo de execução.

### Strategy (Edmund Soares de Sousa)

- **Problema:** o mesmo documento precisa sair em formatos diferentes, escolhidos em tempo de execução, sem colocar regras de formato no modelo.
- **Solução:** `IDocumentExporter` define o contrato, e `DocumentExportContext` guarda e troca a estratégia atual.

### Template Method (Richard Mickaell Santos Nascimento)

- **Problema:** todos os exportadores seguem a mesma sequência, e só muda a forma de escrever cada parte.
- **Solução:** `DocumentExporterTemplate.Export` fixa a sequência e delega os passos variáveis às subclasses. Métodos opcionais, como `FormatFooter` e `FinalizeExport`, funcionam como hooks.

### Observer (Richard Mickaell Santos Nascimento)

- **Problema:** aplicações precisam acompanhar o progresso sem acoplar interface ou log ao exportador.
- **Solução:** `IExportObserver`, `Attach` e `Detach` registram quem recebe os avisos. Também há eventos nativos do C#.

## Limitações conhecidas

- O PDF desenha uma caixa com a legenda e o endereço da imagem, sem embutir o arquivo.
- O PDF usa fontes padrão com Latin-1. Os acentos do português funcionam, mas caracteres fora dessa tabela viram `?`.
- No PDF, ao combinar estilos do mesmo tipo em um parágrafo, como negrito e itálico, vale o último aplicado. Em títulos, listas e tabelas, os estilos de texto são ignorados.
- `Build()` devolve sempre a mesma instância de `Document`. Use um novo builder para cada documento.
- Uma exceção lançada dentro de um observador interrompe a exportação.

## Estrutura do repositório

```text
DocumentBuilder/
├── DocumentBuilder.sln
├── src/DocumentBuilder/         Biblioteca
│   ├── Elements/                Composite: documento, seções e elementos
│   ├── Builders/                Builder: DocumentBuilder e SectionBuilder
│   ├── Decorators/              Decorator: estilos
│   ├── Exporters/               Strategy e Template Method: Markdown, HTML e PDF
│   └── Observers/               Observer: progresso da exportação
├── samples/DocumentBuilder.Demo/
│   ├── Extensions/              Classes do cliente: exportador, elemento e observer
│   └── Program.cs               Uso padrão e caso de extensão
└── tests/DocumentBuilder.Tests/ Testes unitários com xUnit
```

## Build, testes e demonstração

```bash
dotnet build
dotnet test
dotnet run --project samples/DocumentBuilder.Demo
```

A demonstração grava os arquivos em `samples/DocumentBuilder.Demo/bin/Debug/net10.0/output/`.

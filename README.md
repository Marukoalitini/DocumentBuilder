# DocumentBuilder

Biblioteca em C# (.NET 10) para construção programática e exportação extensível de documentos estruturados em memória.

O projeto foi desenvolvido com foco em boas práticas de orientação a objetos, aderência aos princípios SOLID (especialmente o Princípio Aberto/Fechado — OCP) e aplicação integrada de 6 Design Patterns (GoF).

---

## Integrantes e Divisão de Padrões

| Aluno | Padrões de Projeto | Escopo |
| :--- | :--- | :--- |
| **Marco Alija Ramos Agostini** | Composite & Builder | Estrutura em memória e construção fluente |
| **Edmund Soares de Sousa** | Strategy & Decorator | Estratégias de exportação e estilização dinâmica |
| **Richard Mickaell Santos Nascimento** | Template Method & Observer | Pipeline de processamento e notificações de progresso |

---

## Arquitetura e Design Patterns

A arquitetura separa a representação semântica do documento (`Elements`), a construção fluente (`Builders`), a estilização dinâmica (`Decorators`), o algoritmo de exportação (`Exporters`) e o monitoramento de ciclo de vida (`Observers`).

```
               +------------------------------------+
               |          IDocumentElement          |
               +------------------------------------+
               | + Render(indentationLevel): string |
               +------------------------------------+
                                 ▲
          +----------------------+----------------------+----------------------+
          |                                             |                      |
+---------------------+                       +--------------------+ +-----------------------+
|  Composite Nodes    |                       |     Leaf Nodes     | |      Decorators       |
+---------------------+                       +--------------------+ +-----------------------+
| - Document          |                       | - Paragraph        | | - BoldDecorator       |
| - Section           |                       | - Title            | | - ItalicDecorator     |
+---------------------+                       | - Table            | | - BorderDecorator     |
                                              | - Image            | +-----------------------+
                                              | - BulletList       |
                                              | - OrderedList      |
                                              | - HorizontalSeparator|
                                              +--------------------+
```

### 1. Composite (Estrutural) — *Marco Alija Ramos Agostini*
- **Problema:** Documentos possuem estruturas recursivas em árvore (um documento contém seções, que contêm subseções ou folhas como parágrafos, listas, tabelas e imagens). Sem uma abstração comum, o processamento exigiria condicionais acopladas a cada tipo concreto.
- **Solução:** A interface `IDocumentElement` unifica nós compostos (`Document`, `Section`) e folhas (`Paragraph`, `Title`, `Table`, `Image`, `BulletList`, `OrderedList`, `HorizontalSeparator`), permitindo percorrer toda a hierarquia de maneira polimórfica.

### 2. Builder (Criacional) — *Marco Alija Ramos Agostini*
- **Problema:** Instanciar manualmente a árvore de objetos e controlar coleções de subseções torna o código do desenvolvedor cliente verboso e propenso a erros.
- **Solução:** As classes `DocumentBuilder` e `SectionBuilder` expõem uma API fluente que encapsula a montagem passo a passo da árvore e devolve uma instância consistente de `Document`.

### 3. Decorator (Estrutural) — *Edmund Soares de Sousa*
- **Problema:** Adicionar variações de estilo e destaque (negrito, itálico, caixas com borda) por herança geraria uma explosão combinatória de subclasses (`BoldParagraph`, `BorderedBoldItalicParagraph`, etc.).
- **Solução:** A classe base `DocumentElementDecorator` implementa `IDocumentElement` e envolve qualquer elemento existente. Os decoradores concretos (`BoldDecorator`, `ItalicDecorator`, `BorderDecorator`) podem ser combinados dinamicamente em tempo de execução.

### 4. Strategy (Comportamental) — *Edmund Soares de Sousa*
- **Problema:** O documento em memória precisa ser exportado para diferentes formatos (Markdown, HTML, texto puro, etc.) sem acoplar as regras de serialização dentro das classes de domínio e permitindo trocar o formato em tempo de execução.
- **Solução:** A interface `IDocumentExporter` define o contrato de estratégia, gerenciada pelo contexto `DocumentExportContext`. A biblioteca fornece nativamente `MarkdownDocumentExporter` (`.md`) e `HtmlDocumentExporter` (`.html`), permitindo que o cliente crie novas estratégias sem alterar a biblioteca.

### 5. Template Method (Comportamental) — *Richard Mickaell Santos Nascimento*
- **Problema:** Apesar de gerarem saídas diferentes, todos os exportadores compartilham o mesmo ciclo de processamento (validação, cabeçalho, metadados, iteração pelos elementos do corpo, rodapé e gravação em disco).
- **Solução:** A classe abstrata `DocumentExporterTemplate` define o método principal `Export(Document)` com o esqueleto fixo do algoritmo, delegando apenas a formatação específica (`FormatHeader`, `FormatMetadata`, `FormatTable`, etc.) para as subclasses.

### 6. Observer (Comportamental) — *Richard Mickaell Santos Nascimento*
- **Problema:** Aplicações clientes precisam acompanhar o progresso da exportação (início, elemento atual processado, percentual concluído e término) sem acoplar lógica de interface ou log dentro do exportador.
- **Solução:** A interface `IExportObserver` e os eventos nativos (`ExportStarted`, `ElementExported`, `ExportCompleted`) em `DocumentExporterTemplate` notificam automaticamente todos os observadores registrados a cada etapa da exportação.

---

## Estrutura do Repositório

```text
DocumentBuilder/
├── DocumentBuilder.sln
├── src/
│   └── DocumentBuilder/                # Biblioteca de classes (artefato central - DLL)
│       ├── Elements/                   # Padrão Composite (IDocumentElement, Document, Section, folhas)
│       ├── Builders/                   # Padrão Builder (DocumentBuilder, SectionBuilder)
│       ├── Decorators/                 # Padrão Decorator (Bold, Italic, BorderDecorator)
│       ├── Exporters/                  # Padrões Strategy e Template Method (Markdown, HTML, Context)
│       └── Observers/                  # Padrão Observer (IExportObserver, ExportProgressEventArgs)
├── samples/
│   └── DocumentBuilder.Demo/           # Aplicação cliente demonstrando uso padrão e extensão OCP
│       ├── Extensions/                 # Classes criadas pelo cliente (CalloutBox, PlainTextDocumentExporter)
│       └── Program.cs
└── tests/
    └── DocumentBuilder.Tests/          # Suíte de testes unitários automatizados (xUnit)
        ├── CompositeTests.cs
        ├── BuilderTests.cs
        ├── DecoratorTests.cs
        ├── StrategyAndTemplateTests.cs
        ├── ObserverTests.cs
        └── MarkdownExporterTests.cs
```

---

## Como Executar

### Compilação
```bash
dotnet build
```

### Testes Unitários
```bash
dotnet test
```

### Executar a Demonstração
```bash
dotnet run --project samples/DocumentBuilder.Demo
```

---

## Guia de Uso da Biblioteca

### 1. Uso Padrão (Out-of-the-Box)
```csharp
using DocumentBuilder.Decorators;
using DocumentBuilder.Elements;
using DocumentBuilder.Exporters;
using DocBuilder = DocumentBuilder.Builders.DocumentBuilder;

// 1. Construção fluente do documento (Builder + Composite + Decorator)
Document document = new DocBuilder("Relatório de Auditoria de Infraestrutura")
    .WithAuthor("Operations Group")
    .WithMetadata("Ambiente", "Produção")
    .AddTitle("Sumário Executivo", level: 1)
    .AddParagraph("Relatório consolidado de disponibilidade e utilização de recursos.")
    .AddBoldParagraph("Todos os clusters atingiram a meta de SLA no trimestre.")
    .AddSection("Alocação de Servidores", section => section
        .AddTable(
            headers: ["Identificador", "Tipo de Instância", "Status"],
            rows: [
                ["node-prod-01", "c6i.4xlarge", "Operacional"],
                ["node-prod-02", "c6i.4xlarge", "Operacional"]
            ]
        )
        .AddBorderedParagraph("Nenhuma falha crítica de disco registrada.", label: "Storage")
    )
    .Build();

// 2. Exportação utilizando Strategy (Markdown e HTML nativos)
var exportContext = new DocumentExportContext(new MarkdownDocumentExporter());
exportContext.ExportToFile(document, "relatorio.md");

exportContext.SetStrategy(new HtmlDocumentExporter());
exportContext.ExportToFile(document, "relatorio.html");
```

### 2. Estendendo a Biblioteca (Princípio Aberto/Fechado - OCP)
O desenvolvedor cliente pode criar novos formatos de exportação herdando de `DocumentExporterTemplate` ou novos elementos implementando `IDocumentElement` diretamente no seu projeto, sem modificar o código-fonte da biblioteca:

```csharp
// Novo exportador definido no projeto cliente
public sealed class PlainTextDocumentExporter : DocumentExporterTemplate
{
    public override string FileExtension => ".txt";
    // Sobrescreve os passos de formatação para texto puro...
}

// Uso transparente no contexto de estratégia da biblioteca
exportContext.SetStrategy(new PlainTextDocumentExporter());
exportContext.ExportToFile(document, "relatorio.txt");
```

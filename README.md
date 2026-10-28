# DocumentBuilder

Biblioteca em C# (.NET 10) para construção programática e exportação extensível de documentos estruturados em memória.

O projeto foi desenvolvido com foco em boas práticas de orientação a objetos, aderência aos princípios SOLID e aplicação prática de Design Patterns (GoF).

---

## Integrantes e Divisão de Padrões

| Aluno | Padrões de Projeto | Apresentação |
| :--- | :--- | :--- |
| **Marco Alija Ramos Agostini** | Composite & Builder | Entrega Parcial (29/10) |
| **Edmund Soares de Sousa** | Strategy & Decorator | Entrega Final (12/11) |
| **Richard Mickaell Santos Nascimento** | Template Method & Observer | Entrega Final (12/11) |

## Arquitetura e Design Patterns

A arquitetura do projeto separa a estrutura hierárquica do documento da lógica de construção e dos formatos de exportação.

```
               +------------------------------------+
               |          IDocumentElement          |
               +------------------------------------+
               | + Render(indentationLevel): string |
               +------------------------------------+
                                 ▲
          +----------------------+----------------------+
          |                                             |
+---------------------+                       +--------------------+
|  Composite Nodes    |                       |     Leaf Nodes     |
+---------------------+                       +--------------------+
| - Document          |                       | - Paragraph        |
| - Section           |                       | - Title            |
+---------------------+                       | - Table            |
                                              | - Image            |
                                              | - BulletList       |
                                              | - OrderedList      |
                                              | - HorizontalRule   |
                                              +--------------------+
```

### 1. Composite (Estrutural)
- **Problema:** Documentos são estruturas compostas por blocos aninhados (um documento contém seções, que por sua vez contêm parágrafos, listas, tabelas, imagens ou outras subseções). Sem uma abstração comum, a manipulação exigiria múltiplas condicionais e acoplamento a tipos específicos.
- **Solução:** A interface `IDocumentElement` padroniza o contrato de todos os elementos. `Document` e `Section` atuam como nós compostos contendo coleções de filhos, enquanto `Paragraph`, `Title`, `Table`, `Image`, `BulletList`, `OrderedList` e `HorizontalRule` representam as folhas, permitindo percorrer e renderizar a estrutura de forma polimórfica.

### 2. Builder (Criacional)
- **Problema:** Instanciar manualmente a árvore de objetos e gerenciar referências e coleções aninhadas torna o código cliente complexo e propenso a erros.
- **Solução:** A classe `DocumentBuilder` oferece uma API fluente para configuração e montagem passo a passo do documento. O builder encapsula as instanciações de nós e folhas, entregando ao final uma instância consistente de `Document`.

---

## Estrutura do Repositório

```text
DocumentBuilder/
├── DocumentBuilder.sln
├── src/
│   └── DocumentBuilder/                # Biblioteca de classes (artefato central)
│       ├── Elements/                   # Nós compostos e folhas (Composite)
│       ├── Builders/                   # Fluent API (Builder)
│       └── Exporters/                  # Contratos e exportador padrão Markdown
├── samples/
│   └── DocumentBuilder.Demo/           # Projeto de console para teste e demonstração
│       └── Program.cs
└── tests/
    └── DocumentBuilder.Tests/          # Suíte de testes unitários automatizados (xUnit)
        ├── CompositeTests.cs
        ├── BuilderTests.cs
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

## Exemplo de Uso

### Construção do Documento via Builder
```csharp
using DocumentBuilder.Builders;
using DocumentBuilder.Elements;
using DocumentBuilder.Exporters;
using DocBuilder = DocumentBuilder.Builders.DocumentBuilder;

Document document = new DocBuilder("Relatório de Auditoria de Infraestrutura")
    .WithAuthor("Operations Group")
    .WithMetadata("Ambiente", "Produção")
    .WithMetadata("Classificação", "Confidencial")
    .AddTitle("1. Sumário Executivo", level: 1)
    .AddParagraph("Relatório consolidado de disponibilidade e utilização de recursos.")
    .AddImage("https://monitoring.corp/charts/cluster-q3.png", caption: "Figura 1: Distribuição de Carga")
    .AddSection("2. Alocação de Servidores", section => section
        .AddParagraph("Tabela de capacidade operacional dos nós:")
        .AddTable(
            headers: ["Identificador", "Tipo de Instância", "Status"],
            rows: [
                ["node-prod-01", "c6i.4xlarge", "Operacional"],
                ["node-prod-02", "c6i.4xlarge", "Operacional"]
            ]
        )
        .AddSubSection("Métricas de Armazenamento", sub => sub
            .AddParagraph("Latência média de I/O mantida abaixo de 1 milissegundo.")
        )
        .AddBulletList([
            "Backup diário realizado com sucesso.",
            "Zero vulnerabilidades críticas encontradas."
        ])
    )
    .Build();
```

### Exportação em Formato Markdown (.md)
```csharp
IDocumentExporter exporter = new MarkdownDocumentExporter();

// Obter representação em Markdown na memória
string markdown = exporter.Export(document);
Console.WriteLine(markdown);

// Salvar diretamente em arquivo .md
exporter.ExportToFile(document, "relatorio.md");
```

---

## Divisão de Padrões por Integrante

- **Marco Alija Ramos Agostini:**
  - `Composite`: Modelagem uniforme de nós (`Document`, `Section`) e folhas (`Paragraph`, `Title`, `Table`, `Image`).
  - `Builder`: Interface fluente para criação e configuração estruturada do documento.
- **Edmund Soares de Sousa:**
  - `Strategy`: Interface de exportação e suporte a múltiplos formatos sem acoplamento.
  - `Decorator`: Formatação e decoração dinâmica de elementos (negrito, itálico, bordas).
- **Richard Mickaell Santos Nascimento:**
  - `Template Method`: Esqueleto padronizado do ciclo de vida de processamento e exportação.
  - `Observer`: Sistema de eventos para notificação de progresso da exportação.

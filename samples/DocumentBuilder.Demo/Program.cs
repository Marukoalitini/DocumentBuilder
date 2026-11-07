using DocumentBuilder.Decorators;
using DocumentBuilder.Demo.Extensions;
using DocumentBuilder.Elements;
using DocumentBuilder.Exporters;
using DocBuilder = DocumentBuilder.Builders.DocumentBuilder;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("================================================================");
Console.WriteLine("   DocumentBuilder - Complete Architecture Demonstration (.NET 10)");
Console.WriteLine("================================================================");
Console.ResetColor();

// ---------------------------------------------------------------------------
// PART 1: Building a complex document using Composite, Builder, and Decorator
// ---------------------------------------------------------------------------
Console.WriteLine("\n[1] Constructing document using Composite, Builder, and Decorator patterns...");

Document document = new DocBuilder("Global Cloud Infrastructure & Systems Audit")
    .WithAuthor("Enterprise Operations Group")
    .WithMetadata("Environment", "Production - Multi-Region")
    .WithMetadata("Audit-ID", "AUD-2026-8842-X")
    .WithMetadata("Classification", "Confidential - Internal Use")
    .WithMetadata("Compliance", "SOC2 / ISO 27001 Certified")
    .AddTitle("Executive Summary", level: 1)
    .AddParagraph("During the third quarter of 2026, the global cloud infrastructure maintained " +
                  "an overall system availability of 99.987%, exceeding the agreed Service Level " +
                  "Objective (SLO) of 99.95%. This audit report presents a consolidated breakdown " +
                  "of compute cluster utilization, data layer performance, and security posture " +
                  "across all active regional zones.")
    .AddBoldParagraph("Key Finding: All regional production clusters met high-availability targets without service interruption.")
    .AddImage("https://images.unsplash.com/photo-1558494949-ef010cbdcc31?w=800",
              caption: "Figure 1.1: Multi-Region Compute Cluster Workload Distribution",
              altText: "Cluster Workload Distribution")
    .AddSection("Compute Infrastructure & Workload Health", section => section
        .AddParagraph("The compute layer comprises distributed container clusters deployed across " +
                      "North America and Europe. The table below details resource utilization metrics:")
        .AddTable(
            headers: ["Cluster Identifier", "Node Type", "Active Nodes", "Avg CPU Load", "Health Status"],
            rows: [
                ["prod-useast-k8s-01", "c6i.4xlarge", "48", "62.4%", "Optimal"],
                ["prod-useast-k8s-02", "m6i.4xlarge", "32", "58.1%", "Optimal"],
                ["prod-euwest-k8s-01", "c6i.4xlarge", "36", "71.3%", "Warning (High Load)"],
                ["prod-euwest-k8s-02", "r6g.8xlarge", "16", "44.9%", "Optimal"]
            ]
        )
        .AddSubSection("Cluster Storage & IOPS Metrics", sub => sub
            .AddParagraph("Average persistent volume latency was measured at 0.72 milliseconds, " +
                          "with zero unrecoverable volume degradation events recorded.")
            .AddItalicParagraph("Note: IOPS measurements were collected under peak load conditions.")
            .AddBlockquote(
                "Infrastructure resilience has reached its highest historical benchmark since multi-region active-active failover was deployed.",
                citation: "Principal Site Reliability Architect")
        )
        .AddSubSection("Regional Distribution & Diagnostic Script", sub => sub
            .AddColumns(
                new Paragraph("North America East (us-east-1): 80 active nodes operating at 99.992% uptime."),
                new Paragraph("Europe West (eu-west-1): 52 active nodes operating at 99.981% uptime.")
            )
            .AddCodeBlock(
                "kubectl get nodes --selector=node-role.kubernetes.io/worker -o wide\n" +
                "kubectl top nodes --sort-by=cpu",
                language: "bash")
        )
    )
    .AddPageBreak()
    .AddSection("Compliance and Security Posture", section => section
        .AddBorderedParagraph(
            "All cluster worker nodes have received the latest security kernel patches with zero unresolved critical CVEs.",
            label: "Security Clearance")
        .AddElement(new Paragraph("Critical compliance attestation passed across all regulated financial and healthcare workloads.")
            .WithHighlight("#fef08a")
            .WithAlignment(TextAlignment.Center))
        .AddParagraph("Key audit highlights:")
        .AddBulletList([
            "Automated vulnerability scanning enabled on all CI/CD pipelines.",
            "Zero critical CVE findings reported across container base images.",
            "Off-site backup snapshots verified through automated daily restore drills."
        ])
        .AddHorizontalSeparator()
        .AddParagraph("Mandatory remediation and action items:")
        .AddOrderedList([
            "Complete migration of legacy worker pools to kernel 6.12 LTS.",
            "Enable HTTP/3 protocol negotiation on primary ingress controllers.",
            "Execute annual disaster recovery simulation by mid-November 2026."
        ])
        .AddHyperlink(
            "View Full SOC2 Type II & ISO 27001 Compliance Dashboard",
            "https://compliance.internal.example.com/audits/2026-q3",
            tooltip: "Internal Compliance Portal")
    )
    // Injecting a custom element defined in the client project (Open/Closed Principle)
    .AddElement(new CalloutBox("Notice", "Next recurring quarterly audit is scheduled for December 15, 2026.").WithBorder("Schedule"))
    .Build();

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("✓ Document constructed in memory!");
Console.ResetColor();

string outputDirectory = Path.Combine(AppContext.BaseDirectory, "output");
var progressObserver = new ConsoleExportObserver();

// ---------------------------------------------------------------------------
// PART 2: Standard Out-of-the-Box Export (Strategy + Template Method + Observer)
// ---------------------------------------------------------------------------
Console.WriteLine("\n[2] Exporting with default MarkdownDocumentExporter (Strategy #1)...");

var markdownExporter = new MarkdownDocumentExporter();
markdownExporter.Attach(progressObserver);

var exportContext = new DocumentExportContext(markdownExporter);
string markdownPath = Path.Combine(outputDirectory, "cloud_infrastructure_audit.md");
exportContext.ExportToFile(document, markdownPath);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"✓ Markdown exported to: {markdownPath}");
Console.ResetColor();

Console.WriteLine("\n[3] Switching strategy at runtime to HtmlDocumentExporter (Strategy #2)...");

var htmlExporter = new HtmlDocumentExporter();
htmlExporter.Attach(progressObserver);

exportContext.SetStrategy(htmlExporter);
string htmlPath = Path.Combine(outputDirectory, "cloud_infrastructure_audit.html");
exportContext.ExportToFile(document, htmlPath);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"✓ HTML exported to: {htmlPath}");
Console.ResetColor();

// ---------------------------------------------------------------------------
// PART 3: Client Extensibility (Open/Closed Principle - Custom Exporter)
// ---------------------------------------------------------------------------
Console.WriteLine("\n[4] Switching strategy to custom client PlainTextDocumentExporter (OCP Extension)...");

var customTxtExporter = new PlainTextDocumentExporter();
customTxtExporter.Attach(progressObserver);

exportContext.SetStrategy(customTxtExporter);
string txtPath = Path.Combine(outputDirectory, "cloud_infrastructure_audit.txt");
exportContext.ExportToFile(document, txtPath);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"✓ Custom Plain Text exported to: {txtPath}");
Console.ResetColor();

Console.WriteLine("\nAll demonstrations completed successfully.");

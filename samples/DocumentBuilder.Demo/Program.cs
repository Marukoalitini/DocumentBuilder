using DocumentBuilder.Elements;
using DocumentBuilder.Exporters;
using DocBuilder = DocumentBuilder.Builders.DocumentBuilder;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("==========================================================");
Console.WriteLine("   DocumentBuilder - Sample Client Demonstration (.NET 10)");
Console.WriteLine("==========================================================");
Console.ResetColor();

// Construct a real-world enterprise infrastructure audit report
Console.WriteLine("\n[1] Constructing document using DocumentBuilder fluent API...");

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
        )
    )
    .AddSection("Compliance and Security Posture", section => section
        .AddParagraph("All cluster worker nodes have received the latest security kernel patches " +
                      "with zero unresolved high-severity Common Vulnerabilities and Exposures (CVEs). " +
                      "Strict mutual TLS 1.3 encryption is enforced across all internal microservice endpoints.")
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
        .AddParagraph("The next recurring quarterly review is scheduled for December 15, 2026.")
    )
    .Build();

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("✓ Document constructed successfully in memory!");
Console.ResetColor();

// Export the document using the out-of-the-box MarkdownDocumentExporter
Console.WriteLine("\n[2] Exporting document using MarkdownDocumentExporter...");

IDocumentExporter exporter = new MarkdownDocumentExporter();
string renderedContent = exporter.Export(document);

Console.WriteLine("\n--- Rendered Markdown (.md) Output ---");
Console.WriteLine(renderedContent);

// Save to an output file
string outputDirectory = Path.Combine(AppContext.BaseDirectory, "output");
string outputPath = Path.Combine(outputDirectory, "cloud_infrastructure_audit.md");

exporter.ExportToFile(document, outputPath);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"\n✓ Document exported to file successfully: {outputPath}");
Console.ResetColor();
Console.WriteLine("\nDemonstration completed successfully.");

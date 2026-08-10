using Microsoft.CodeAnalysis;

namespace Dhazel.Revertibles.Generators.Tests;

public class RevertibleObjectGeneratorTests
{
    [Fact]
    public void GeneratesExpectedMembers_ForPartialClass()
    {
        const string source = """
            using Dhazel.Revertibles;

            namespace MyApp;

            [RevertibleObject]
            public partial class Widget
            {
                public string? Name { get; set; }
            }
            """;

        var (output, diagnostics) = GeneratorTestHelper.RunGenerator(
            source, new RevertibleObjectGenerator());

        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));

        var generatedTree = output.SyntaxTrees
            .FirstOrDefault(t => t.FilePath.Contains("Widget.RevertibleObject.g.cs"));

        Assert.NotNull(generatedTree);

        var generatedText = generatedTree!.ToString();
        Assert.Contains("public void AcceptChanges(string? propertyName = null)", generatedText);
        Assert.Contains("public bool HasChanged(string? propertyName = null)", generatedText);
        Assert.Contains("public void Revert(string? propertyName = null)", generatedText);
        Assert.Contains("Revertible.Track(this, true)", generatedText);
    }

    [Fact]
    public void RespectsRequireManualAcceptChanges()
    {
        const string source = """
            using Dhazel.Revertibles;

            namespace MyApp;

            [RevertibleObject(AcceptPristineValues = false)]
            public partial class Widget
            {
                public string? Name { get; set; }
            }
            """;

        var (output, _) = GeneratorTestHelper.RunGenerator(source, new RevertibleObjectGenerator());

        var generatedText = output.SyntaxTrees
            .First(t => t.FilePath.Contains("Widget.RevertibleObject.g.cs"))
            .ToString();

        Assert.Contains("Revertible.Track(this, false)", generatedText);
    }

    [Fact]
    public void DoesNotGenerateRealMembers_WhenClassIsNotPartial()
    {
        const string source = """
            using Dhazel.Revertibles;

            namespace MyApp;

            [RevertibleObject]
            public class Widget
            {
                public string? Name { get; set; }
            }
            """;

        var (output, _) = GeneratorTestHelper.RunGenerator(source, new RevertibleObjectGenerator());

        var generatedText = output.SyntaxTrees
            .First(t => t.FilePath.Contains("Widget.RevertibleObject.g.cs"))
            .ToString();

        Assert.DoesNotContain("public void AcceptChanges", generatedText);
    }
}

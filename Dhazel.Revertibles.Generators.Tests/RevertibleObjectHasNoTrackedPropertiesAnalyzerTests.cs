using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Dhazel.Revertibles.Generators.Tests;

public class RevertibleObjectHasNoTrackedPropertiesAnalyzerTests
{
    [Fact]
    public async Task Warns_WhenNoPropertyHasRevertibleAttribute()
    {
        const string source = """
                              using Dhazel.Revertibles;

                              namespace MyApp;

                              [RevertibleObject]
                              public partial class {|#0:Widget|}
                              {
                                  public string Name { get; set; } = "";
                              }
                              """;

        var test = new CSharpAnalyzerTest<RevertibleObjectHasNoTrackedPropertiesAnalyzer, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net100,
            TestState =
            {
                Sources = { source },
                AdditionalReferences = { typeof(RevertibleObjectAttribute).Assembly },
            },
            ExpectedDiagnostics =
            {
                new DiagnosticResult(RevertibleObjectHasNoTrackedPropertiesAnalyzer.Rule)
                    .WithLocation(0)
                    .WithArguments("Widget"),
            },
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task NoWarning_WhenAtLeastOnePropertyHasRevertibleAttribute()
    {
        const string source = """
                              using Dhazel.Revertibles;

                              namespace MyApp;

                              [RevertibleObject]
                              public partial class Widget
                              {
                                  [Revertible]
                                  public string Name { get; set; } = "";

                                  public int UntrackedCount { get; set; }
                              }
                              """;

        var test = new CSharpAnalyzerTest<RevertibleObjectHasNoTrackedPropertiesAnalyzer, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net100,
            TestState =
            {
                Sources = { source },
                AdditionalReferences = { typeof(RevertibleObjectAttribute).Assembly }
            }
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task NoWarning_WhenClassHasNoRevertibleObjectAttribute()
    {
        const string source = """
                              namespace MyApp;

                              public class PlainWidget
                              {
                                  public string Name { get; set; } = "";
                              }
                              """;

        var test = new CSharpAnalyzerTest<RevertibleObjectHasNoTrackedPropertiesAnalyzer, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net100,
            TestState =
            {
                Sources = { source },
                AdditionalReferences = { typeof(RevertibleObjectAttribute).Assembly }
            }
        };

        await test.RunAsync();
    }
}
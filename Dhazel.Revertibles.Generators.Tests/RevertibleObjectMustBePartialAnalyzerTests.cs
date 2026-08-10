using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Dhazel.Revertibles.Generators.Tests;

public class RevertibleObjectMustBePartialAnalyzerTests
{
    [Fact]
    public async Task Error_WhenClassIsNotPartial()
    {
        const string source = """
            using Dhazel.Revertibles;

            namespace MyApp;

            [RevertibleObject]
            public class {|#0:Widget|}
            {
                [Revertible]
                public string Name { get; set; } = "";
            }
            """;

        var test = new CSharpAnalyzerTest<RevertibleObjectMustBePartialAnalyzer, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net100,
            TestState =
            {
                Sources = { source },
                AdditionalReferences = { typeof(RevertibleObjectAttribute).Assembly },
            },
            ExpectedDiagnostics =
            {
                new DiagnosticResult(RevertibleObjectMustBePartialAnalyzer.Rule)
                    .WithLocation(0)
                    .WithArguments("Widget"),
            },
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task NoDiagnostic_WhenClassIsPartial()
    {
        const string source = """
            using Dhazel.Revertibles;

            namespace MyApp;

            [RevertibleObject]
            public partial class Widget
            {
                [Revertible]
                public string Name { get; set; } = "";
            }
            """;

        var test = new CSharpAnalyzerTest<RevertibleObjectMustBePartialAnalyzer, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net100,
            TestState =
            {
                Sources = { source },
                AdditionalReferences = { typeof(RevertibleObjectAttribute).Assembly },
            }
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task NoDiagnostic_WhenClassHasNoRevertibleObjectAttribute()
    {
        const string source = """
            namespace MyApp;

            public class PlainWidget
            {
                public string Name { get; set; } = "";
            }
            """;

        var test = new CSharpAnalyzerTest<RevertibleObjectMustBePartialAnalyzer, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net100,
            TestState =
            {
                Sources = { source },
                AdditionalReferences = { typeof(RevertibleObjectAttribute).Assembly },
            }
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task NoDiagnostic_WhenNonPartialClassHasUnrelatedAttribute()
    {
        // Sanity check: a random other attribute shouldn't trigger REV001.
        const string source = """
            using System;

            namespace MyApp;

            public class ObsoleteAttribute2 : Attribute { }

            [ObsoleteAttribute2]
            public class Widget
            {
                public string Name { get; set; } = "";
            }
            """;

        var test = new CSharpAnalyzerTest<RevertibleObjectMustBePartialAnalyzer, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net100,
            TestState =
            {
                Sources = { source },
                AdditionalReferences = { typeof(RevertibleObjectAttribute).Assembly },
            }
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task Error_FiresOncePerNonPartialClass_NotOncePerAttributeUsage()
    {
        // Two separate non-partial classes, each decorated -> two diagnostics, not one.
        const string source = """
            using Dhazel.Revertibles;

            namespace MyApp;

            [RevertibleObject]
            public class {|#0:WidgetA|}
            {
                [Revertible]
                public string Name { get; set; } = "";
            }

            [RevertibleObject]
            public class {|#1:WidgetB|}
            {
                [Revertible]
                public string Title { get; set; } = "";
            }
            """;

        var test = new CSharpAnalyzerTest<RevertibleObjectMustBePartialAnalyzer, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net100,
            TestState =
            {
                Sources = { source },
                AdditionalReferences = { typeof(RevertibleObjectAttribute).Assembly },
            },
            ExpectedDiagnostics =
            {
                new DiagnosticResult(RevertibleObjectMustBePartialAnalyzer.Rule).WithLocation(0).WithArguments("WidgetA"),
                new DiagnosticResult(RevertibleObjectMustBePartialAnalyzer.Rule).WithLocation(1).WithArguments("WidgetB"),
            },
        };

        await test.RunAsync();
    }
}
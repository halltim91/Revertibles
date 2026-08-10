using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Dhazel.Revertibles.Generators;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RevertibleObjectMustBePartialAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor Rule = new(
        id: "REV001",
        title: "RevertibleObject class must be partial",
        messageFormat: "Class '{0}' is marked with [RevertibleObject] but is not declared 'partial'",
        category: "Dhazel.Revertibles",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var symbol = (INamedTypeSymbol)context.Symbol;

        var hasAttribute = symbol.GetAttributes()
            .Any(a => a.AttributeClass?.ToDisplayString() == "Dhazel.Revertibles.RevertibleObjectAttribute");

        if (!hasAttribute) return;

        var isPartial = symbol.DeclaringSyntaxReferences
            .Select(r => r.GetSyntax())
            .OfType<ClassDeclarationSyntax>()
            .Any(c => c.Modifiers.Any(SyntaxKind.PartialKeyword));

        if (!isPartial)
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, symbol.Locations[0], symbol.Name));
        }
    }
}
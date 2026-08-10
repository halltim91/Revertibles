using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Dhazel.Revertibles.Generators;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RevertibleObjectHasNoTrackedPropertiesAnalyzer : DiagnosticAnalyzer
{
    private const string RevertibleObjectAttributeName = "Dhazel.Revertibles.RevertibleObjectAttribute";
    private const string RevertibleAttributeName = "Dhazel.Revertibles.RevertibleAttribute";

    public static readonly DiagnosticDescriptor Rule = new(
        id: "REV002",
        title: "RevertibleObject class has no tracked properties",
        messageFormat: "Class '{0}' is marked with [RevertibleObject] but has no properties marked with [Revertible]",
        category: "Dhazel.Revertibles",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "A [RevertibleObject] class with no [Revertible] properties will never report changes. " +
                     "Either add [Revertible] to at least one writable property, or remove the attribute.");

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

        var revertibleObjectAttribute = symbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == RevertibleObjectAttributeName);

        if (revertibleObjectAttribute is null)
        {
            return;
        }

        var hasTrackedProperty = symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => !p.IsReadOnly && !p.IsWriteOnly && p.SetMethod is not null)
            .Any(p => p.GetAttributes()
                .Any(a => a.AttributeClass?.ToDisplayString() == RevertibleAttributeName));

        if (!hasTrackedProperty)
        {
            var location = symbol.DeclaringSyntaxReferences
                .Select(r => r.GetSyntax())
                .OfType<ClassDeclarationSyntax>()
                .Select(c => c.Identifier.GetLocation())
                .FirstOrDefault() ?? symbol.Locations.FirstOrDefault() ?? Location.None;

            context.ReportDiagnostic(Diagnostic.Create(Rule, location, symbol.Name));
        }
    }
}
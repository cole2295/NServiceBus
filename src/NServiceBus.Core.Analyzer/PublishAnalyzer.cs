using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NServiceBus.Core.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PublishAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor diagnostic = new DiagnosticDescriptor(
            "NSB001",
            "TBD",
            "TBD",
            "TBD",
            DiagnosticSeverity.Error,
            true,
            "TBD");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(diagnostic);

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
            context.RegisterSyntaxNodeAction(AnalyzeMethodInvocations, SyntaxKind.InvocationExpression);
        }

        void AnalyzeMethodInvocations(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            if (invocation == null)
            {
                return;
            }

            //TODO: directly register on the ExpressionStatement syntax instead?
            if (invocation.Parent is ExpressionStatementSyntax)
            {
                //TODO: check identifier name before digging deeper into the semantic model

                // method is invoked without using return value
                var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation.Expression);
                var symbol = symbolInfo.Symbol as IMethodSymbol;
                if (symbol?.ToString() == "NServiceBus.IPipelineContext.Publish(object, NServiceBus.PublishOptions)")
                {
                    var location = invocation.GetLocation();
                    context.ReportDiagnostic(Diagnostic.Create(
                        diagnostic, location));
                }
            }
        }



        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            if (context.Symbol.Name == "Publish") // TODO: replace this nonsense with an actual implementation
            {
                context.ReportDiagnostic(Diagnostic.Create(diagnostic, context.Symbol.Locations[0], context.Symbol.Name));
            }
        }
    }
}

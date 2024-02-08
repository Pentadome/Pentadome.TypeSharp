using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pentadome.TypeSharp.Models;

namespace Pentadome.TypeSharp.Extensions;

internal static class SemanticModelExtensions
{
    internal static ArrayValue<T?>[] GetConstantArray<T>(
        this SemanticModel @this,
        ExpressionSyntax arrayExpression
    )
    {
        if (arrayExpression is CollectionExpressionSyntax collectionExpressionSyntax)
        {
            return GetFromCollectionExpressionSyntax<T>(@this, collectionExpressionSyntax);
        }
        throw new NotImplementedException();
    }

    private static ArrayValue<T?>[] GetFromCollectionExpressionSyntax<T>(
        SemanticModel @this,
        CollectionExpressionSyntax arrayExpression
    )
    {
        var values = new ArrayValue<T?>[arrayExpression.Elements.Count];
        for (var index = 0; index < arrayExpression.Elements.Count; index++)
        {
            var element = arrayExpression.Elements[index];
            if (element is ExpressionElementSyntax expressionElementSyntax)
            {
                var rawValue = @this.GetConstantValue(expressionElementSyntax.Expression);
                var value = rawValue is { HasValue: true, Value: T } ? (T)rawValue.Value : default;
                values[index] = new ArrayValue<T?>(
                    value,
                    expressionElementSyntax.Expression.GetLocation()
                );
                continue;
            }

            throw new NotImplementedException();
        }

        return values;
    }
}
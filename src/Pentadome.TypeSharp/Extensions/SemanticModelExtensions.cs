using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pentadome.TypeSharp.Models;

namespace Pentadome.TypeSharp.Extensions;

internal static class SemanticModelExtensions
{
    internal static T? GetConstantOrDefault<T>(
        this SemanticModel @this,
        ExpressionSyntax expression
    )
    {
        var constantValue = @this.GetConstantValue(expression);
        return constantValue.HasValue ? (T?)constantValue.Value : default;
    }

    internal static ArrayValue<T?>[] GetConstantArray<T>(
        this SemanticModel @this,
        ExpressionSyntax arrayExpression
    )
    {
        return arrayExpression switch
        {
            CollectionExpressionSyntax collectionExpressionSyntax
                => GetConstantArray<T>(@this, collectionExpressionSyntax),
            ArrayCreationExpressionSyntax arrayCreationExpressionSyntax
                => GetConstantArray<T>(@this, arrayCreationExpressionSyntax),
            ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpressionSyntax
                => GetConstantArray<T>(@this, implicitArrayCreationExpressionSyntax),
            _ => throw new NotSupportedException()
        };
    }

    private static ArrayValue<T?>[] GetConstantArray<T>(
        SemanticModel semanticModel,
        CollectionExpressionSyntax collectionExpression
    )
    {
        var values = new ArrayValue<T?>[collectionExpression.Elements.Count];
        for (var index = 0; index < collectionExpression.Elements.Count; index++)
        {
            var element = collectionExpression.Elements[index];
            if (element is not ExpressionElementSyntax expressionElementSyntax)
                throw new NotSupportedException();

            values[index] = new ArrayValue<T?>(
                semanticModel.GetConstantOrDefault<T>(expressionElementSyntax.Expression),
                expressionElementSyntax.Expression.GetLocation()
            );
        }

        return values;
    }

    private static ArrayValue<T?>[] GetConstantArray<T>(
        SemanticModel semanticModel,
        ArrayCreationExpressionSyntax arrayCreationExpression
    )
    {
        var initializer = arrayCreationExpression.Initializer;
        return GetConstantArray<T>(semanticModel, initializer);
    }

    private static ArrayValue<T?>[] GetConstantArray<T>(
        SemanticModel semanticModel,
        ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpression
    )
    {
        var initializer = implicitArrayCreationExpression.Initializer;
        return GetConstantArray<T>(semanticModel, initializer);
    }

    private static ArrayValue<T?>[] GetConstantArray<T>(
        SemanticModel semanticModel,
        InitializerExpressionSyntax? initializer
    )
    {
        var valueExpressions = initializer?.Expressions;
        if (!valueExpressions.HasValue || valueExpressions.Value.Count == 0)
            return [];

        var values = new ArrayValue<T?>[valueExpressions.Value.Count];
        for (var index = 0; index < valueExpressions.Value.Count; index++)
        {
            var valueExpression = valueExpressions.Value[index];
            values[index] = new ArrayValue<T?>(
                semanticModel.GetConstantOrDefault<T>(valueExpression),
                valueExpression.GetLocation()
            );
        }

        return values;
    }
}

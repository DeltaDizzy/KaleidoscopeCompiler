using System;
using KaleidoscopeCompiler.AST;

namespace KaleidoscopeCompiler;

/// <summary>
/// Parses the Lexer's token sequence into AST nodes
/// </summary>
public class Parser
{
    ExpressionAST ParseNumberExpression(Token token) {
        if (token.Type != TokenType.Literal)
        {
            // error
        }
        if (!double.TryParse(token.Value, out double val))
        {
            // error
        }
        return new NumberExpressionAST {
            Value = val
        };
    }

    (ExpressionAST, IEnumerable<Token>) ParseParenthesesExpression(IEnumerable<Token> tokens) {
        // parse the expression inside the parentheses
        var (expression, expressionTokenCount) = ParseExpression(tokens.Skip(1));
        if (tokens.Skip(1+expressionTokenCount).First().Type != TokenType.EndGrouping)
        {
            // error
        }
        return (expression, tokens.Skip(2 + expressionTokenCount));
    }

    (ExpressionAST, int) ParseExpression(IEnumerable<Token> tokens) {
        var lhs = ParsePrimary();
        return ParseBinOpRHS(0, lhs);
    }

    private (ExpressionAST, int) ParseBinOpRHS(int minimumPrecedence, ExpressionAST lhs)
    {
        throw new NotImplementedException();
    }

    private ExpressionAST ParsePrimary(Token token)
    {
        return token.Type switch
        {
            TokenType.Identifier => throw new NotImplementedException(),
            TokenType.Literal => ParseNumberExpression(token),
            TokenType.Operator => throw new NotImplementedException(),
            TokenType.Delimiter => throw new NotImplementedException(),
            TokenType.StartGrouping => throw new NotImplementedException(),
            TokenType.EndGrouping => throw new NotImplementedException(),
            TokenType.DefineFunction => throw new NotImplementedException(),
            TokenType.ControlFlow => throw new NotImplementedException(),
            TokenType.ExternDeclaration => throw new NotImplementedException(),
            TokenType.Indent => throw new NotImplementedException(),
            TokenType.Dedent => throw new NotImplementedException(),
        };
    }
}

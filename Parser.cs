using System;
using KaleidoscopeCompiler.AST;

namespace KaleidoscopeCompiler;

/// <summary>
/// Parses the Lexer's token sequence into AST nodes
/// </summary>
public class Parser
{
    private static readonly Dictionary<char, int> BinaryOperatorPrecedence = [];
    // based on first token, invoke a specialized expression parser
    // if it stops matching, bail and die
    public static List<AST.AST> ParseAST(List<Token> tokens)
    {
        List<AST.AST> ast = [];
        switch (tokens[0].Type)
        {
            case TokenType.DefineFunction:
                FunctionAST? func = ParseDefinition(tokens);
                if (func is null)
                {
                    // die
                    return null;
                }
                ast.Add(func);
                break;
            default:
                break;
        }
        return ast;
    }

    private static FunctionAST? ParseDefinition(List<Token> tokensIn) {
        // eat the 'def' token
        var tokens = tokensIn.Skip(1);
        (PrototypeAST? prototype, tokens) = ParsePrototype(tokens);
        if (prototype is null)
        {
            // bail
            return null;
        }
        if (tokens is null)
        {
            // bail
            return null;
        }

        (ExpressionAST? expression, tokens) = ParseExpression(tokens);

        if (expression is null)
        {
            return null;
        }

        return new FunctionAST {
            Proto = prototype,
            Body = expression
        };
    }

    private static (ExpressionAST?, IEnumerable<Token>) ParseExpression(IEnumerable<Token> tokens)
    {
        (ExpressionAST? leftHandSide, tokens) = ParsePrimary(tokens);
        if (leftHandSide is null)
        {
            return (null, tokens);
        }
        return ParseBinaryOpRhs(0, tokens);
    }

    private static (ExpressionAST?, IEnumerable<Token>) ParseBinaryOpRhs(int v, IEnumerable<Token> tokens)
    {
        throw new NotImplementedException();
    }

    private static (ExpressionAST?, IEnumerable<Token>) ParsePrimary(IEnumerable<Token> tokens)
    {
        return tokens.First().Type switch {
            TokenType.Identifier => ParseIdentifierExpression(tokens),
            TokenType.StartGrouping => ParseParensExpression(tokens),
            TokenType.Literal => ParseNumberExpression(tokens)
        };
    }

    private static (NumberExpressionAST?, IEnumerable<Token>?) ParseNumberExpression(IEnumerable<Token> tokens)
    {
        if(!double.TryParse(tokens.First().Value, out double val)) {
            // error
            return (null, null);
        }
        return (new NumberExpressionAST { Value = val }, tokens.Skip(1));
    }

    private static (ExpressionAST?, IEnumerable<Token>) ParseParensExpression(IEnumerable<Token> tokens)
    {
        tokens = tokens.Skip(1); // eat (
        (ExpressionAST? expression, tokens) = ParseExpression(tokens);
        if (expression is null)
        {
            return (null, tokens);
        }
        if (tokens.First().Type is not TokenType.EndGrouping)
        {
            return (null, tokens);
        }
        return (expression, tokens.Skip(1));
    }

    private static (ExpressionAST, IEnumerable<Token>) ParseIdentifierExpression(IEnumerable<Token> tokens)
    {
        string idName = tokens.First().Value;
        tokens = tokens.Skip(1);
        // is this actually a call
        if (tokens.First().Type is not TokenType.StartGrouping)
        {
            return (new VariableExpressionAST { Name = idName }, tokens.Skip(1));
        }
        // this is a call
        tokens = tokens.Skip(1);
        List<ExpressionAST> args = [];
        if (tokens.First().Type is not TokenType.EndGrouping)
        {
            while (true)
            {
                (ExpressionAST? arg, tokens) = ParseExpression(tokens);
                if (arg is null)
                {
                    return (null, tokens);
                }
                args.Add(arg);
                if (tokens.First().Type is TokenType.EndGrouping)
                {
                    break;
                }
                // add comma
            }
            tokens = tokens.Skip(1); // eat )
        }
        return (
            new CallExpressionAST {
                CalledFunction = idName,
                Arguments = args
            },
            tokens
        );
    }

    private static (PrototypeAST? prototype, IEnumerable<Token> remainingTokens) ParsePrototype(IEnumerable<Token> tokens)
    {
        if (tokens.First().Type is not TokenType.Identifier)
        {
            // error: no name
            return (null, tokens);
        }
        string funcName = tokens.First().Value;
        
        // eat identifier
        tokens = tokens.Skip(1);

        // next is (
        if (tokens.First().Type is not TokenType.StartGrouping)
        {
            // error
            return (null, tokens);
        }
        List<string> argNames = [];
        while (tokens.First().Type is TokenType.Identifier)
        {
            argNames.Add(tokens.First().Value);
            tokens = tokens.Skip(1);
        }
        if (tokens.First().Type is not TokenType.EndGrouping) {
            // error
            return (null, tokens);
        }
        tokens = tokens.Skip(1);
        return (
            new PrototypeAST {
                FunctionName = funcName,
                Arguments = argNames
            },
            tokens
        );
    }
}
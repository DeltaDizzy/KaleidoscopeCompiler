namespace KaleidoscopeCompiler.AST;

public record class ExpressionAST {}

public record class NumberExpressionAST : ExpressionAST {
    public required double Value { get; init; }
}

public record class VariableExpressionAST : ExpressionAST {
    public required string Name { get; init; }
}

public record class BinaryExpressionAST() : ExpressionAST {
    public required string OpCode { get; init; }
    public required ExpressionAST Lhs { get; init; }
    public required ExpressionAST Rhs { get; init; }
}

public record class CallExpressionAST : ExpressionAST {
    public required string CalledFunction { get; init; }
    public required List<ExpressionAST> Arguments { get; init; }
}

public record class PrototypeAST {
    public required string FunctionName { get; init; }
    public required List<string> Arguments { get; init; }
}

public record class FunctionAST {
    public required PrototypeAST Proto { get; init; }
    public required ExpressionAST Body { get; init; }
}

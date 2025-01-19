namespace KaleidoscopeCompiler;

public enum TokenType {
    Identifier,
    Literal,
    Operator,
    Delimiter,
    StartGrouping,
    EndGrouping,
    DefineFunction,
    ControlFlow,
    ExternDeclaration
}

public readonly record struct Token(TokenType Type, string Value);

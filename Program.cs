﻿using KaleidoscopeCompiler;

FileInfo testFile = new(@"/home/emily/projects/KaleidoscopeCompiler/test2.ks");
Lexer lexer = new();
lexer.LoadFile(testFile);
List<Token> tokens = lexer.GetAllTokens();
foreach (Token token in tokens)
{
    Console.WriteLine(token);
}

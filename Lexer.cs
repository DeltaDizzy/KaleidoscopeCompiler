using System;
using System.Text.RegularExpressions;

namespace KaleidoscopeCompiler;

public class Lexer
{
    private Regex functionDef = new(@"(def) ([\w]+)(\()(?:(?'arg'\w+) *)+?(\))");
    private Regex controlFlowIf = new(@"(?:[^\S\r\n]+)(if) ([\w]+) (<|>|is|not|\+|-|\*|\\)* (\d+) then");
    private Regex functionCall = new(@"(\w+)(\()(\w+|\.?\d+)(\))");
    private List<Token> tokens = [];
    private string[] lines = [];

    public List<Token> GetAllTokens() {
        if (lines.Length is 0)
        {
            throw new Exception("A file must be loaded before Tokens can be enumerated.");
        }
        foreach (string line in lines)
        {
            if (MatchFunctionDef(line)) continue;
            //if (MatchControlFlow(line)) continue;
            if (MatchFunctionCall(line)) continue;
            // TODO: Parse extern prototypes
        }
        return tokens;
    }

    private bool MatchFunctionDef(string line) {
        var match = functionDef.Match(line);
        if (!match.Success)
        {
            return false;
        }
        // find all tokens in order
        // this MUST be a valid definition because we have matches to begin with
        tokens.Add(new Token(TokenType.DefineFunction, match.Groups[1].Value));
        tokens.Add(new Token(TokenType.Identifier, match.Groups[2].Value));
        tokens.Add(new Token(TokenType.StartGrouping, match.Groups[3].Value));
        // args will be folded into the last group
        foreach (Capture arg in match.Groups[5].Captures)
        {
            tokens.Add(new Token(TokenType.Identifier, arg.Value));
        }
        tokens.Add(new Token(TokenType.EndGrouping, match.Groups[4].Value));

        return true;
    }

    private bool MatchControlFlow(string line) {
        // regex will fail if it is just an else, so lets special case that
        if (line.Trim() is "else") {
            tokens.Add(new Token(TokenType.ControlFlow, "else"));
            return true;
        }
        var match = controlFlowIf.Match(line);
        if (!match.Success)
        {
            return false;
        }

        // only support if statements of the form '[identifier] [comaprison operator] [literal]'
        // wanna add literal v. literal, identifier v. identifier later
        tokens.Add(new Token(TokenType.ControlFlow, match.Groups[1].Value));
        tokens.Add(new Token(TokenType.Identifier, match.Groups[2].Value));
        tokens.Add(new Token(TokenType.Operator, match.Groups[3].Value));
        tokens.Add(new Token(TokenType.Literal, match.Groups[4].Value));
        return true;
    }

    private bool MatchFunctionCall(string line) {
        var match = functionCall.Match(line);
        if (line.Split(' ').Contains("def") || line.Split(' ').Contains("extern") || !match.Success)
        {
            return false;
        }

        tokens.Add(new Token(TokenType.Identifier, match.Groups[1].Value));
        tokens.Add(new Token(TokenType.StartGrouping, match.Groups[2].Value));
        if (double.TryParse(match.Groups[3].Value, out double _)) {
            // this is a literal
            tokens.Add(new Token(TokenType.Literal, match.Groups[3].Value));
        } else {
            // this is a variable
            tokens.Add(new Token(TokenType.Literal, match.Groups[3].Value));
        }
        tokens.Add(new Token(TokenType.EndGrouping, match.Groups[4].Value));

        return true;
    }

    public void LoadFile(FileInfo file) {
       lines = File.ReadAllLines(file.FullName);
    }
}

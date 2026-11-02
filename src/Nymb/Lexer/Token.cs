namespace Nymb.Lexer;

internal record Token(TokenType Type, string Value, string RawValue, SourceSpan Span);

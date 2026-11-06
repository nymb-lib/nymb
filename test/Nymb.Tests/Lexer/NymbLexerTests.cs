using Nymb.Lexer;

namespace Nymb.Tests.Lexer;

public class NymbLexerTests
{
    [Fact]
    public void Empty_input_produces_no_tokens()
    {
        Assert.Empty(new NymbLexer("").Tokenize());
    }

    [Fact]
    public void Input_is_split_into_words_whitespace_delimiters_and_parentheses()
    {
        var tokens = new NymbLexer("(foo = bar)\n", ['=']).Tokenize();

        Assert.Collection(
            tokens,
            token => AssertToken(token, TokenType.OpenParenthesis, "(", "("),
            token => AssertToken(token, TokenType.BareToken, "foo", "foo"),
            token => AssertToken(token, TokenType.Whitespace, " ", " "),
            token => AssertToken(token, TokenType.Delimiter, "=", "="),
            token => AssertToken(token, TokenType.Whitespace, " ", " "),
            token => AssertToken(token, TokenType.BareToken, "bar", "bar"),
            token => AssertToken(token, TokenType.CloseParenthesis, ")", ")"),
            token => AssertToken(token, TokenType.Whitespace, "\n", "\n"));
    }

    [Fact]
    public void Token_spans_track_offsets_and_lines_across_carriage_return_line_feed()
    {
        var tokens = new NymbLexer("one\r\ntwo").Tokenize();

        var startOfFirstToken = new SourcePosition(0, 1, 1);
        var endOfFirstToken = new SourcePosition(3, 1, 4);
        var firstTokenSpan = new SourceSpan(startOfFirstToken, endOfFirstToken);

        var startOfLineBreak = new SourcePosition(3, 1, 4);
        var endOfLineBreak = new SourcePosition(5, 2, 1);
        var lineBreakSpan = new SourceSpan(startOfLineBreak, endOfLineBreak);

        var startOfSecondToken = new SourcePosition(5, 2, 1);
        var endOfSecondToken = new SourcePosition(8, 2, 4);
        var secondTokenSpan = new SourceSpan(startOfSecondToken, endOfSecondToken);

        Assert.Equal(firstTokenSpan, tokens[0].Span);
        Assert.Equal(lineBreakSpan, tokens[1].Span);
        Assert.Equal(secondTokenSpan, tokens[2].Span);
        Assert.Equal("\r\n", tokens[1].Value);
    }

    [Fact]
    public void Lexer_is_deterministic_by_default()
    {
        var lexer = new NymbLexer("word (value)", [';']);

        Assert.Equal(lexer.Tokenize(), lexer.Tokenize());
    }

    private static void AssertToken(Token token, TokenType type, string value, string rawValue)
    {
        Assert.Equal(type, token.Type);
        Assert.Equal(value, token.Value);
        Assert.Equal(rawValue, token.RawValue);
    }
}

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
    public void Input_is_split_into_bare_tokens_whitespaces_delimiters_and_parentheses()
    {
        var lexerConfiguration = NymbLexerConfiguration.Default with { Delimiters = ['='] };
        var tokens = new NymbLexer("(foo = bar)\n", lexerConfiguration).Tokenize();

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
    public void Lexer_is_deterministic_by_default()
    {
        var lexerConfiguration = NymbLexerConfiguration.Default with { Delimiters = [';'] };
        var lexer = new NymbLexer("word (value)", lexerConfiguration);

        Assert.Equal(lexer.Tokenize(), lexer.Tokenize());
    }

    private static void AssertToken(Token token, TokenType type, string value, string rawValue)
    {
        Assert.Equal(type, token.Type);
        Assert.Equal(value, token.Value);
        Assert.Equal(rawValue, token.RawValue);
    }
}

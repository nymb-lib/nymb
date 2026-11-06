using System.Collections;
using Nymb.Lexer;

namespace Nymb.Tests.Lexer;

public class NymbLexerControlCharactersTests
{
    [Theory]
    [InlineData("\n", "\n")]
    [InlineData("\r", "\r")]
    [InlineData("\r\n", "\r\n")]
    public void Line_break_characters_are_whitespace_tokens(string lineBreak, string expectedValue)
    {
        var tokens = new NymbLexer(lineBreak).Tokenize();
        var whitespaceToken = Assert.Single(tokens);

        AssertToken(whitespaceToken, TokenType.Whitespace, expectedValue, expectedValue);
    }

    [Fact]
    public void Tab_character_is_a_whitespace_token()
    {
        var tokens = new NymbLexer("\t").Tokenize();
        var whitespaceToken = Assert.Single(tokens);

        AssertToken(whitespaceToken, TokenType.Whitespace, "\t", "\t");
    }

    [Fact]
    public void Vertical_tab_character_is_a_whitespace_token()
    {
        var tokens = new NymbLexer("\v").Tokenize();
        var whitespaceToken = Assert.Single(tokens);

        AssertToken(whitespaceToken, TokenType.Whitespace, "\v", "\v");
    }

    [Fact]
    public void Form_feed_character_is_a_whitespace_token()
    {
        var tokens = new NymbLexer("\f").Tokenize();
        var whitespaceToken = Assert.Single(tokens);

        AssertToken(whitespaceToken, TokenType.Whitespace, "\f", "\f");
    }

    [Theory]
    [ClassData(typeof(NonWhitespaceControlCharactersData))]
    public void Non_whitespace_control_characters_are_rejected(char controlCharacter, string codePoint)
    {
        var input = controlCharacter.ToString();
        var formatException = Assert.Throws<FormatException>(() => new NymbLexer(input).Tokenize());

        Assert.True(
            formatException.Message.StartsWith("Not allowed control character", StringComparison.Ordinal),
            $"Expected {codePoint} to be rejected, but got: {formatException.Message}");
    }

    private static void AssertToken(Token token, TokenType type, string value, string rawValue)
    {
        Assert.Equal(type, token.Type);
        Assert.Equal(value, token.Value);
        Assert.Equal(rawValue, token.RawValue);
    }
}

file sealed class NonWhitespaceControlCharactersData : IEnumerable<TheoryDataRow<char, string>>
{
    public IEnumerator<TheoryDataRow<char, string>> GetEnumerator()
    {
        var nonWhitespaceAsciiControlCharacters = Enumerable.Range(0, 128)
            .Select(value => (char)value)
            .Where(char.IsControl)
            .Where(controlCharacter => !char.IsWhiteSpace(controlCharacter));

        var nonWhitespaceUnicodeControlCharacters = Enumerable.Range(0x80, 0x20)
            .Select(value => (char)value)
            .Where(char.IsControl)
            .Where(controlCharacter => !char.IsWhiteSpace(controlCharacter));

        var controlCharacters = nonWhitespaceAsciiControlCharacters.Concat(nonWhitespaceUnicodeControlCharacters);
        foreach (var controlCharacter in controlCharacters)
            yield return new TheoryDataRow<char, string>(controlCharacter, $"U+{(int)controlCharacter:X4}");
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

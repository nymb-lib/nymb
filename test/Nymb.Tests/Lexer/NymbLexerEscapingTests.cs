using Nymb.Lexer;

namespace Nymb.Tests.Lexer;

public class NymbLexerEscapingTests
{
    [Fact]
    public void An_escaped_quote_can_appear_inside_a_quoted_string()
    {
        const string input = """
                             "say -> \" <- hello"
                             """;

        const string expectedValue = """
                                     say -> " <- hello
                                     """;

        var tokens = new NymbLexer(input).Tokenize();
        var token = Assert.Single(tokens);

        Assert.Equal(expectedValue, token.Value);
        Assert.Equal(input, token.RawValue);
    }

    [Fact]
    public void An_escaped_backslash_becomes_one_literal_backslash()
    {
        const string input = """
                             "C:\\temp"
                             """;
        var token = Assert.Single(new NymbLexer(input).Tokenize());

        Assert.Equal("""C:\temp""", token.Value);
        Assert.Equal(input, token.RawValue);
    }

    [Theory]
    [InlineData("""\t""", "\t")]
    [InlineData("""\r""", "\r")]
    [InlineData("""\n""", "\n")]
    [InlineData("""\a""", "\a")]
    [InlineData("""\b""", "\b")]
    [InlineData("""\f""", "\f")]
    [InlineData("""\v""", "\v")]
    [InlineData("""\0""", "\0")]
    public void An_allowed_backslash_sequence_represents_a_control_character(string escape, string expectedCharacter)
    {
        var source = $"""
                      "before{escape}after"
                      """;
        var token = Assert.Single(new NymbLexer(source).Tokenize());

        Assert.Equal($"before{expectedCharacter}after", token.Value);
        Assert.Equal(source, token.RawValue);
    }

    [Fact]
    public void An_escaped_single_quote_can_appear_inside_a_single_quoted_string()
    {
        const string input = """
                             'can\'t'
                             """;
        var token = Assert.Single(new NymbLexer(input).Tokenize());

        Assert.Equal("can't", token.Value);
        Assert.Equal(input, token.RawValue);
    }

    [Fact]
    public void A_literal_line_break_inside_a_quoted_string_is_preserved()
    {
        // A real line break is different from the two-character escape sequence \n.
        const string source = """
                              "first
                              second"
                              """;
        const string expectedValue = """
                                     first
                                     second
                                     """;
        var token = Assert.Single(new NymbLexer(source).Tokenize());

        Assert.Equal(expectedValue, token.Value);
        Assert.Equal(source, token.RawValue);
    }
}

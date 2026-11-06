using Nymb.Lexer;

namespace Nymb.Tests.Lexer;

public class NymbLexerEscapingTests
{
    [Fact]
    public void An_escaped_quote_can_appear_inside_a_quoted_string()
    {
        // The backslash says that this quote is content, not the end of the string.
        const string input = """
                             "say \"hello\""
                             
                             """;
        
        const string expectedValue = """
                                     say "hello"
                                     """;
        
        var tokens = new NymbLexer(input).Tokenize();
        var token = Assert.Single(tokens);

        Assert.Equal(expectedValue, token.Value);
        Assert.Equal(input, token.RawValue);
    }

    [Fact]
    public void An_escaped_backslash_becomes_one_literal_backslash()
    {
        // Two source backslashes are needed to represent one backslash in the value.
        const string input = """
                             "C:\\temp"
                             """;
        var token = Assert.Single(new NymbLexer(input).Tokenize());

        Assert.Equal("""C:\temp""", token.Value);
        Assert.Equal(input, token.RawValue);
    }

    [Theory]
    [InlineData("""\n""", "\n")]
    [InlineData("""\r""", "\r")]
    [InlineData("""\t""", "\t")]
    [InlineData("""\0""", "\0")]
    public void A_backslash_sequence_represents_a_control_character(string escape, string expectedCharacter)
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
        // The same idea works when the string is delimited by single quotes.
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

    [Theory]
    [InlineData(
        """
        "unterminated
        """,
        "Unterminated quoted string at line 1, column 1.")]
    [InlineData(
        """
        'valid' "bad\q"
        """,
        """Invalid escape sequence '\q' at line 1, column 13.""")]
    [InlineData(
        """
        'trailing\
        """,
        "Unterminated escape sequence at line 1, column 10.")]
    public void Invalid_or_incomplete_quoted_text_reports_its_source_location(string input, string diagnostic)
    {
        var exception = Assert.Throws<FormatException>(() => new NymbLexer(input).Tokenize());

        Assert.Contains(diagnostic, exception.Message);
        Assert.Contains('↑', exception.Message);
    }
}

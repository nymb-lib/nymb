using System.Text;

namespace Nymb.Lexer;

internal sealed class NymbLexer
{
    private readonly HashSet<char> _delimiters;
    private readonly string _input;

    public NymbLexer(string input, IEnumerable<char>? delimiters = null)
    {
        ArgumentNullException.ThrowIfNull(input);

        _input = input;
        _delimiters = delimiters is null ? [] : [.. delimiters];
    }

    public List<Token> Tokenize()
    {
        return new TokenizationRun(_input, _delimiters).Tokenize();
    }
}

file class TokenizationRun(string input, HashSet<char> delimiters)
{
    private int _column = 1;
    private int _line = 1;
    private int _offset;

    private SourcePosition CurrentPosition => new(_offset, _line, _column);

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (_offset < input.Length)
        {
            if (char.IsWhiteSpace(input[_offset]))
            {
                tokens.Add(ReadWhitespace());
                continue;
            }

            var start = CurrentPosition;
            var character = input[_offset];

            switch (character)
            {
                case '(':
                    tokens.Add(ReadSingleCharacter(TokenType.OpenParenthesis));
                    break;
                case ')':
                    tokens.Add(ReadSingleCharacter(TokenType.CloseParenthesis));
                    break;
                case '\'':
                case '"':
                    tokens.Add(ReadQuotedText());
                    break;
                default:
                    tokens.Add(delimiters.Contains(character)
                        ? ReadSingleCharacter(TokenType.Delimiter)
                        : ReadWord(start));
                    break;
            }
        }

        return tokens;
    }

    private Token ReadWhitespace()
    {
        var start = CurrentPosition;
        var valueStart = _offset;

        while (_offset < input.Length && char.IsWhiteSpace(input[_offset]))
            Advance();

        var value = input[valueStart.._offset];
        return new Token(TokenType.Whitespace, value, value, new SourceSpan(start, CurrentPosition));
    }

    private Token ReadSingleCharacter(TokenType type)
    {
        var start = CurrentPosition;
        var value = input[_offset].ToString();
        Advance();

        return new Token(type, value, value, new SourceSpan(start, CurrentPosition));
    }

    private Token ReadQuotedText()
    {
        var start = CurrentPosition;
        var quote = input[_offset];
        Advance();
        var value = new StringBuilder();

        while (_offset < input.Length)
        {
            var character = input[_offset];
            if (character == quote)
            {
                Advance();
                var span = new SourceSpan(start, CurrentPosition);
                var rawValue = input[span.Start.Offset..span.End.Offset];
                return new Token(TokenType.StringLiteral, value.ToString(), rawValue, span);
            }

            if (character != '\\')
            {
                var characterStart = _offset;
                Advance();
                value.Append(input.AsSpan(characterStart, _offset - characterStart));
                continue;
            }

            var escapeStart = CurrentPosition;
            Advance();

            if (_offset == input.Length)
            {
                var escapeSpan = new SourceSpan(escapeStart, CurrentPosition);
                throw new FormatException(FormatDiagnostic("Unterminated escape sequence", escapeSpan));
            }

            var escapedCharacter = input[_offset];
            Advance();
            var escapedValue = escapedCharacter switch
            {
                '\\' => '\\',
                '\'' => '\'',
                '"' => '"',
                'n' => '\n',
                'r' => '\r',
                't' => '\t',
                '0' => '\0',
                _ => throw new FormatException(FormatDiagnostic(
                    $"Invalid escape sequence '\\{escapedCharacter}'",
                    new SourceSpan(escapeStart, CurrentPosition)))
            };
            value.Append(escapedValue);
        }

        var errorSpan = new SourceSpan(start, CurrentPosition);
        throw new FormatException(FormatDiagnostic("Unterminated quoted string", errorSpan));
    }

    private Token ReadWord(SourcePosition start)
    {
        var valueStart = _offset;
        while (_offset < input.Length &&
               !char.IsWhiteSpace(input[_offset]) &&
               !delimiters.Contains(input[_offset]) &&
               input[_offset] is not '(' and not ')' and not '\'' and not '"')
            Advance();

        var value = input[valueStart.._offset];
        return new Token(TokenType.BareToken, value, value, new SourceSpan(start, CurrentPosition));
    }

    private void Advance()
    {
        switch (input[_offset])
        {
            case '\r':
                _offset += _offset + 1 < input.Length && input[_offset + 1] == '\n' ? 2 : 1;
                _line++;
                _column = 1;
                break;
            case '\n':
                _offset++;
                _line++;
                _column = 1;
                break;
            default:
                _offset++;
                _column++;
                break;
        }
    }

    private string FormatDiagnostic(string message, SourceSpan span)
    {
        var (sourceExcerpt, markerColumn) = GetSourceExcerpt(span);
        var marker = new string(' ', markerColumn) + '↑';

        return $"""
                {message} at line {span.Start.Line}, column {span.Start.Column}.
                {sourceExcerpt}{Environment.NewLine}{marker}
                """;
    }

    private (string Text, int MarkerColumn) GetSourceExcerpt(SourceSpan span)
    {
        var lineStart = input.AsSpan(0, span.Start.Offset).LastIndexOfAny('\r', '\n') + 1;
        var lineEndOffset = input.AsSpan(span.Start.Offset).IndexOfAny('\r', '\n');
        var lineEnd = lineEndOffset < 0 ? input.Length : span.Start.Offset + lineEndOffset;

        const int contextBefore = 15;
        const int contextAfter = 15;

        var excerptStart = Math.Max(lineStart, span.Start.Offset - contextBefore);
        var excerptEnd = Math.Min(lineEnd, span.Start.Offset + contextAfter);
        var hasLeadingOmission = excerptStart > lineStart;
        var hasTrailingOmission = excerptEnd < lineEnd;

        var text =
            $"{(hasLeadingOmission ? "…" : "")}{input[excerptStart..excerptEnd]}{(hasTrailingOmission ? "…" : "")}";
        var markerColumn = (hasLeadingOmission ? 1 : 0) + span.Start.Offset - excerptStart;

        return (text, markerColumn);
    }
}
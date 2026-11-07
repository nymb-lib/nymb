using System.Collections.Frozen;

namespace Nymb.Lexer;

internal sealed record NymbLexerConfiguration
{
    public static readonly NymbLexerConfiguration Default = new();

    public FrozenSet<char> Delimiters { get; init; } = FrozenSet<char>.Empty;
}

param(
    [Parameter(Position = 0)]
    [string] $CommitMessageFile
)

if ([string]::IsNullOrWhiteSpace($CommitMessageFile) -or
    -not (Test-Path -LiteralPath $CommitMessageFile -PathType Leaf)) {
    Write-Output 'ERROR: Cannot read the commit message.'
    Write-Output 'Git must pass the commit message file as the first argument.'
    exit 1
}

$lines = [System.IO.File]::ReadAllLines($CommitMessageFile)
$subject = if ($lines.Length -gt 0) { $lines[0] } else { '' }
$pattern = '^(feat|fix|docs|test|refactor|chore)(\([A-Za-z0-9][A-Za-z0-9._/-]*\))?!?: \S.*$'

if ($subject -cnotmatch $pattern) {
    Write-Output ''
    Write-Output 'ERROR: Invalid commit message.'
    Write-Output ''
    Write-Output 'Allowed types: feat, fix, docs, test, refactor, chore.'
    Write-Output ''
    Write-Output 'The subject must follow the Conventional Commits format:'
    Write-Output '  <type>[optional scope][!]: <description>'
    Write-Output ''
    Write-Output 'Examples:'
    Write-Output '  feat: add tokenization support'
    Write-Output '  fix(lexer): handle empty input'
    Write-Output '  refactor(parser)!: simplify expression parsing'
    Write-Output ''
    Write-Output 'Use one of the allowed types, then a colon and a non-empty description.'
    Write-Output "Your subject: $subject"
    Write-Output ''
    exit 1
}

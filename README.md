# Nymb

Write your filter logic — not your parser

## Commit messages

Commit subjects must follow the Conventional Commits format:

```text
<type>[optional scope][!]: <description>
```

Allowed types are `feat`, `fix`, `docs`, `test`, `refactor`, and `chore`.
For example: `feat(lexer): support escaped characters`. The `!` marker before
the colon indicates a breaking change. The `.githooks/commit-msg` Git hook
checks each commit subject. Git for Windows runs it using its bundled `sh`
shell, so no additional runtime is needed. Enable the hook once in each clone
with:

```sh
git config core.hooksPath .githooks
```

GitHub Actions also checks commit messages on pushes and pull requests using
`commitlint.config.mjs`.

## Branch names

Use `<type>/<short-kebab-case-description>` for branch names, for example
`feat/login-page`, `fix/navbar-overflow`, or `chore/update-dependencies`.
Common types include `feat`, `fix`, `chore`, `refactor`, `docs`, `test`, and
`perf`. Ticket IDs are optional, e.g. `feat/#142-user-profile`.

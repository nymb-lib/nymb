# Agent instructions

## Solution

- Use the .NET SDK selected by `global.json`.
- The solution is `Nymb.slnx`. Production code is in `src/Nymb`; tests are in
  `test/Nymb.Tests`.
- The test project uses xUnit v3 with Microsoft.Testing.Platform.
- Nullable reference types and implicit usings are enabled in both projects.

## Common commands

Run these from the repository root:

```sh
dotnet restore Nymb.slnx
dotnet build Nymb.slnx --no-restore
dotnet test Nymb.slnx --no-build
```

Run restore before using `--no-restore`, and build before using `--no-build`.
Omit those options when the corresponding step has not run or inputs have
changed.

## Tests

When adding or renaming tests, follow the test-naming skill in
[`.github/skills/test-naming/SKILL.md`](.github/skills/test-naming/SKILL.md).
Apply it to every test you create; do not introduce a competing naming
convention.

Keep changes focused, preserve existing project conventions, and do not edit
generated build output under `bin/` or `obj/`.

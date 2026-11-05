---
name: test-naming
description: Use when creating or renaming automated tests, to name them after the behavior they verify.
---

# Writing and naming tests

Write tests for observable behavior, not implementation details. Keep each test
focused on one meaningful behavior and make it deterministic and independent of
other tests.

## Names

- Use a clear statement of the behavior, understandable to someone familiar
  with the problem domain. Prefer plain English and separate words with
  underscores.
- Describe the scenario and its observable outcome specifically, and state a
  fact rather than an intention. For example:
  `Delivery_with_a_past_date_is_invalid`.
- Avoid a fixed `Method_Scenario_Result` template and names coupled to
  implementation details. Mention a system-under-test method only when
  describing behavior is impractical, such as for simple utility code.
- Keep names concise without losing meaning. Give test classes a recognizable
  subject name, commonly ending in `Tests`; a class is an organizational entry
  point, and its tests may cover behavior involving multiple production
  classes.
- Place tests under the test root namespace, followed by the production
  namespace's subnamespace. For example, tests for `Nymb.Lexer` belong in
  `Nymb.Tests.Lexer`, with a class such as `NymbLexerTests`. If a test covers
  behavior across multiple production namespaces, use the subnamespace of its
  primary subject.

## Test quality

- Use xUnit v3 conventions and attributes supported by this repository's
  Microsoft.Testing.Platform runner.
- Make tests repeatable: do not depend on execution order, shared mutable state,
  wall-clock timing, or external services unless the test explicitly exercises
  that integration.
- Assert externally observable results. Keep setup and actions easy to find;
  use Arrange/Act/Assert structure when it improves clarity, not as a rigid
  template.
- Keep assertions easy to read as plain-English statements. Assign nontrivial
  expected values to descriptively named variables instead of constructing
  them inline in an assertion.
- Use a theory when several inputs illustrate the same behavior; use separate
  facts when the scenarios verify meaningfully different behavior.

Source: [You are naming your tests wrong!](https://enterprisecraftsmanship.com/posts/you-naming-tests-wrong/)

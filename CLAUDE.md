# CSharpExtender

@~/.claude/rules/programming.md
@~/.claude/rules/csharp.md
@~/.claude/rules/github.md

Project-specific guidance for this repo. Rules that apply to more than one project live in the
user-level `CLAUDE.md` and in the imported files above; this file is only for things particular
to CSharpExtender.

## What this is

A NuGet package, published as `ScottLilly.CSharpExtender`, of extension methods and small helper
classes reused across my projects. Everything public here is someone else's API.

## Layout

```
CSharpExtender.sln
  CSharpExtender/         the library itself, net8.0, packed on build
  Test.CSharpExtender/    xUnit, covers the library
  BenchmarkTestBench/     BenchmarkDotNet console app, not shipped
docs/                     design notes and architecture
tools/                    scripts and utilities that are not part of the build
```

All markdown except this file, `README.md` and `RELEASE_NOTES.md` lives in `docs/`.

## It is published, so changes are breaking changes

- A public signature that changes breaks every consumer on recompile. Say so when proposing one,
  and put it under `### Breaking Changes` in `RELEASE_NOTES.md`.
- `README.md` is the package readme on nuget.org. A new public member is not done until it is
  listed there.
- The version lives in `CSharpExtender/CSharpExtender.csproj`. Do not bump it unless asked.

## Where this repo differs from the C# rules

| Rule | What this repo does |
|---|---|
| Test projects are named `Tests.<Name>` | `Test.CSharpExtender`, singular. Predates the convention. Match it until it is deliberately renamed |
| MSTest | xUnit. The whole test project is xUnit |

## Writing documents in docs/

- **Be terse.** Long documents do not get read. Cut preamble and restatement.
- **Do not speculate past what I told you.** Do not turn three sentences into three pages of
  inferred rationale or decisions I never made.
- **Mark inference as inference.** Tag it `*(inference)*` so my intent is distinguishable from
  your reading of it.
- A short list beats prose. One concrete example beats a general explanation.
- If a document has grown unwieldy, say so and offer to consolidate rather than adding to it.
- No em dashes, no en dashes, no smart quotes.

## Settled work leaves no trace

When something is **built**, delete it from `docs/BACKLOG.md` and do not write it up anywhere else.
No superseded sections, no struck-through questions, no "amended on such a date" banners. The
documents describe the project as it is now. Git holds the history.

When something is **decided against**, delete it from Proposed and leave one brief line under
`## Decided against` saying why, so it does not get re-proposed. A GitHub issue that is dropped
is closed as not planned and taken off its milestone, so it does not count as that milestone's work.

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
  Tests.CSharpExtender/   xUnit, covers the library
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

## Every change to the package goes in RELEASE_NOTES.md

A change is not done until it is written up under the `## Version x.y.z` heading for the release
being worked on. Additions, removals and modifications alike, whether or not a consumer has to do
anything about it. If you changed what ships, write it down.

That includes a fix to something added earlier in the same unreleased version. The reader is
someone upgrading from the last released version, and what they get is the end state, not the
steps it took to arrive there. So describe the behavior they will see, not the edit.

Put the entry under the section it belongs to: `Breaking Changes`, `Features`, `Bug Fixes`,
`Performance` or `Dependencies`. `Breaking Changes` is measured against the previous release on
NuGet, so a fix to a member introduced in the version being written up is a bug fix, not a break.

Work that never reaches the package stays out: the test project, the benchmark bench, the build
and CI, and the repo's own documents.

## Where this repo differs from the C# rules

| Rule | What this repo does |
|---|---|
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

## Unbuilt work lives in GitHub issues

This repo has no `docs/BACKLOG.md`. Everything not built yet is a GitHub issue, and an idea worth
keeping is raised as one rather than written into a document.

## Settled work leaves no trace

When something is **built**, do not write it up anywhere. No superseded sections, no struck-through
questions, no "amended on such a date" banners. The documents describe the project as it is now.
Git holds the history, and the commit that closes an issue holds the reasoning.

When something is **decided against**, the reason goes where someone would hit the question again,
which is usually the XML doc comment on the member it concerns. A GitHub issue that is dropped is
closed as not planned and taken off its milestone, so it does not count as that milestone's work.

# Backlog

Open work. **Nothing here is committed.** Every entry is a proposal and an open question, not a
queue position.

Most of this project's open work is tracked in
[GitHub issues](https://github.com/ScottLilly/CSharpExtender/issues). This file is for items that
are not issues yet.

**Lifecycle:** an entry leaves `## Proposed` as soon as it is settled, in one of two ways.

- **Done.** Delete it outright. Nothing gets written up anywhere else to record that it was ever
  proposed: no superseded section, no struck-through text, no "shipped on such a date" note. Git
  holds the history. The code is the record of what got built.
- **Decided against.** Delete it from `## Proposed` and leave one brief line under
  `## Decided against` saying what it was and why not, so the same idea does not come back around
  in three months.

## Proposed

Everything below came out of a code review in September 2026 and has not been raised as a GitHub
issue. None of it is decided.

### Structure and tooling

| Item | What it would cost |
|---|---|
| Enable nullable reference types on the library | The test project already has them on. The library does not, which is where they would pay, since every public signature is someone else's API. Expect a pile of warnings on first run |
| Collapse the two `.editorconfig` files | `CSharpExtender/.editorconfig` sets `root = true`, so the root `.editorconfig` never reaches the library project. One file, at the root |
| Add `Directory.Build.props` with `EnforceCodeStyleInBuild` | Needs the editorconfig question settled first, and needs a clean build, or every future build is noisy. Watch for collision with the version and packaging properties already in the csproj |
| Rename `Test.CSharpExtender` to `Tests.CSharpExtender` | Directory, `.csproj`, the `.sln` entry, and the namespace in 21 test files. No `InternalsVisibleTo` to break, so the blast radius is small |
| Convert `CSharpExtender.sln` to `.slnx` | The standard layout assumes `.slnx`. Solution folders for `docs` and `tools` are already in the `.sln` |
| Tag releases | `release.yml` creates a `vX.Y.Z` tag, but the `PackageReleaseNotes` link points at `master`, so an old version's package page shows the newest notes. A tag link fixes it but needs bumping each release |

### Code cleanup

| Item | What |
|---|---|
| `BaseRedactionService` and `CompositeRegexMatcher` | The same 15 lines of regex-combining constructor logic, twice. The base class could hold a matcher |
| `BaseRedactionService` | `public` constructor on an `abstract` class. Should be `protected` |
| `Common/Enums.cs` | Holds `IndentType`. The filename should match the type |
| `StringBuilderExtensionMethods.ProcessText` | Applies `MaxLength` before prefix, suffix, format and indent, so the result can exceed `MaxLength` |
| `StringBuilderExtensionMethods.AppendFormatted` | An optional parameter sits before `params object[] args`, so callers must pass `null` explicitly to supply args |

## Open design questions

### Null handling differs between the two redaction services

`XmlRedactionService.Redact(null)` throws `ArgumentNullException`. `JsonRedactionService.Redact(null)`
returns null, and its `RedactToString(null)` throws `NullReferenceException`. The XML behavior is
the better one, but the two should agree. Changing the JSON side is breaking.

### Should `IRedactionService<T>` be public

It is the reason both services have identical member lists. Keeping it public invites outside
implementations and locks the four-member shape. It also has a latent problem: `Redact(T)` and
`Redact(string)` are ambiguous if anyone closes it with `T` as `string`.

The README documents it as public, alongside `BaseRedactionService`. Making it internal means
taking that section back out.

### Does anything else belong in `tools/`

The folder exists with only a README. `BenchmarkTestBench` is a solution project rather than a
tool, because it references the library and runs from Visual Studio.

## Decided against

- **Changing what `IsDigitsOnly` returns for an empty string.** It returns `true`, which was
  raised as surprising. The null case was the real defect and is fixed: `null` now returns
  `false`. Empty staying `true` is deliberate, is documented in `RELEASE_NOTES.md`, and changing
  it now would be a second breaking change to one method in one release for no practical gain.

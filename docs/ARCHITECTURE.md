# Architecture

What exists and why it is built this way. Work that is not built yet lives in the repo's
[GitHub issues](https://github.com/ScottLilly/CSharpExtender/issues).

## Shape

```
CSharpExtender.sln
  CSharpExtender/         net8.0   the library, packed to NuGet on build
  Tests.CSharpExtender/   net8.0   xUnit, covers the library
  BenchmarkTestBench/     net8.0   BenchmarkDotNet console app, not shipped
docs/                     design notes and architecture
.github/workflows/        ci.yml and release.yml
```

Only the library ships. `Tests.CSharpExtender` and `BenchmarkTestBench` both set
`<IsPackable>false</IsPackable>`, so `dotnet pack` at the solution root produces exactly one
package.

Inside `CSharpExtender/`:

| Folder | Holds |
|---|---|
| `Collections/` | `GenericCache<TKey, TValue>` |
| `DataAnnotations/` | `ValidationAttribute` subclasses |
| `ExtensionMethods/` | One static class per extended type |
| `Models/` | `ObservableModel`, `PropertyChangeTrackingModel` and their support types |
| `Options/` | Option objects passed into extension methods, and the types they are built from |
| `Services/` | Classes that are not extension methods: RNG, regex matching, redaction, reflection |

## Decisions already made

### The package is the API

Everything public in `CSharpExtender/` is consumed by other projects through NuGet. A changed
signature is a breaking change for every consumer, which is why signature changes are called out
in `RELEASE_NOTES.md` under `### Breaking Changes` and why the README doubles as the package's
reference list.

### net8.0, from version 3.0.0

Version 3.0.0 moved the package to .NET 8.0. Earlier versions targeted older frameworks.

### xUnit, not MSTest

The test project is xUnit and predates the MSTest default in `~/.claude/rules/csharp.md`.

### Reflection results are cached

`SmartReflection` holds a `ConcurrentDictionary<Type, PropertyInfo[]>` so repeated property lookups
on the same type do not re-enter `Type.GetProperties`, and a second dictionary of resolved
`MethodInfo` so `InvokeMethod` does not re-resolve an overload on every call.
`UniqueItemsAttribute` keys a property cache on `Type` the same way. Every cache is static and
unbounded, which is the right trade for a fixed set of types in a single process.

`EnumExtensionMethods` caches differently, and deliberately. Its cache is a nested generic class
holding one `ConcurrentDictionary<TEnum, string>` per closed enum type, rather than one dictionary
keyed on `Type`, so a lookup neither boxes the enum value nor needs a second dictionary hit to
reach the right inner cache.

Lookups key on the object's **runtime** type, `obj.GetType()`, never on a generic parameter. The
extension-method wrappers in `SmartReflectionExtensionMethods` pass the object as `object`, so
keying on `typeof(T)` would look up the properties of `System.Object` and find nothing. This is
why `GetPropertyValue`, `SetPropertyValue` and `InvokeMethod` take a single type parameter for the
property or return type, and not a second one for the object.

### IsOfType is exact, IsOfTypeOrSubclass is assignable

`ObjectExtensionMethods` carries two families of type check, and the split between them is the
whole point of having both:

| Family | Means | Implemented with |
|---|---|---|
| `IsOfType`, `IsNotOfType` | Exactly this type. A subclass does not count | `GetType() == type` |
| `IsOfTypeOrSubclass`, `IsNotOfTypeOrSubclass` | This type, a subclass, or an implementation when it is an interface | `Type.IsAssignableFrom` |

Each family has four members: generic and `Type`-parameter forms, on an `object` receiver and on a
`Type` receiver. All four members of a family agree, which they did not before 3.0.0.

`Type.IsSubclassOf` is deliberately **not** used anywhere here. It excludes the type itself and
ignores interfaces, so `typeof(Foo).IsOfTypeOrSubclass<Foo>()` returned `false` while the same
check on an instance of `Foo` returned `true`.

### Redaction walks a path and matches it against one combined regex

`BaseRedactionService` joins the supplied patterns into a single alternation and compiles it once.
Each service walks its document, builds a dotted path for every node, and tests that path.

| | JSON | XML |
|---|---|---|
| Path | Property names from the root, `user.ssn` | Element names from the root, `person.ssn` |
| Repeated entries | Array entries indexed, `items[0]` | Not indexed. One pattern redacts every matching element |
| Attributes | n/a | Element path plus `.@name`, `person.@id` |
| A redacted value becomes | `""`, `0`, `false`, or null by JSON type | An element emptied of content, or an empty attribute value |

Patterns are matched with `IsMatch`, which is unanchored, so a pattern that matches a node also
matches everything below it. Redacting `person` takes the whole subtree, and redacting
`person.ssn` also blanks that element's attributes. That is the intended direction for a redaction
tool: over-redact rather than under-redact.

### Releases are manual, and the version comes from the csproj

`ci.yml` builds and tests every push and pull request to `master`. `release.yml` is triggered by
hand from the Actions tab and does the shipping: build, test, pack, then a GitHub Release with the
`.nupkg` attached.

The release version is read back off the packed file rather than taken as a workflow input, so the
tag can never disagree with the package, and NuGet's normalization is picked up for free
(`<Version>3.0.0.0</Version>` packs as `3.0.0` and tags as `v3.0.0`). Bumping a version means
editing `CSharpExtender/CSharpExtender.csproj` and nothing else.

The release body is the matching `## Version x.y.z` section lifted out of `RELEASE_NOTES.md`.

## Dependencies

| Package | Why it is here |
|---|---|
| `System.Text.Json` | JSON serialization and the redaction services. Replaced Newtonsoft.Json |
| `BenchmarkDotNet` | `BenchmarkTestBench` only, not shipped |
| `xunit`, `coverlet.collector` | Test project only |

`System.Text.Json` is the only dependency that reaches a consumer.

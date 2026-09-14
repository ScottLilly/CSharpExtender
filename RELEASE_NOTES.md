# RELEASE NOTES

## Version 3.0.0 (unreleased)

Changes are against version 2.2.0, the previous release on NuGet.

### Breaking Changes

* Targets .NET 8.0. Version 2.2.0 targeted .NET Standard 2.1, so projects on .NET Framework or .NET Standard cannot take this version.
* `GenericCache<TKey, TValue>` moved out of the global namespace into `CSharpExtender.Collections`. Consuming code needs `using CSharpExtender.Collections;`.
* `GenericCache.Remove` returns `bool` instead of `void`. Removing a key that is not in the cache returns `false` rather than throwing.
* `GenericCache` calculates expiration times in UTC instead of local time. Cached items no longer expire an hour early or late across a daylight saving change.
* `ObjectExtensionMethods.IsOfType<T>` and `IsNotOfType<T>` on an object are now an exact type match. An instance of a subclass of `T` no longer counts as being of type `T`. Use `IsOfTypeOrSubclass<T>` for the old behavior.
* `ObjectExtensionMethods.IsOfTypeOrSubclass` and `IsNotOfTypeOrSubclass` on a `Type` now include the type itself and, for an interface, its implementations. They previously used `Type.IsSubclassOf`, which excluded both, so `typeof(Foo).IsOfTypeOrSubclass<Foo>()` returned `false`.
* `JsonExtensionMethods.AsSerializedJson` and `StringBuilderExtensionMethods.AppendLineIfNotEmpty` each take a new optional parameter. Existing calls still compile, but they have to be recompiled.
* `DateTimeExtensionMethods.ToIso8601String` uses the round-trip ("O") format instead of `yyyy-MM-ddTHH:mm:ss.fffZ`. It previously appended a literal "Z" to every value, publishing a local or unspecified time as though it were UTC. A `Utc` value now ends in "Z", a `Local` value carries its real offset, and an `Unspecified` value carries neither. The fractional second is now seven digits rather than three, so the string changes for every caller.
* `StringExtensionMethods.IsDigitsOnly` returns `false` for a null string instead of `true`. An empty string still returns `true`.
* `StringExtensionMethods.SplitPascalCase` returns an empty list for a null or empty string. It previously returned a list holding that null or empty string, so a caller iterating the result got a null element.
* `StringExtensionMethods.SplitPath` no longer returns an empty entry for a path segment of nothing but whitespace. `@"a/ /b".SplitPath()` returned `["a", "", "b"]` and now returns `["a", "b"]`, which is what the method always documented. Segments are trimmed before empty ones are removed, rather than after.

### Features

New classes:

* `SmartReflection` and `SmartReflectionExtensionMethods` - reflection helpers that cache each type's properties, so repeated lookups do not re-enter `Type.GetProperties`.
* `JsonRedactionService` and `XmlRedactionService` - remove sensitive values from JSON and XML, for paths matching a list of regex patterns. Both implement `IRedactionService<T>`.
* `CompositeRegexMatcher` - checks a string against a list of regex patterns compiled into one.
* `AlphaOnlyAttribute` - validates that a string property contains only letters.
* `ConditionalRequiredAttribute` - validates that a property has a value when another property on the same object holds a particular value. Needs a call that supplies the object, such as `Validator.ValidateObject`.
* `IsInListAttribute` - validates that a property holds one of a fixed list of allowed values, optionally ignoring case for strings.
* `MaskAttribute` - declares how a property's value should be masked when displayed or logged. Not a `ValidationAttribute`: `MaskExtensionMethods` applies it.
* `MaskExtensionMethods` - `ToMaskedString` and `ToMaskedStrings` apply a property's `MaskAttribute`.
* `DisplayFormatExtensionMethods` - `ToDisplayString` and `ToDisplayStrings` apply the `DisplayFormatAttribute` from `System.ComponentModel.DataAnnotations`, which is otherwise only honored by UI frameworks. Accepts both a bare format specifier and the conventional `"{0:...}"` form.
* `UniqueItemsAttribute` - validates that a collection property contains no duplicates.
* `StringBuilderOptions` - controls case, indentation, maximum length, prefix and suffix text, HTML escaping, and formatting for the StringBuilder extension methods.

New members:

* `StringBuilderExtensionMethods` gains `Append`, `AppendIf`, `AppendLine`, `AppendLineIf`, `AppendFormatted`, `AppendLineFormatted`, `AppendJoined` and `AppendLineJoined`. Every method, including the existing `AppendLineIfNotEmpty`, accepts an optional `StringBuilderOptions`.
* `GenericCache.TryGet` - retrieves a value without returning `default` for a key that is genuinely absent.
* `NumericExtensionMethods.ApproximatelyEquals` - compares two floats within a percentage tolerance, handling `NaN`, infinity, and values near zero.
* `StringExtensionMethods.ToMaxLengthOf` - trims a string to a maximum length, leaving a shorter string and a null alone.
* `StringExtensionMethods.Mask` - replaces the middle of a string with a mask character, leaving a number of characters visible at each end. Separators stay visible by default, so a card or phone number keeps its shape.
* `NumericExtensionMethods.StandardDeviation` and `PopulationStandardDeviation` - sample (n-1) and population (n) standard deviation of a collection, with overloads for `double`, `int` and `decimal`.
* `JsonExtensionMethods.AsSerializedJson` accepts an optional `JsonSerializerOptions`.
* `StringExtensionMethods.IncludesTheWords` gains an overload taking a `StringComparison` before the words. The existing signature keeps comparing with `CurrentCultureIgnoreCase`, so nothing has to change; `Ordinal` and `OrdinalIgnoreCase` measure 12x to 20x faster.

### Bug Fixes

* `JsonExtensionMethods.PrettyPrintJson(object, JsonSerializerOptions)` copies the supplied options instead of setting `WriteIndented` on the caller's instance. It previously threw `InvalidOperationException` for any options instance that had already been used to serialize, and silently left `WriteIndented` set to `true` on the caller's object.
* `EnumExtensionMethods.GetEnumDescription` returns the value's string representation instead of throwing `NullReferenceException` for an undefined value or a combination of `[Flags]` members.

### Performance

Measured with BenchmarkDotNet. No behavior changes: every method below returns what it returned before.

* `StringExtensionMethods.IsDigitsOnly` iterates a `ReadOnlySpan<char>` instead of running a LINQ predicate over the string.
* `StringExtensionMethods.Repeat` builds the result with `string.Create`, and uses `new string(char, int)` when the text is a single character, instead of concatenating an `Enumerable.Repeat`. Between 1.6x and 11x faster depending on the text and count, with the single-character case seeing the largest gain.
* `StringExtensionMethods.ToDigitsOnly` filters into a stack buffer instead of a LINQ `Where` and `ToArray`. Between 2.7x and 4.3x faster, allocating a sixth as much for short strings.
* `StringExtensionMethods.IncludesTheWords` searches a `ReadOnlySpan<char>` and no longer uses LINQ. Around 5% faster, and no longer allocates.
* `StringExtensionMethods.RemoveText` tests whether a pass shortened the string instead of calling `Contains` before each `Replace`, saving a scan of the string per pass.
* `EnumExtensionMethods.GetEnumDescription` holds its cache per closed enum type instead of in one dictionary keyed on a boxed `Enum`. Between 10x and 18x faster, and no longer allocates on a cache hit. `GetEnumDescriptions` benefits through it.
* `ObjectExtensionMethods.DeepClone` reuses one `JsonSerializerOptions` instead of building one per call, and round-trips through UTF-8 bytes rather than a string. About 30% faster, allocating 15% less, and no longer reaching gen 1 or gen 2.
* `StringBuilderExtensionMethods` share one default `StringBuilderOptions` instead of allocating one whenever the caller passes none, and `AppendLineIfNotEmpty` tests its condition directly instead of through a `Func<bool>` that captured the text. `AppendLineIfNotEmpty` is 3.5x faster; both stop allocating.
* `GenericCache.Set` writes through the dictionary indexer instead of `AddOrUpdate` with a lambda that captured the new item. About 29% faster, allocating a quarter as much.
* `StringExtensionMethods.SplitPath` holds its separator array in a static field instead of building one per call, and splits with `StringSplitOptions.TrimEntries` rather than trimming through LINQ afterwards. See Breaking Changes: that also corrected the whitespace-only segment case.

### Dependencies

* `System.Text.Json` updated from 8.0.4 to 9.0.2.

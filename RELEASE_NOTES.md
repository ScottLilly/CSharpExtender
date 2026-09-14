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

### Features

New classes:

* `SmartReflection` and `SmartReflectionExtensionMethods` - reflection helpers that cache each type's properties, so repeated lookups do not re-enter `Type.GetProperties`.
* `JsonRedactionService` and `XmlRedactionService` - remove sensitive values from JSON and XML, for paths matching a list of regex patterns. Both implement `IRedactionService<T>`.
* `CompositeRegexMatcher` - checks a string against a list of regex patterns compiled into one.
* `AlphaOnlyAttribute` - validates that a string property contains only letters.
* `UniqueItemsAttribute` - validates that a collection property contains no duplicates.
* `StringBuilderOptions` - controls case, indentation, maximum length, prefix and suffix text, HTML escaping, and formatting for the StringBuilder extension methods.

New members:

* `StringBuilderExtensionMethods` gains `Append`, `AppendIf`, `AppendLine`, `AppendLineIf`, `AppendFormatted`, `AppendLineFormatted`, `AppendJoined` and `AppendLineJoined`. Every method, including the existing `AppendLineIfNotEmpty`, accepts an optional `StringBuilderOptions`.
* `GenericCache.TryGet` - retrieves a value without returning `default` for a key that is genuinely absent.
* `NumericExtensionMethods.ApproximatelyEquals` - compares two floats within a percentage tolerance, handling `NaN`, infinity, and values near zero.
* `StringExtensionMethods.ToMaxLengthOf` - trims a string to a maximum length, leaving a shorter string and a null alone.
* `JsonExtensionMethods.AsSerializedJson` accepts an optional `JsonSerializerOptions`.

### Performance

* `StringExtensionMethods.IsDigitsOnly` iterates a `ReadOnlySpan<char>` instead of running a LINQ predicate over the string.

### Dependencies

* `System.Text.Json` updated from 8.0.4 to 9.0.2.

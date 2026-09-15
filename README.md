# ScottLilly.CSharpExtender (NuGet package)

<img align="left" width="75" height="75" style="color:white" src="https://github.com/ScottLilly/CSharpExtender/blob/master/CSharpExtender/Icon.png">

Extension methods and classes I often find useful for C# development.

The extension methods are written to make the source code read more like a natural language, sometimes just wrapping an existing "negative" function into a more natural "not postive" name, eliminating the need for a "!", which doesn't read as naturally.

The package is annotated for nullable reference types, so the compiler can tell you which arguments accept a null and which returns can be one.

## Project Overview
![Build Status](https://github.com/ScottLilly/CSharpExtender/actions/workflows/ci.yml/badge.svg)
[![NuGet](https://img.shields.io/nuget/v/ScottLilly.CSharpExtender)](https://www.nuget.org/packages/ScottLilly.CSharpExtender/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ScottLilly.CSharpExtender)](https://www.nuget.org/packages/ScottLilly.CSharpExtender/)
[![License](https://img.shields.io/github/license/ScottLilly/CSharpExtender)](https://github.com/ScottLilly/CSharpExtender/blob/master/LICENSE.txt)

## DataAnnotations

Attribute classes to validate properties in models, and to declare how a property should be displayed.

- **`AlphaOnlyAttribute`**: Check that a string only contains letters.
- **`ConditionalRequiredAttribute`**: Check that a property has a value when another property on the same object holds a particular value. Set `DependentProperty` and `RequiredWhenValue`.
- **`IsInListAttribute`**: Check that a property holds one of a fixed list of allowed values. Set `IgnoreCase` to compare strings without regard to case.
- **`MaskAttribute`**: Declare how a property's value should be masked when displayed or logged. Set `MaskChar`, `VisiblePrefixLength` and `VisibleSuffixLength`. Not a validation attribute: apply it with `ToMaskedString` in `MaskExtensionMethods`.
- **`UniqueItemsAttribute`**: Check that a collection property does not contain duplicate items.

## Collections

### GenericCache

This class creates a cache object with a generic key and generic value.

- **`Clear`**: Remove all items from the cache.
- **`Get`**: Retrieves a value from the cache, or default, if not in the cache.
- **`Set`**: Adds or updates a value in the cache.
- **`Remove`**: Remove an entry fom the cache.
- **`RemoveExpiredItems`**: Removes all expired items from the cache.

## Extension Methods

### DataReaderExtensionMethods

This class provides extension methods for reading a column out of an `IDataReader` by name instead of by ordinal, which is the forward-only path used for anything too large to pull into a `DataSet`.

Every method returns `default` when the column holds `DBNull`, and a nullable type is converted to its underlying type, so a null column read as `int?` returns null. A column name that is not in the result set throws `IndexOutOfRangeException` naming the column and listing the ones that are there. A value that will not convert throws `InvalidCastException` naming the column, which is the whole point of reading by name.

- **`GetValue<T>`**: Gets a typed value from a column, by name.
- **`GetBoolean`**, **`GetDateTime`**, **`GetDecimal`**, **`GetDouble`**, **`GetFloat`**, **`GetGuid`**, **`GetInt32`**, **`GetString`**: Typed wrappers around `GetValue<T>`.
- **`GetByteArray`**: Gets a column as a `byte[]`. Named for the type because `IDataReader` already declares a `GetBytes` that copies into a caller's buffer.

These share their names with the ordinal-based methods `IDataReader` declares itself. Overload resolution separates them by argument type, so `reader.GetString("Name")` reaches this class and `reader.GetString(0)` reaches the reader's own.

### DataSetExtensionMethods

This class provides extension methods for DataTables and DataSets

- **`Get<T>`**: Gets a typed value from a column in a DataRow.
- **`HasColumn`**: Checks if a DataSet has a column with the specified name.
- **`HasRows`**: Checks if a DataSet has any rows.

### DateTimeExtensionMethods

This class provides extension methods for `DateTime` in C#.

- **`EndOfDay`**: Returns the end of the day 23:59:59 for the provided date.
- **`EndOfMonth`**: Returns the date for the end of the month, at 23:59:59, for the provided date.
- **`IsWeekday`**: Checks if the given date is a weekday.
- **`IsWeekend`**: Checks if the given date is a weekend.
- **`NextDay`**: Gets the next day from the given date.
- **`PreviousDay`**: Gets the previous day from the given date.
- **`StartOfDay`**: Returns the start of the day 00:00:00 for the provided date.
- **`StartOfMonth`**: Returns the date for the start of the month, at 00:00:00, for the provided date.
- **`ToIso8601String`**: Converts the provided date to an ISO 8601 round-trip ("O") string. A `Utc` value ends in "Z", a `Local` value carries its real offset, and an `Unspecified` value carries neither.

### DisplayFormatExtensionMethods

This class applies the `DisplayFormatAttribute` from `System.ComponentModel.DataAnnotations`, which is otherwise only honored by UI frameworks. A bare specifier (`"yyyy-MM-dd"`) and the conventional wrapped form (`"{0:yyyy-MM-dd}"`) are both accepted.

- **`ToDisplayString`**: Returns the named property's value, formatted with its `DisplayFormatAttribute`.
- **`ToDisplayStrings`**: Returns every property carrying a `DisplayFormatAttribute`, formatted, keyed by property name.

### EnumExtensionMethods

This class provides extension methods for Enums in C#.

- **`GetEnumDescription`**: Gets the description of an enum value, from its `[Description]` attribute.
- **`GetEnumDescriptions`**: Gets all descriptions of a specific enum type.
- **`GetEnumDisplayName`**: Gets the display name of an enum value, from the `Name` on its `[Display]` attribute. Falls back to the value's string representation when there is no attribute, or the attribute sets no `Name`.
- **`GetEnumDisplayNames`**: Gets all display names of a specific enum type.
- **`GetEnumValues`**: Gets all values of a specific enum type, as a read-only collection. The same instance is returned to every caller.
- **`ParseEnum`**: Parses a string to an enum value.

### JsonExtensionMethods

This class provides extension methods for JSON manipulation in C#.

- **`AsDeserializedJson`**: Deserializes the JSON string to an object of the specified type.
- **`AsSerializedJson`**: Serializes the specified object to a JSON string.
- **`GetValueFromJsonPath`**: Retrieves a value from a JSON string using a JSON path.
- **`GetValueFromJsonPath<T>`**: Retrieves a typed value from a JSON string using a JSON path.
- **`PrettyPrintJson` (string)**: Formats the JSON string with indented formatting.
- **`PrettyPrintJson` (object)**: Serializes the specified object to a JSON string with indented formatting.

### LinqExtensionMethods

This class provides LINQ-related extension methods in C#.

- **`ForEach` (IEnumerable)**: Applies an action to each element in the collection.
- **`ForEach` (List)**: Applies an action to each element in the list.
- **`HasDuplicatePropertyValue<T, TProperty>`**: Checks if any objects have the same value in the specified property.
- **`HasDuplicatePropertyValue<T>`**: Checks if any objects have the same value in the specified string property.
- **`None`**: Checks if none of the elements in the collection satisfy the provided condition. If no condition is provided, it checks if the collection is empty.
- **`RandomElement`**: Returns a random element from the list.

### MaskExtensionMethods

This class applies the `MaskAttribute` declared on a property, for hiding a value when it is displayed or logged.

- **`ToMaskedString`**: Returns the named property's value with its `MaskAttribute` applied.
- **`ToMaskedStrings`**: Returns every property carrying a `MaskAttribute`, masked, keyed by property name.

### NumericExtensionMethods

This class provides extension methods for numerical operations in C#.

- **`ApproximatelyEquals`**: Checks if two floating point numbers are approximately equal, within a specified tolerance.
- **`IsEven`**: Checks if the given integer is even.
- **`IsEvenlyDivisibleBy`**: Checks if the given integer is evenly divisible by another integer.
- **`IsNegative`**: Checks if the given integer is negative.
- **`IsOdd`**: Checks if the given integer is odd.
- **`IsPositive`**: Checks if the given integer is positive.
- **`PopulationStandardDeviation`**: Calculates the standard deviation of a collection, dividing by n. Use when the values are the complete set. Overloads for `double`, `int` and `decimal`.
- **`StandardDeviation`**: Calculates the sample standard deviation of a collection, dividing by n-1. Use when the values are a sample of a larger population. Overloads for `double`, `int` and `decimal`.

### ObjectExtensionMethods

This class provides extension methods for objects in C#.

- **`DeepClone`**: Creates a deep clone of the source object.
- **`HasCustomAttributeOfType`**: Checks if the property info has a custom attribute of a specific type.
- **`IsFloatingPointType`**: Checks if the object is of a floating point type.
- **`IsIntegerType`**: Checks if the object is of an integer type.
- **`IsNotNull`**: Checks if the object is not null.
- **`IsNotOfType` (with Type parameter)**: Checks if the object is not exactly of a specific type.
- **`IsNotOfType` (for Type with baseType parameter)**: Checks if the type is not exactly a specific type.
- **`IsNotOfType<T>`**: Checks if the object is not exactly of a specific type.
- **`IsNotOfType<T>` (for Type)**: Checks if the type is not exactly a specific type.
- **`IsNotOfTypeOrSubclass` (with Type parameter)**: Checks if the object is not of a specific type, a subclass of it, or an implementation of it.
- **`IsNotOfTypeOrSubclass` (for Type with baseType parameter)**: Checks if the type is not a specific type, a subclass of it, or an implementation of it.
- **`IsNotOfTypeOrSubclass<T>`**: Checks if the object is not of a specific type, a subclass of it, or an implementation of it.
- **`IsNotOfTypeOrSubclass<T>` (for Type)**: Checks if the type is not a specific type, a subclass of it, or an implementation of it.
- **`IsNull`**: Checks if the object is null.
- **`IsNumericType`**: Checks if the object is of a numeric type.
- **`IsOfType` (with Type parameter)**: Checks if the object is exactly of a specific type.
- **`IsOfType` (for Type with baseType parameter)**: Checks if the type is exactly a specific type.
- **`IsOfType<T>`**: Checks if the object is exactly of a specific type.
- **`IsOfType<T>` (for Type)**: Checks if the type is exactly a specific type.
- **`IsOfTypeOrSubclass` (with Type parameter)**: Checks if the object is of a specific type, a subclass of it, or an implementation of it.
- **`IsOfTypeOrSubclass` (for Type with baseType parameter)**: Checks if the type is a specific type, a subclass of it, or an implementation of it.
- **`IsOfTypeOrSubclass<T>`**: Checks if the object is of a specific type, a subclass of it, or an implementation of it.
- **`IsOfTypeOrSubclass<T>` (for Type)**: Checks if the type is a specific type, a subclass of it, or an implementation of it.

The `IsOfType` family is an exact type match. The `IsOfTypeOrSubclass` family also accepts subclasses and, when the type is an interface, implementations of it.

### SmartReflectionExtensionMethods

The same reflection helpers as the `SmartReflection` service class, called on an object instead of on a `Type`, so `myObject.GetPropertyValue<string>("Name")` reads the way the rest of this package does. They share `SmartReflection`'s property cache.

Lookups use the object's runtime type, so an object held in a base-class or `object` variable still finds its own properties.

- **`GetPropertiesWithAttribute<TAttribute>`**: Retrieves all properties on the object that have a specific attribute.
- **`GetPropertyNames`**: Gets the names of all public instance properties on the object.
- **`GetPropertyType`**: Gets the type of a specified property on the object.
- **`GetPropertyValue<TProperty>`**: Gets the value of a property from the object, with type safety.
- **`HasAttribute<TAttribute>`**: Checks if the object's type has a specific attribute.
- **`HasProperty`**: Checks if the object has a specific property.
- **`HasPropertyAttribute<TAttribute>`**: Checks if a property on the object has a specific attribute.
- **`InvokeMethod<TResult>`**: Invokes a method on the object by name, with type-safe return value.
- **`SetPropertyValue<TProperty>`**: Sets the value of a property on the object, with type safety.

### StringExtensionMethods

This class provides extension methods for string manipulations in C#.

- **`CollapseWhitespace`**: Trims the text and reduces every run of whitespace inside it to a single space, tabs and line breaks included. A null returns null, and text that needs nothing changed is returned as the same instance.
- **`ConvertFromString<T>`**: Converts a string to the specified type, if possible.
- **`DoesNotHaveText`**: Returns 'true' if the string is null, empty, or only contains whitespace.
- **`DoesNotMatch`**: Check if strings are not equal, using InvariantCultureIgnoreCase.
- **`HasText`**: Returns 'true' if the string is not null, empty, or only contains whitespace.
- **`IncludesTheWords`**: Checks if a string contains all the words in the specified array. Compares with `CurrentCultureIgnoreCase`, or pass a `StringComparison` as the first argument. `Ordinal` and `OrdinalIgnoreCase` are an order of magnitude faster.
- **`IsDigitsOnly`**: Returns 'true' if the string only contains digits. A null returns 'false'. An empty string returns 'true', because it holds no character that is not a digit.
- **`Mask`**: Replaces the middle of a string with a mask character, leaving a number of characters visible at each end. Separators stay visible by default, so `"1234-5678-9012-5678".Mask('*', 4, 4)` returns `"1234-****-****-5678"`.
- **`Matches`**: Check if strings are equal, using InvariantCultureIgnoreCase.
- **`NullIfEmpty`**: Returns a null if the string is null, empty, or only contains whitespace.
- **`RemoveText`**: Removes all instances of the specified text from the string.
- **`Repeat`**: Returns a string with the text repeated the specified number of times.
- **`SplitPath`**: Convert a file path into an array of the individual directories, splitting on both forward and back slashes. Entries are trimmed, and no empty entries are returned.
- **`ToDigitsOnly`**: Returns a string with all non-digits removed.
- **`ToMaxLengthOf`**: Trims string to a maximum length, if it exceeds that length
- **`ToStringWithLineFeeds`**: Converts an IEnumerable of strings to a single string with line feeds between each string.

### StringBuilderExtensionMethods

This class provides extension methods for StringBuilder objects in C#. 
All functions accept an optional `StringBuilderOptions` object, which can be used to specify the behavior of the function. 
When it is omitted, the text is appended unchanged.

- **`Append`**: Appends a string to the StringBuilder object.
- **`AppendFormatted`**: Appends a formatted string to the StringBuilder object.
- **`AppendIf`**: Appends text to the StringBuilder object, if a function evaluates to 'true'.
- **`AppendJoined`**: Appends a joined string to the StringBuilder object, using the specified separator.
- **`AppendLine`**: Appends a string and line feed to the StringBuilder object.
- **`AppendLineFormatted`**: Appends a formatted string and line feed to the StringBuilder object.
- **`AppendLineIf`**: Appends text and line feed to the StringBuilder object, if a function evaluates to 'true'.
- **`AppendLineIfNotEmpty`**: If the passed in line is not empty, it will be appended to the StringBuilder object.
- **`AppendLineJoined`**: Appends a joined string and line feed to the StringBuilder object, using the specified separator.

#### StringBuilderOptions

Properties on the options object, all optional. A new `StringBuilderOptions` with nothing set leaves the text alone.

- **`EscapeHtml`**: HTML-encodes the text.
- **`Format`**: A composite format string the text is passed through, such as `"[{0}]"`.
- **`IndentDepth`**: How many tabs or spaces make up one indent level. Defaults to 4.
- **`IndentLevel`**: How many levels to indent the text.
- **`IndentType`**: `IndentType.Spaces` (the default) or `IndentType.Tabs`.
- **`MaxLength`**: Caps the length of the value, not the length of the finished string. It is applied after `ToUpper`, `ToLower` and `EscapeHtml` have rewritten the value, and before `Format`, `PrefixText`, `SuffixText` and the indent wrap it, so none of those four is ever cut in half. A `MaxLength` of 3 with a `SuffixText` of `]` returns four characters.
- **`PrefixText`**: Text placed before the value.
- **`SuffixText`**: Text placed after the value.
- **`ToLower`** / **`ToUpper`**: Changes the case of the text. Setting both leaves the case alone.

### VersionExtensionMethods

This class reads version numbers out of strings, keeping the alpha text rather than stripping it. `1.2.3-beta.1`, `2.0.0-rc1` and `3.1.0+build7` all read. The result is a `SemanticVersion`, described under Models below, so the prerelease label survives alongside the numbers.

- **`ToSemanticVersion`**: Reads a version string. Throws `FormatException` on text it cannot read.
- **`TryParseSemanticVersion`**: The same, without throwing. Returns false and sets null for text it cannot read.
- **`NumericPartOfVersion`**: Returns the numbers only, so `"1.2.3-beta.1"` gives `"1.2.3"`. Null when the text is not a version.
- **`PrereleaseLabelOf`**: Returns the label without its leading `-`, so `"1.2.3-beta.1"` gives `"beta.1"`. Null when there is no label.

`ToSemanticVersion` and `TryParseSemanticVersion` divide the work the way `Version.Parse` and `Version.TryParse` do, so the caller picks whether unreadable input is exceptional rather than having that decided for them. Nothing here returns a zero version for input it could not read.

### XElementExtensionMethods

This class provides extension methods for `XElement`, the LINQ to XML API, mirroring the `XmlNode` set below member for member. A missing attribute or element returns null from the string methods and the type's default from the others, and a value that will not parse returns the type's default rather than throwing, which is what the `XmlNode` set does. An unprefixed name matches only attributes and elements that are in no namespace.

Both sets parse the same way, under the invariant culture, so a document reads the same on every machine. A boolean accepts `true`, `false`, `1` and `0` in any casing, covering both what `bool.ToString()` writes and what schema-generated XML writes. A date keeps what the text said about its time zone, so a value ending in `Z` comes back with `Kind` of `Utc`, and a slash-separated date is always `MM/dd/yyyy`.

- **`AttributeAsBool`**: Returns the value of the specified attribute as a boolean.
- **`AttributeAsDateTime`**: Returns the value of the specified attribute as a DateTime.
- **`AttributeAsInt`**: Returns the value of the specified attribute as an integer.
- **`AttributeAsString`**: Returns the value of the specified attribute as a string.
- **`ElementAsInt`**: Returns the text of the specified child element as an integer.
- **`ElementAsString`**: Returns the text of the specified child element as a string.
- **`GetValue`**: Returns the value of an attribute or a child element of the given name, whichever carries it, for XML that writes the same value either way. The attribute wins when both carry it, and a miss returns null. This is the one member with no `XmlNode` counterpart.

These throw `ArgumentNullException` for a null element, where the `XmlNode` set throws `NullReferenceException`.

### XmlExtensionMethods

This class provides extension methods for XML handling in C#, using `XmlNode`, the pre-LINQ API. See `XElementExtensionMethods` above for the same set over `XElement`.

- **`AttributeAsBool`**: Returns the value of the specified attribute as a boolean.
- **`AttributeAsDateTime`**: Returns the value of the specified attribute as a DateTime.
- **`AttributeAsInt`**: Returns the value of the specified attribute as an integer.
- **`AttributeAsString`**: Returns the value of the specified attribute as a string.
- **`ElementAsInt`**: Returns the inner text of the specified child element as an integer.
- **`ElementAsString`**: Returns the inner text of the specified child element as a string.

## Models

These classes can be used as base classes for models and handle property changed notification and logging of changed property values.

### SemanticVersion

An immutable record holding a version's numeric parts alongside its prerelease label and build metadata, so `1.2.3-beta.1` stays one value. `System.Version` has no room for a label, which is why this exists: through a `Version`, `1.2.3-beta.1` and `1.2.3` compare as equal, and a caller deciding whether to upgrade gets the wrong answer with nothing to tell them so. Build it with `ToSemanticVersion` or `TryParseSemanticVersion`, above.

- **`Major`**, **`Minor`**, **`Patch`**, **`Revision`**: The numeric parts. A part the text did not carry is zero. `Revision` is there because .NET version strings often have a fourth part; Semantic Versioning does not.
- **`PrereleaseLabel`**: The label without its leading `-`, or null.
- **`BuildMetadata`**: The metadata without its leading `+`, or null.
- **`IsPrerelease`**: Whether there is a label.
- **`NumericPart`**: The numbers on their own, as a string.
- **`ToVersion`**: Converts to a `System.Version`. Lossy on purpose: the label and metadata are dropped.

Ordering follows the Semantic Versioning precedence rules, through `IComparable<SemanticVersion>` and the `<`, `>`, `<=` and `>=` operators, so `1.0.0-alpha` < `1.0.0-beta.2` < `1.0.0-beta.11` < `1.0.0-rc.1` < `1.0.0`. Build metadata takes no part in precedence, while equality covers every part including it. Those disagree in one case: `1.2.3+a` and `1.2.3+b` are not equal, but neither is greater.

### ObservableModel

Base class that handles property changed notification.

-**`SetProperty<T>`**: Used to set a property's backing field and raise property changed event.

### PropertyChangeTrackingModel

Base class that inherits from ObservableModel (to handle property change notification) and also logs the values of the changed properties. Implements IChangeTracking.

-**`AcceptChanges`**: Clears the PropertyChangeLog.  
-**`IsChanged`**: True when the PropertyChangeLog holds at least one entry. Raises property changed when that changes.  
-**`PropertyChangeLog`**: ObservableCollection of properties values that were changed.

### PropertyChangedLog

One entry in a `PropertyChangeTrackingModel`'s log. Inherit from it to record more about a change, such as the user who made it.

-**`ChangeDateTime`**: When the property was changed, in UTC. `Kind` is `DateTimeKind.Utc`, so call `ToLocalTime()` to display it.  
-**`NewValue`**: The value the property was changed to.  
-**`PropertyName`**: The name of the property that changed.

## Service classes

Classes to handle common tasks.

### BaseRedactionService

The shared base behind `JsonRedactionService` and `XmlRedactionService`. It holds the supplied patterns as a `CompositeRegexMatcher`, which its subclasses reach through the protected `_matcher` field.

Both services expose the same four members, `Redact` and `RedactToString` over the document type and over text, and every one of them throws `ArgumentNullException` for a null argument.

### CompositeRegexMatcher

Accepts a list of regex patterns and checks if a string matches any of them. This is what `BaseRedactionService` uses to decide which paths get redacted.

-**`MatchesAny` (string)**: Checks if the string matches any of the regex patterns.
-**`MatchesAny` (ReadOnlySpan&lt;char&gt;)**: The same check, for text a caller has built and does not need as a string.
-**`HasPatterns`**: Whether any pattern was supplied. False means nothing can ever match, so a caller can skip work it would only do to find that out.

### JsonRedactionService

Removes sensitive values from JSON, for paths matching any of a list of regex patterns.

Paths are property names from the root, separated by `.`, with array entries indexed: the `ssn` property in `{"user": {"ssn": "..."}}` has the path `user.ssn`, and the first entry of an `items` array has the path `items[0]`. Patterns are matched anywhere in a path rather than against the whole of it, so a pattern matching an object also matches everything below it.

A redacted string becomes empty, a number becomes zero, a boolean becomes false, and an object or array becomes null. Pass `ignoreCase: true` to the constructor to match paths case-insensitively.

- **`Redact` (JsonObject)**: Redacts the JsonObject in place and returns it.
- **`Redact` (string)**: Parses the JSON string, redacts it, and returns the result as a JsonObject.
- **`RedactToString` (JsonObject)**: Redacts the JsonObject in place and returns the result as a JSON string.
- **`RedactToString` (string)**: Parses the JSON string, redacts it, and returns the result as a JSON string.

### RngCreator

This class is designed to create cryptographically random numbers in C#.

- **`GetNumberBetween`**: Generates a cryptographically random number between the specified minimum and maximum values.

### SmartReflection

This class provides efficient, type-safe reflection utilities with property caching for improved performance in .NET applications.

- **`HasAttribute<TAttribute>`**: Checks if a type has a specific attribute.
- **`HasPropertyAttribute<TAttribute>`**: Checks if a property on a type has a specific attribute.
- **`GetPropertiesWithAttribute<TAttribute>`**: Retrieves all properties on a type that have a specific attribute.
- **`GetPropertyValue<TProperty>`**: Gets the value of a property from an object, with type safety.
- **`SetPropertyValue<TProperty>`**: Sets the value of a property on an object, with type safety.
- **`GetPropertyNames`**: Gets the names of all public instance properties on a type.
- **`HasProperty`**: Checks if a type has a specific property.
- **`GetPropertyType`**: Gets the type of a specified property on a type.
- **`InvokeMethod<TResult>`**: Invokes a method on an object by name, with type-safe return value.

### XmlRedactionService

Removes sensitive values from XML, for paths matching any of a list of regex patterns.

Paths are element names from the root, separated by `.`, so the `ssn` element in `<person><ssn/></person>` has the path `person.ssn`. An attribute is its element's path plus `.@` and the attribute name, so `person.@id`. Repeated sibling elements are not indexed, so one pattern redacts every element sharing a path. Patterns are matched anywhere in a path rather than against the whole of it, so a pattern matching an element also matches everything below it, including its attributes.

A redacted element keeps its tag and loses all of its content, including child elements. A redacted attribute keeps its name and gets an empty value. Pass `ignoreCase: true` to the constructor to match paths case-insensitively.

- **`Redact` (XmlDocument)**: Redacts the XmlDocument in place and returns it.
- **`Redact` (string)**: Loads the XML string, redacts it, and returns the result as an XmlDocument.
- **`RedactToString` (XmlDocument)**: Redacts the XmlDocument in place and returns the result as an XML string.
- **`RedactToString` (string)**: Loads the XML string, redacts it, and returns the result as an XML string.

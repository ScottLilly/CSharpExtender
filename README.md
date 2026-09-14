# ScottLilly.CSharpExtender (NuGet package)

<img align="left" width="75" height="75" style="color:white" src="https://github.com/ScottLilly/CSharpExtender/blob/master/CSharpExtender/Icon.png">

Extension methods and classes I often find useful for C# development.

The extension methods are written to make the source code read more like a natural language, sometimes just wrapping an existing "negative" function into a more natural "not postive" name, eliminating the need for a "!", which doesn't read as naturally.

## Project Overview
![Build Status](https://github.com/ScottLilly/CSharpExtender/actions/workflows/ci.yml/badge.svg)
[![NuGet](https://img.shields.io/nuget/v/ScottLilly.CSharpExtender)](https://www.nuget.org/packages/ScottLilly.CSharpExtender/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ScottLilly.CSharpExtender)](https://www.nuget.org/packages/ScottLilly.CSharpExtender/)
[![License](https://img.shields.io/github/license/ScottLilly/CSharpExtender)](https://github.com/ScottLilly/CSharpExtender/LICENSE)

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

- **`GetEnumDescription`**: Gets the description of an enum value.
- **`GetEnumDescriptions`**: Gets all descriptions of a specific enum type.
- **`GetEnumValues`**: Gets all values of a specific enum type.
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

### StringExtensionMethods

This class provides extension methods for string manipulations in C#.

- **`ConvertFromString<T>`**: Converts a string to the specified type, if possible.
- **`DoesNotHaveText`**: Returns 'true' if the string is null, empty, or only contains whitespace.
- **`DoesNotMatch`**: Check if strings are not equal, using InvariantCultureIgnoreCase.
- **`HasText`**: Returns 'true' if the string is not null, empty, or only contains whitespace.
- **`IncludesTheWords`**: Checks if a string contains all the words in the specified array. Compares with `CurrentCultureIgnoreCase`, or pass a `StringComparison` as the first argument. `Ordinal` and `OrdinalIgnoreCase` are an order of magnitude faster.
- **`IsDigitsOnly`**: Returns 'true' if the string only contains digits. A null returns 'false'.
- **`Mask`**: Replaces the middle of a string with a mask character, leaving a number of characters visible at each end. Separators stay visible by default, so `"1234-5678-9012-5678".Mask('*', 4, 4)` returns `"1234-****-****-5678"`.
- **`Matches`**: Check if strings are equal, using InvariantCultureIgnoreCase.
- **`NullIfEmpty`**: Returns a null if the string is null, empty, or only contains whitespace.
- **`RemoveText`**: Removes all instances of the specified text from the string.
- **`Repeat`**: Returns a string with the text repeated the specified number of times.
- **`SplitPath`**: Convert a file path into an array of the individual directories.
- **`ToDigitsOnly`**: Returns a string with all non-digits removed.
- **`ToMaxLengthOf`**: Trims string to a maximum length, if it exceeds that length
- **`ToStringWithLineFeeds`**: Converts an IEnumerable of strings to a single string with line feeds between each string.

### StringBuilderExtensionMethods

This class provides extension methods for StringBuilder objects in C#. 
All functions accept optional StringBuilderOptions object, which can be used to specify the behavior of the function. 
The default value is `StringBuilderOptions.None`.

- **`Append`**: Appends a string to the StringBuilder object.
- **`AppendFormatted`**: Appends a formatted string to the StringBuilder object.
- **`AppendIf`**: Appends text to the StringBuilder object, if a function evaluates to 'true'.
- **`AppendJoined`**: Appends a joined string to the StringBuilder object, using the specified separator.
- **`AppendLine`**: Appends a string and line feed to the StringBuilder object.
- **`AppendLineFormatted`**: Appends a formatted string and line feed to the StringBuilder object.
- **`AppendLineIf`**: Appends text and line feed to the StringBuilder object, if a function evaluates to 'true'.
- **`AppendLineIfNotEmpty`**: If the passed in line is not empty, it will be appended to the StringBuilder object.
- **`AppendLineJoined`**: Appends a joined string and line feed to the StringBuilder object, using the specified separator.

### XmlExtensionMethods

This class provides extension methods for XML handling in C#.

- **`AttributeAsBool`**: Returns the value of the specified attribute as a boolean.
- **`AttributeAsDateTime`**: Returns the value of the specified attribute as a DateTime.
- **`AttributeAsInt`**: Returns the value of the specified attribute as an integer.
- **`AttributeAsString`**: Returns the value of the specified attribute as a string.
- **`ElementAsInt`**: Returns the inner text of the specified child element as an integer.
- **`ElementAsString`**: Returns the inner text of the specified child element as a string.

## Models

These classes can be used as base classes for models and handle property changed notification and logging of changed property values.

### ObservableModel

Base class that handles property changed notification.

-**`SetProperty<T>`**: Used to set a property's backing field and raise property changed event.

### PropertyChangeTrackingModel

Base class that inherits from ObservableModel (to handle property change notification) and also logs the values of the changed properties. Implements IChangeTracking.

-**`AcceptChanges`**: Clears the PropertyChangeLog.  
-**`PropertyChangeLog`**: ObservableCollection of properties values that were changed.

## Service classes

Classes to handle common tasks.

### CompositeRegexMatcher

Accepts a list of regex patterns and checks if a string matches any of them.

-**`MatchesAny`**: Checks if the string matches any of the regex patterns.

### JsonRedactionService

Removes sensitive values from JSON, for paths matching any of a list of regex patterns. Implements `IRedactionService<JsonObject>`.

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

Removes sensitive values from XML, for paths matching any of a list of regex patterns. Implements `IRedactionService<XmlDocument>`.

Paths are element names from the root, separated by `.`, so the `ssn` element in `<person><ssn/></person>` has the path `person.ssn`. An attribute is its element's path plus `.@` and the attribute name, so `person.@id`. Repeated sibling elements are not indexed, so one pattern redacts every element sharing a path. Patterns are matched anywhere in a path rather than against the whole of it, so a pattern matching an element also matches everything below it, including its attributes.

A redacted element keeps its tag and loses all of its content, including child elements. A redacted attribute keeps its name and gets an empty value. Pass `ignoreCase: true` to the constructor to match paths case-insensitively.

- **`Redact` (XmlDocument)**: Redacts the XmlDocument in place and returns it.
- **`Redact` (string)**: Loads the XML string, redacts it, and returns the result as an XmlDocument.
- **`RedactToString` (XmlDocument)**: Redacts the XmlDocument in place and returns the result as an XML string.
- **`RedactToString` (string)**: Loads the XML string, redacts it, and returns the result as an XML string.

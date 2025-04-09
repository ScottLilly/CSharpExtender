# CSharpExtender

Extension methods and classes I often find useful for C# development.

## DataAnnotations

Attribute classes to validate properties in models.

- **`AlphaOnlyAttribute`**: Check that a string only contains letters.
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
- **`ToIso8601String`**: Converts the provided date to an ISO 8601 string.

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

### NumericExtensionMethods

This class provides extension methods for numerical operations in C#.

- **`IsEven`**: Checks if the given integer is even.
- **`IsEvenlyDivisibleBy`**: Checks if the given integer is evenly divisible by another integer.
- **`IsNegative`**: Checks if the given integer is negative.
- **`IsOdd`**: Checks if the given integer is odd.
- **`IsPositive`**: Checks if the given integer is positive.

### ObjectExtensionMethods

This class provides extension methods for objects in C#.

- **`DeepClone`**: Creates a deep clone of the source object.
- **`HasCustomAttributeOfType`**: Checks if the property info has a custom attribute of a specific type.
- **`IsFloatingPointType`**: Checks if the object is of a floating point type.
- **`IsIntegerType`**: Checks if the object is of an integer type.
- **`IsNotNull`**: Checks if the object is not null.
- **`IsNotOfType` (with Type parameter)**: Checks if the object is not of a specific type.
- **`IsNotOfType` (for Type with baseType parameter)**: Checks if the type is not of a specific type.
- **`IsNotOfType<T>`**: Checks if the object is not of a specific type.
- **`IsNotOfType<T>` (for Type)**: Checks if the type is not of a specific type.
- **`IsNotOfTypeOrSubclass` (with Type parameter)**: Checks if the object is not of a specific type and not a subclass of that type.
- **`IsNotOfTypeOrSubclass` (for Type with baseType parameter)**: Checks if the type is not of a specific type and not a subclass of that type.
- **`IsNotOfTypeOrSubclass<T>`**: Checks if the object is not of a specific type and not a subclass of that type.
- **`IsNotOfTypeOrSubclass<T>` (for Type)**: Checks if the type is not of a specific type and not a subclass of that type.
- **`IsNull`**: Checks if the object is null.
- **`IsNumericType`**: Checks if the object is of a numeric type.
- **`IsOfType` (with Type parameter)**: Checks if the object is of a specific type.
- **`IsOfType` (for Type with baseType parameter)**: Checks if the type is of a specific type.
- **`IsOfType<T>`**: Checks if the object is of a specific type.
- **`IsOfType<T>` (for Type)**: Checks if the type is of a specific type.
- **`IsOfTypeOrSubclass` (with Type parameter)**: Checks if the object is of a specific type or a subclass of that type.
- **`IsOfTypeOrSubclass` (for Type with baseType parameter)**: Checks if the type is of a specific type or a subclass of that type.
- **`IsOfTypeOrSubclass<T>`**: Checks if the object is of a specific type or a subclass of that type.
- **`IsOfTypeOrSubclass<T>` (for Type)**: Checks if the type is of a specific type or a subclass of that type.

### StringExtensionMethods

This class provides extension methods for string manipulations in C#.

- **`ConvertFromString<T>`**: Converts a string to the specified type, if possible.
- **`DoesNotHaveText`**: Returns 'true' if the string is null, empty, or only contains whitespace.
- **`DoesNotMatch`**: Check if strings are not equal, using InvariantCultureIgnoreCase.
- **`HasText`**: Returns 'true' if the string is not null, empty, or only contains whitespace.
- **`IncludesTheWords`**: Checks if a string contains all the words in the specified array.
- **`IsDigitsOnly`**: Returns 'true' if the string only contains digits.
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
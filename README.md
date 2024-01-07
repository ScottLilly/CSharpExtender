# CSharpExtender

# DateTimeExtensionMethods

This class provides extension methods for `DateTime` in C#.

- **`StartOfDay`**: Returns the start of the day 00:00:00 for the provided date.
- **`EndOfDay`**: Returns the end of the day 23:59:59 for the provided date.
- **`StartOfMonth`**: Returns the date for the start of the month, at 00:00:00, for the provided date.
- **`EndOfMonth`**: Returns the date for the end of the month, at 23:59:59, for the provided date.
- **`ToIso8601String`**: Converts the provided date to an ISO 8601 string.
- **`IsWeekend`**: Checks if the given date is a weekend.
- **`IsWeekday`**: Checks if the given date is a weekday.
- **`NextDay`**: Gets the next day from the given date.
- **`PreviousDay`**: Gets the previous day from the given date.

# EnumExtensionMethods

This class provides extension methods for Enums in C#.

- **`GetEnumDescription`**: Gets the description of an enum value.
- **`GetEnumValues`**: Gets all values of a specific enum type.
- **`GetEnumDescriptions`**: Gets all descriptions of a specific enum type.
- **`ParseEnum`**: Parses a string to an enum value.

# JsonExtensionMethods

This class provides extension methods for JSON manipulation in C#.

- **`GetValueFromJsonPath`**: Retrieves a value from a JSON string using a JSON path.
- **`AsSerializedJson`**: Serializes the specified object to a JSON string.
- **`AsDeserializedJson`**: Deserializes the JSON string to an object of the specified type.
- **`PrettyPrintJson` (string)**: Formats the JSON string with indented formatting.
- **`PrettyPrintJson` (object)**: Serializes the specified object to a JSON string with indented formatting.

# LinqExtensionMethods

This class provides LINQ-related extension methods in C#.

- **`None`**: Checks if none of the elements in the collection satisfy the provided condition. If no condition is provided, it checks if the collection is empty.
- **`ForEach` (IEnumerable)**: Applies an action to each element in the collection.
- **`ForEach` (List)**: Applies an action to each element in the list.
- **`RandomElement`**: Returns a random element from the list.

# NumericExtensionMethods

This class provides extension methods for numerical operations in C#.

- **`IsEven`**: Checks if the given integer is even.
- **`IsOdd`**: Checks if the given integer is odd.
- **`IsPositive`**: Checks if the given integer is positive.
- **`IsNegative`**: Checks if the given integer is negative.
- **`IsEvenlyDivisibleBy`**: Checks if the given integer is evenly divisible by another integer.

# ObjectExtensionMethods

This class provides extension methods for objects in C#.

- **`DeepClone`**: Creates a deep clone of the source object.
- **`IsNumericType`**: Checks if the object is of a numeric type.
- **`IsIntegerType`**: Checks if the object is of an integer type.
- **`IsFloatingPointType`**: Checks if the object is of a floating point type.
- **`HasCustomAttributeOfType`**: Checks if the property info has a custom attribute of a specific type.
- **`IsNull`**: Checks if the object is null.
- **`IsNotNull`**: Checks if the object is not null.
- **`IsOfType<T>`**: Checks if the object is of a specific type.
- **`IsOfType` (with Type parameter)**: Checks if the object is of a specific type.
- **`IsNotOfType<T>`**: Checks if the object is not of a specific type.
- **`IsNotOfType` (with Type parameter)**: Checks if the object is not of a specific type.
- **`IsOfTypeOrSubclass<T>`**: Checks if the object is of a specific type or a subclass of that type.
- **`IsOfTypeOrSubclass` (with Type parameter)**: Checks if the object is of a specific type or a subclass of that type.
- **`IsNotOfTypeOrSubclass<T>`**: Checks if the object is not of a specific type and not a subclass of that type.
- **`IsNotOfTypeOrSubclass` (with Type parameter)**: Checks if the object is not of a specific type and not a subclass of that type.
- **`IsOfTypeOrSubclass<T>` (for Type)**: Checks if the type is of a specific type or a subclass of that type.
- **`IsOfTypeOrSubclass` (for Type with baseType parameter)**: Checks if the type is of a specific type or a subclass of that type.
- **`IsNotOfTypeOrSubclass<T>` (for Type)**: Checks if the type is not of a specific type and not a subclass of that type.
- **`IsNotOfTypeOrSubclass` (for Type with baseType parameter)**: Checks if the type is not of a specific type and not a subclass of that type.
- **`IsOfType<T>` (for Type)**: Checks if the type is of a specific type.
- **`IsOfType` (for Type with baseType parameter)**: Checks if the type is of a specific type.
- **`IsNotOfType<T>` (for Type)**: Checks if the type is not of a specific type.
- **`IsNotOfType` (for Type with baseType parameter)**: Checks if the type is not of a specific type.

# StringExtensionMethods

This class provides extension methods for string manipulations in C#.

- **`Matches`**: Check if strings are equal, using InvariantCultureIgnoreCase.
- **`DoesNotMatch`**: Check if strings are not equal, using InvariantCultureIgnoreCase.
- **`HasText`**: Returns 'true' if the string is not null, empty, or only contains whitespace.
- **`DoesNotHaveText`**: Returns 'true' if the string is null, empty, or only contains whitespace.
- **`NullIfEmpty`**: Returns a null if the string is null, empty, or only contains whitespace.
- **`ToDigitsOnly`**: Returns a string with all non-digits removed.
- **`IsDigitsOnly`**: Returns 'true' if the string only contains digits.
- **`ToStringWithLineFeeds`**: Converts an IEnumerable of strings to a single string with line feeds between each string.
- **`Repeat`**: Returns a string with the text repeated the specified number of times.
- **`SplitPath`**: Convert a file path into an array of the individual directories.
- **`IncludesTheWords`**: Checks if a string contains all the words in the specified array.
- **`RemoveText`**: Removes all instances of the specified text from the string.

# XmlExtensionMethods

This class provides extension methods for XML handling in C#.

- **`AttributeAsInt`**: Returns the value of the specified attribute as an integer.
- **`AttributeAsString`**: Returns the value of the specified attribute as a string.
- **`AttributeAsBool`**: Returns the value of the specified attribute as a boolean.
- **`AttributeAsDateTime`**: Returns the value of the specified attribute as a DateTime.
- **`ElementAsString`**: Returns the inner text of the specified child element as a string.
- **`ElementAsInt`**: Returns the inner text of the specified child element as an integer.

# RngCreator

This class is designed to create cryptographically random numbers in C#.

- **`GetNumberBetween`**: Generates a cryptographically random number between the specified minimum and maximum values.

using CSharpExtender.Services;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CSharpExtender.ExtensionMethods
{
    /// <summary>
    /// Scott's extension methods for LINQ
    /// </summary>
    public static class LinqExtensionMethods
    {
        /// <summary>
        /// Checks if none of the elements in the collection satisfy the provided condition.
        /// If no condition is provided, it checks if the collection is empty.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="elements">The collection to check.</param>
        /// <param name="func">The condition to check each element against.</param>
        /// <returns>True if none of the elements satisfy the condition, false otherwise.</returns>
        public static bool None<T>(this IEnumerable<T> elements, Func<T, bool> func = null)
        {
            if (func == null)
            {
                return !elements.Any();
            }

            return !elements.Any(func);
        }

        /// <summary>
        /// Applies an action to each element in the collection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="elements">The collection to iterate over.</param>
        /// <param name="action">The action to apply to each element.</param>
        public static void ForEach<T>(this IEnumerable<T> elements, Action<T> action)
        {
            foreach (T element in elements)
            {
                action(element);
            }
        }

        /// <summary>
        /// Applies an action to each element in the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to iterate over.</param>
        /// <param name="action">The action to apply to each element.</param>
        public static void ForEach<T>(this List<T> list, Action<T> action)
        {
            for (int i = 0; i < list.Count; i++)
            {
                action(list[i]);
            }
        }

        /// <summary>
        /// Returns a random element from the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="options">The list to pick a random element from.</param>
        /// <returns>A random element from the list, or default(T) if the list is empty.</returns>
        public static T RandomElement<T>(this List<T> options)
        {
            return options.Count == 0
                ? default
                : options[RngCreator.GetNumberBetween(0, options.Count - 1)];
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoScribeClient.Utilities
{
    public static class StringExtensions {
        /// <summary>
        /// An alternative method for String.Contains, which uses a custom comparison.
        /// This method is helpful when the content of a protocol is searched.
        /// </summary>
        /// <param name="str">The main string.</param>
        /// <param name="substring">The string to check.</param>
        /// <param name="comp">Comparison type.</param>
        /// <returns>True if the main string contains the subtring.</returns>
        public static bool Contains(this String str, String substring,
                                    StringComparison comp) {
            if (substring == null)
                throw new ArgumentNullException("substring",
                                                "substring cannot be null.");
            return str.IndexOf(substring, comp) >= 0;
        }
    }
}

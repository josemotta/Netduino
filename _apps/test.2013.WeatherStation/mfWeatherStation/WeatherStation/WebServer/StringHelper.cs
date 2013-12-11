using System;
using Microsoft.SPOT;

namespace WeatherStation.WebServer
{
    /// <summary>
    /// Helper class with extension methods for String class
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Replace an oldValue with a newValue inside a string
        /// </summary>
        /// <param name="input">String on which execute replace</param>
        /// <param name="oldVale">Old value that must be replaced</param>
        /// <param name="newValue">New value for replacing</param>
        /// <returns>New string with replacement</returns>
        public static string Replace(this string input, string oldVale, string newValue)
        {
            if ((input == null) || (input == String.Empty))
                return input;

            string result = String.Empty;
            int startIndex = 0;
            int idxOldValue = 0;
            
            while ((idxOldValue = input.IndexOf(oldVale, startIndex)) != -1)
            {
                result += input.Substring(startIndex, idxOldValue - startIndex);
                result += newValue;
                startIndex = idxOldValue + oldVale.Length;
            }

            if (startIndex < input.Length)
                result += input.Substring(startIndex);

            return result;
        }

        /// <summary>
        /// Verify if a string starts with another string
        /// </summary>
        /// <param name="input">String to verify</param>
        /// <param name="start">Starter string to verify</param>
        /// <returns>True if string input starts with specified string. False otherwise</returns>
        public static bool StartsWith(this string input, string start)
        {
            if ((input == null) || (input == String.Empty))
                throw new ArgumentNullException("input");

            if ((start == null) || (start == String.Empty))
                throw new ArgumentNullException("start");

            if (start.Length > input.Length)
                throw new ArgumentException("start parameter is longer than input parameter");

            return (input.IndexOf(start) == 0);
        }
    }
}

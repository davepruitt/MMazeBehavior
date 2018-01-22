using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MMazeBehavior
{
    public static class HelperMethods
    {
        /// <summary>
        /// Cleans an input string to it contains only alphanumeric characters
        /// </summary>
        /// <param name="strIn">The input string</param>
        /// <returns>The output string</returns>
        public static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^\w\-]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Parses a list of numbers in string format, and returns a List<int> object of all the numbers found.
        /// </summary>
        /// <param name="list">The string containing the list of numbers</param>
        /// <param name="sep">The delimitor between the numbers in the string</param>
        /// <returns>A list of numbers as a List object</returns>
        public static List<int> ParseNumberList (string list, char sep = ',')
        {
            List<int> result = new List<int>();

            var string_list = list.Split(new char[] { sep });
            for (int i = 0; i < string_list.Length; i++)
            {
                int new_number = 0;
                bool success = Int32.TryParse(string_list[i].Trim(), out new_number);
                if (success)
                {
                    result.Add(new_number);
                }
            }

            return result;
        }
    }
}

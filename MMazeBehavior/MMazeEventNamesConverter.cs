using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MMazeBehavior
{
    public class MMazeEventNamesConverter
    {
        /// <summary>
        /// Converts a string to an event type.
        /// </summary>
        /// <param name="description">String description of an event type</param>
        /// <returns>Event type</returns>
        public static MMazeEventNames ConvertFullDescriptionToMMazeEventType (string description)
        {
            var type = typeof(MMazeEventNames);

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (MMazeEventNames)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (MMazeEventNames)field.GetValue(null);
                }
            }

            return MMazeEventNames.Undefined;
        }

        /// <summary>
        /// Converts a string to an event type.
        /// </summary>
        /// <param name="description">String description of an event type</param>
        /// <returns>Event type</returns>
        public static MMazeEventNames ConvertShortDescriptionToMMazeEventType(char short_desc)
        {
            var type = typeof(MMazeEventNames);

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(MMazeEventNamesShortDescriptionAttribute)) as MMazeEventNamesShortDescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.ShortDescription == short_desc)
                        return (MMazeEventNames)field.GetValue(null);
                }
            }

            return MMazeEventNames.Undefined;
        }

        /// <summary>
        /// Converts an event type to a string description.
        /// </summary>
        /// <param name="thresholdType">Event type</param>
        /// <returns>String description of the event type.</returns>
        public static string ConvertToDescription(MMazeEventNames thresholdType)
        {
            FieldInfo fi = thresholdType.GetType().GetField(thresholdType.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return thresholdType.ToString();
        }
    }
}

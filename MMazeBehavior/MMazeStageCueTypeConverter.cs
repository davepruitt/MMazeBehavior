using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MMazeBehavior
{
    public class MMazeStageCueTypeConverter
    {
        /// <summary>
        /// Converts a string to an cue type.
        /// </summary>
        /// <param name="description">String description of an cue type</param>
        /// <returns>Cue type</returns>
        public static MMazeStageCueType ConvertFullDescriptionToMMazeCueType(string description)
        {
            var type = typeof(MMazeStageCueType);

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description.Equals(description, StringComparison.InvariantCultureIgnoreCase))
                        return (MMazeStageCueType)field.GetValue(null);
                }
                else
                {
                    if (field.Name.Equals(description, StringComparison.InvariantCultureIgnoreCase))
                        return (MMazeStageCueType)field.GetValue(null);
                }
            }

            return MMazeStageCueType.Undefined;
        }

        /// <summary>
        /// Converts a cue type to a string description.
        /// </summary>
        /// <param name="thresholdType">Cue type</param>
        /// <returns>String description of the cue type.</returns>
        public static string ConvertToDescription(MMazeStageCueType thresholdType)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMazeBehavior
{
    /// <summary>
    /// Allows for adding a "short description" one-char attribute to each event name
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class MMazeEventNamesShortDescriptionAttribute : System.Attribute
    {
        public char ShortDescription = char.MinValue;

        public MMazeEventNamesShortDescriptionAttribute(char a)
        {
            ShortDescription = a;
        }
    }
}

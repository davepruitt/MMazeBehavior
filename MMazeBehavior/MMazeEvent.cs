using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMazeBehavior
{
    /// <summary>
    /// This object encapsulates a single MMaze event
    /// </summary>
    public class MMazeEvent
    {
        #region Constructors

        /// <summary>
        /// Empty constructor
        /// </summary>
        public MMazeEvent ()
        {
            //empty constructor
        }

        /// <summary>
        /// Constructor that takes an event time and type
        /// </summary>
        /// <param name="t"></param>
        /// <param name="e"></param>
        public MMazeEvent (DateTime t, MMazeEventNames e)
        {
            EventTime = t;
            EventType = e;
        }

        #endregion

        #region Properties

        public DateTime EventTime = DateTime.MinValue;
        public MMazeEventNames EventType = MMazeEventNames.Undefined;

        #endregion
    }
}

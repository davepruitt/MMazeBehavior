using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMazeBehavior
{
    /// <summary>
    /// A class that represents a stage in the MMaze
    /// </summary>
    public class MMazeStage
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public MMazeStage ()
        {
            //empty
        }

        #endregion

        #region Properties

        public string StageName = string.Empty;
        public string StageDescription = string.Empty;
        public List<Tuple<string, string>> SoundFiles = new List<Tuple<string, string>>();
        public List<int> StageRefractoryPeriods = new List<int>();
        public List<int> StageMaxSoundDelays = new List<int>();

        #endregion
    }
}

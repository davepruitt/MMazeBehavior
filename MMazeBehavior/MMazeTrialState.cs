using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMazeBehavior
{
    /// <summary>
    /// A set of states for a trial within the MMaze
    /// </summary>
    public enum MMazeTrialState
    {
        NoTrialStarted,
        LeftToRight,
        RightToLeft,
        LeftToRight_EnterLeftProx,
        RightToLeft_EnterRightProx,
        Undefined
    }
}

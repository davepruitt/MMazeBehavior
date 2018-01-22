using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMazeBehavior
{
    /// <summary>
    /// This defines how we determine when to deliver cues to the animal
    /// </summary>
    public enum CueDeliveryType
    {
        [Description("Cues are delivered at precisely timed intervals, independent of what the rat is doing.")]
        AbsoluteFixedIntervals,

        [Description("Cues are delivered according to a fixed interval schedule, but must be triggered by rat movement. The timer for the next cue does not start until the current cue is delivered.")]
        FixedIntervals_BehaviorDependent1,

        [Description("Cues are delivered according to a fixed interval schedule, but must be triggered by rat movement. The timer for the next cue starts immediately once the timer for the previous cue has finished.")]
        FixedIntervals_BehaviorDependent2,

        [Description("Cues are delivered according to a fixed interval schedule, but must be triggered by rat movement unless a maximum timer interval is reached.  The timer for the next cue does not start until the current cue is delivered.")]
        IntervalRange_BehaviorDependent,

        [Description("Cues are delivered on a random set of trials according to a pre-defined proportion.")]
        RandomProportionOfTrials,

        [Description("Undefined")]
        Undefined
    }
}

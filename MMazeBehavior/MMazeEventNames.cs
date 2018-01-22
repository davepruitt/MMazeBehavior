using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMazeBehavior
{
    /// <summary>
    /// List of event types in the M-Maze
    /// </summary>
    public enum MMazeEventNames
    {
        [Description("LEFT_NOSEPOKE_ENTER")]
        [MMazeEventNamesShortDescription('1')]
        LeftNosepokeEnter,

        [Description("LEFT_NOSEPOKE_LEAVE")]
        [MMazeEventNamesShortDescription('0')]
        LeftNosepokeLeave,

        [Description("RIGHT_NOSEPOKE_ENTER")]
        [MMazeEventNamesShortDescription('9')]
        RightNosepokeEnter,

        [Description("RIGHT_NOSEPOKE_LEAVE")]
        [MMazeEventNamesShortDescription('5')]
        RightNosepokeLeave,

        [Description("LEFT_PROX_ENTER")]
        [MMazeEventNamesShortDescription('L')]
        LeftProxEnter,

        [Description("LEFT_PROX_LEAVE")]
        [MMazeEventNamesShortDescription('l')]
        LeftProxLeave,

        [Description("RIGHT_PROX_ENTER")]
        [MMazeEventNamesShortDescription('R')]
        RightProxEnter,

        [Description("RIGHT_PROX_LEAVE")]
        [MMazeEventNamesShortDescription('r')]
        RightProxLeave,

        [Description("LEFT_FEEDER_TRIGGERED")]
        [MMazeEventNamesShortDescription('F')]
        LeftFeederTriggered,

        [Description("RIGHT_FEEDER_TRIGGERED")]
        [MMazeEventNamesShortDescription('f')]
        RightFeederTriggered,

        [Description("SOUND")]
        [MMazeEventNamesShortDescription(char.MaxValue)]
        SoundCue,

        [Description("UNDEFINED")]
        [MMazeEventNamesShortDescription(char.MinValue)]
        Undefined
    }
}

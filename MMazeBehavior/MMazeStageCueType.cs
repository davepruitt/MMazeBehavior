using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MMazeBehavior
{
    /// <summary>
    /// Defines when a cue occurs
    /// </summary>
    public enum MMazeStageCueType
    {
        [Description("Prox")]
        ProximitySensor,

        [Description("Nosepoke")]
        Nosepoke,

        [Description("")]
        Undefined
    }
}

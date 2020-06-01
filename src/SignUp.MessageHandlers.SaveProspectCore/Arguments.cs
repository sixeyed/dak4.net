using PowerArgs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignUp.MessageHandlers.SaveProspectCore
{
    [AllowUnexpectedArgs]
    public class Arguments
    {
        [ArgDefaultValue("listen")]
        [ArgShortcut("m")]
        [ArgDescription("Run mode")]
        public RunMode Mode { get; set; }
    }
}

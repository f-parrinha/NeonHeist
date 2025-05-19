using AI.Common;
using System.Collections.Generic;

namespace AI.Events
{
    public class OnTargetsUpdateArgs
    {
        public List<ScannedTarget> NewTargets { get; set; }
        public List<ScannedTarget> LostTargets { get; set; }
    }
}
using System.Collections.Generic;

namespace EngineGroupController
{
    public class EngineGroup
    {
        public float Throttle { get; set; }
        public string GroupId { get; private set; }

        public EngineGroup(string groupId)
        {
            GroupId = groupId;
            Throttle = 100.0f;
            EngineRefList = new List<EngineWrapper>();
            IsEnabled = true;
        }

        public bool ThrottleChanged { get; set; }
        public bool IsEnabled { get; set; }

        public IList<EngineWrapper> EngineRefList { get; private set; }
    }
}
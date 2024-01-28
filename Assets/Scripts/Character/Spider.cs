using UnityEngine;

namespace Model.Character
{
    public class Spider : Character
    {
        public SpiderLeg[] legs;

        [ContextMenu("Get 'SpiderLeg' Components")]
        private void GetSpiderLegComponents()
        {
            legs = this.GetComponentsInChildren<SpiderLeg>(true);
        }
    }
}
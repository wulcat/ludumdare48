using System;

namespace Assets.Scripts.ProceduralSystem
{
    [Serializable]
    public class IntRangeValue : RangeValue<int>
    {
        public IntRangeValue()
        {
            this.min = 0;
            this.max = 1;
        }
        public override int getRandom()
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
}

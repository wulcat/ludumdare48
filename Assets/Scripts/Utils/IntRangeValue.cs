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
        public IntRangeValue(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
        public override int GetRandom()
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
}

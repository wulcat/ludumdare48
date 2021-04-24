using System;

namespace Assets.Scripts.ProceduralSystem
{
    [Serializable]
    public class FloatRangeValue : RangeValue<float>
    {
        public FloatRangeValue()
        {
            this.min = 0;
            this.max = 1;
        }
        public FloatRangeValue(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
        public override float GetRandom()
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
}

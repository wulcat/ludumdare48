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
        public override float getRandom()
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
}

using UnityEngine;

namespace Assets.Scripts.ProceduralSystem
{
    public class FloatRangeValue : RangeValue<float>
    {
        public override float getRandom()
        {
            return Random.Range(min, max);
        }
    }
}

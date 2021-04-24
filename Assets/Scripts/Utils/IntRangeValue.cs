using UnityEngine;

namespace Assets.Scripts.ProceduralSystem
{
    public class IntRangeValue : RangeValue<int>
    {
        public override int getRandom()
        {
            return Random.Range(min, max);
        }
    }
}

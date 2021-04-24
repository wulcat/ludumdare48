using System;

namespace Assets.Scripts.ProceduralSystem
{
    [Serializable]
    public abstract class RangeValue<T>
    { 
        public T min;
        public T max;

        public abstract T GetRandom();       
    }
}

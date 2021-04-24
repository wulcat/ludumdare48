using System;
using UnityEngine;

namespace Assets.Scripts.ProceduralSystem
{
    public abstract class RangeValue<T>
    { 
        public T min;
        public T max;

        public abstract T getRandom();       
    }
}

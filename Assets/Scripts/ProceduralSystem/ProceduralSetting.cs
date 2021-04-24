using System;

namespace Assets.Scripts.ProceduralSystem
{
    [Serializable]
    public class ProceduralSetting
    {
        /// <summary>
        /// Use this to adjust the whole generation scale in unity (by default 1)
        /// </summary>
        public float scaleSize = 1f;
        public IntRangeValue minMaxRoomRange;
    }
}

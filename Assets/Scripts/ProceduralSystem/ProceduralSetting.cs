using System;
using UnityEngine;

namespace Assets.Scripts.ProceduralSystem
{
    [Serializable]
    public class ProceduralSetting
    {
        /// <summary>
        /// Pixel size for snapping elements
        /// </summary>
        public float tileSize = 4f;
        public GameObject simulationCubePrefab;
    }
}

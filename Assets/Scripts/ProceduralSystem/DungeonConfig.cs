using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.ProceduralSystem
{
    [Serializable]
    public class DungeonConfig
    {
        public float dungeonRadius = 5;
        public IntRangeValue roomGenerateCountRange = new IntRangeValue(20, 30);
        public FloatRangeValue roomGenerateSizeRange = new FloatRangeValue(0.5f, 1f);

        public GameObject floorPrefab;
        public GameObject wallPrefab;

        public FloatRangeValue enemyCountRange;
        public List<GameObject> enemyPrefabs;
        public List<GameObject> bossPrefabs;
    }
}

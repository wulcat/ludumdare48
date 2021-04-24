using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.ProceduralSystem
{
    [Serializable]
    public class DungeonConfig
    {
        public GameObject floorPrefab;
        public GameObject wallPrefab;

        public FloatRangeValue enemyCountRange;
        public List<GameObject> enemyPrefabs;
        public List<GameObject> bossPrefabs;
    }
}

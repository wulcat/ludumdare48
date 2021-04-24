using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.ProceduralSystem
{
    public class DungeonConfig
    {
        public RangeValue<float> enemyCountRange;
        public List<GameObject> enemyPrefabs;
        public List<GameObject> bossPrefabs;
    }
}

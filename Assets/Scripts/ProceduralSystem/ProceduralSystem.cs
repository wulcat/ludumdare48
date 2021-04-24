using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ProceduralSystem
{
    public class ProceduralSystem : MonoBehaviour
    {
        public ProceduralSetting setting;
        public Dungeon dungeon;
        public List<DungeonConfig> dungeonConfigs;

        /// <summary>
        /// Genearte the dungeon
        /// </summary>
        public Dungeon CreateDungeon(DungeonConfig config)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Destroy the left overs
        /// </summary>
        public void DestroyDungeon()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Pull all elements back to pool if possible
        /// </summary>
        public void CacheDungeon()
        {
            throw new System.NotImplementedException();
        }
    }
}

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

        public void Start()
        {
            CreateDungeon(this.dungeonConfigs[0]);
        }

        /// <summary>
        /// Genearte the dungeon
        /// </summary>
        public void CreateDungeon(DungeonConfig config)
        {
            var dungeon = new Dungeon(config , this.setting.tileSize);

            StartCoroutine(dungeon.Generate(this.setting.simulationCubePrefab));

            this.dungeon = dungeon;
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


        public void OnDrawGizmos()
        {
            if (this.dungeon == null || this.dungeon.rooms == null)
                return;

            Gizmos.DrawWireSphere(transform.position, this.dungeon.config.dungeonRadius);

            for (var i = 0; i < this.dungeon.rooms.Count; i++)
            {
                var room = this.dungeon.rooms[i];
                var rect = room.rect;

                if(room.isMain)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.blue;
                }

                Gizmos.DrawLine( // bottom line
                    new Vector3(rect.xMin , 0 , rect.yMin),
                    new Vector3(rect.xMax , 0 , rect.yMin)
                );
                Gizmos.DrawLine( // left line
                    new Vector3(rect.xMin , 0 , rect.yMin),
                    new Vector3(rect.xMin , 0 , rect.yMax)
                );
                Gizmos.DrawLine( // right line
                    new Vector3(rect.xMax , 0 , rect.yMin),
                    new Vector3(rect.xMax , 0 , rect.yMax)
                );
                Gizmos.DrawLine( // top line
                    new Vector3(rect.xMin , 0 , rect.yMax),
                    new Vector3(rect.xMax , 0 , rect.yMax)
                );

            }
        }
    }
}

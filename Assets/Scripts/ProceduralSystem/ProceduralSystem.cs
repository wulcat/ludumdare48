using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunatorSharp.Unity.Extensions;

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
            if (this.dungeon == null || this.dungeon.floorNodes == null)
                return;

            Gizmos.DrawWireSphere(transform.position, this.dungeon.config.dungeonRadius);

            for (var i = 0; i < this.dungeon.floorNodes.Count; i++)
            {
                var room = this.dungeon.floorNodes[i];
                var rect = room.rect;

                if (room.isMain)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.gray;
                }

                Gizmos.DrawLine( // bottom line
                    new Vector3(rect.xMin, 0, rect.yMin),
                    new Vector3(rect.xMax, 0, rect.yMin)
                );
                Gizmos.DrawLine( // left line
                    new Vector3(rect.xMin, 0, rect.yMin),
                    new Vector3(rect.xMin, 0, rect.yMax)
                );
                Gizmos.DrawLine( // right line
                    new Vector3(rect.xMax, 0, rect.yMin),
                    new Vector3(rect.xMax, 0, rect.yMax)
                );
                Gizmos.DrawLine( // top line
                    new Vector3(rect.xMin, 0, rect.yMax),
                    new Vector3(rect.xMax, 0, rect.yMax)
                );
            }

            //// Draw the triangulator output
            //if (this.dungeon.delaunator == null)
            //    return;

            //Gizmos.color = Color.green;

            //this.dungeon.delaunator.ForEachTriangleEdge(edge =>
            //{
            //    var pointA = edge.P.ToVector3();
            //    var pointB = edge.Q.ToVector3();

            //    Gizmos.DrawLine(new Vector3(pointA.x, 0 , pointA.y) , new Vector3(pointB.x, 0 , pointB.y));
            //});

            // Draw the min span output
            if (this.dungeon.treeEdgeNodes == null)
                return;

            //Gizmos.color = Color.black;

            //foreach (var edgeNode in this.dungeon.treeEdgeNodes)
            //{
            //    var pointA = edgeNode.a.rect.center;
            //    var pointB = edgeNode.b.rect.center;

            //    Gizmos.DrawLine(
            //        new Vector3(pointA.x, 0, pointA.y),
            //        new Vector3(pointB.x, 0, pointB.y)
            //    );
            //}

            //// Draw intersectin hallway lines
            //Gizmos.color = Color.red;
            //foreach (var edgeNode in this.dungeon.treeEdgeNodes)
            //{
            //    var aRect = edgeNode.a.rect;
            //    Gizmos.DrawLine( // bottom line
            //        new Vector3(aRect.xMin, 0, aRect.yMin),
            //        new Vector3(aRect.xMax, 0, aRect.yMin)
            //    );
            //    Gizmos.DrawLine( // left line
            //        new Vector3(aRect.xMin, 0, aRect.yMin),
            //        new Vector3(aRect.xMin, 0, aRect.yMax)
            //    );
            //    Gizmos.DrawLine( // right line
            //        new Vector3(aRect.xMax, 0, aRect.yMin),
            //        new Vector3(aRect.xMax, 0, aRect.yMax)
            //    );
            //    Gizmos.DrawLine( // top line
            //        new Vector3(aRect.xMin, 0, aRect.yMax),
            //        new Vector3(aRect.xMax, 0, aRect.yMax)
            //    );

            //    var bRect = edgeNode.b.rect;
            //    Gizmos.DrawLine( // bottom line
            //        new Vector3(bRect.xMin, 0, bRect.yMin),
            //        new Vector3(bRect.xMax, 0, bRect.yMin)
            //    );
            //    Gizmos.DrawLine( // left line
            //        new Vector3(bRect.xMin, 0, bRect.yMin),
            //        new Vector3(bRect.xMin, 0, bRect.yMax)
            //    );
            //    Gizmos.DrawLine( // right line
            //        new Vector3(bRect.xMax, 0, bRect.yMin),
            //        new Vector3(bRect.xMax, 0, bRect.yMax)
            //    );
            //    Gizmos.DrawLine( // top line
            //        new Vector3(bRect.xMin, 0, bRect.yMax),
            //        new Vector3(bRect.xMax, 0, bRect.yMax)
            //    );
            //}


            // Draw intersectin hallway lines
            Gizmos.color = Color.cyan;
            foreach (var edgeNode in this.dungeon.treeEdgeNodes)
            {
                var pointA = edgeNode.a.rect.center;
                var pointB = edgeNode.b.rect.center;
                var intersectionPoint = edgeNode.FindRectanularIntersection();

                Gizmos.DrawLine(
                    new Vector3(pointA.x, 0, pointA.y),
                    new Vector3(intersectionPoint.x, 0, intersectionPoint.y)
                );

                Gizmos.DrawLine(
                    new Vector3(pointB.x, 0, pointB.y),
                    new Vector3(intersectionPoint.x, 0, intersectionPoint.y)
                );

                Gizmos.DrawWireSphere(new Vector3(intersectionPoint.x, 0, intersectionPoint.y), 1f);
            }

            // Draw main room joints
            Gizmos.color = Color.cyan;

            foreach (var joint in this.dungeon.jointRects)
            {
                var rect = joint;
                Gizmos.DrawLine( // bottom line
                    new Vector3(rect.xMin, 0, rect.yMin),
                    new Vector3(rect.xMax, 0, rect.yMin)
                );
                Gizmos.DrawLine( // left line
                    new Vector3(rect.xMin, 0, rect.yMin),
                    new Vector3(rect.xMin, 0, rect.yMax)
                );
                Gizmos.DrawLine( // right line
                    new Vector3(rect.xMax, 0, rect.yMin),
                    new Vector3(rect.xMax, 0, rect.yMax)
                );
                Gizmos.DrawLine( // top line
                    new Vector3(rect.xMin, 0, rect.yMax),
                    new Vector3(rect.xMax, 0, rect.yMax)
                );
            }

            Gizmos.color = Color.green;

            foreach (var hallway in this.dungeon.hallWayRects)
            {
                var rect = hallway;
                Gizmos.DrawLine( // bottom line
                    new Vector3(rect.xMin, 0, rect.yMin),
                    new Vector3(rect.xMax, 0, rect.yMin)
                );
                Gizmos.DrawLine( // left line
                    new Vector3(rect.xMin, 0, rect.yMin),
                    new Vector3(rect.xMin, 0, rect.yMax)
                );
                Gizmos.DrawLine( // right line
                    new Vector3(rect.xMax, 0, rect.yMin),
                    new Vector3(rect.xMax, 0, rect.yMax)
                );
                Gizmos.DrawLine( // top line
                    new Vector3(rect.xMin, 0, rect.yMax),
                    new Vector3(rect.xMax, 0, rect.yMax)
                );
            }
        }
    }
}

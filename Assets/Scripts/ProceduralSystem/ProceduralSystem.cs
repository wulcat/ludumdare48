using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunatorSharp.Unity.Extensions;
using ClipperLib;

namespace Assets.Scripts.ProceduralSystem
{
    public class ProceduralSystem : MonoBehaviour
    {
        public ProceduralSetting setting;

        private Dungeon dungeon;
        private Dungeon nextDungeon;

        public List<DungeonConfig> dungeonConfigs;
        public static ProceduralSystem Instance;

        private float currentYAxis = 0;
        public bool pIsReady
        {
            get
            {
                if(this.dungeon == null)
                {
                    return false;
                }
                else
                {
                    return this.dungeon.isReady;
                }
                
            }
        }

        private void Awake()
        {
            ProceduralSystem.Instance = this;
        }
        public void Start()
        {
            CreateDungeon(this.dungeonConfigs[0] , true);
            //CreateDungeon(this.dungeonConfigs[0], false);
        }

        private void Update()
        {
            if (!pIsReady)
                return;

            if(this.dungeon.isReady && this.nextDungeon == null)
            {
                CreateDungeon(this.dungeonConfigs[0], false);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                GoToNextLevel();
            }
        }

        /// <summary>
        /// Genearte the dungeon
        /// </summary>
        public void CreateDungeon(DungeonConfig config , bool isSpawn)
        {
            var dungeon = new Dungeon(config , this.setting.tileSize, this.setting.obstacleMask);

            StartCoroutine(dungeon.Generate(this.setting.simulationCubePrefab,this.currentYAxis , isSpawn));

            if (isSpawn)
            {
                this.dungeon = dungeon;
            }
            else
            {
                this.nextDungeon = dungeon;
            }
            this.currentYAxis += 50;
        }


        public void GoToNextLevel()
        {
            if (this.nextDungeon != null && this.nextDungeon.isReady)
            {
                this.dungeon = this.nextDungeon;
                this.dungeon.Spawn(this.currentYAxis - 50);


                this.nextDungeon = null;
            }
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

            //Gizmos.DrawWireSphere(transform.position, this.dungeon.config.dungeonRadius);

            //DrawRooms();
            //DrawTriangulation();

            // Draw the min span output
            if (this.dungeon.treeEdgeNodes == null)
                return;

            DrawMinTreeSpan();
            //DrawIntersections();
            //DrawMainRoom();
            //DrawHallwayRect();
            //DrawRoomJoints();

            //DrawClip();
            DrawEntryExitPoint();

            //Gizmos.DrawWireCube(this.testPOint, new Vector3(4, 4, 4));
            //Gizmos.DrawWireSphere(this.testPOint + Vector3.zero * 0, 4);
            //if (Physics.SphereCast(this.testPOint, 4, Vector3.zero, out hit, 0, this.setting.obstacleMask))
        }

        private void DrawRooms()
        {
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
        }
        private void DrawMainRoom() {
            // Draw intersectin hallway lines
            Gizmos.color = Color.red;
            foreach (var edgeNode in this.dungeon.treeEdgeNodes)
            {
                var aRect = edgeNode.a.rect;
                Gizmos.DrawLine( // bottom line
                    new Vector3(aRect.xMin, 0, aRect.yMin),
                    new Vector3(aRect.xMax, 0, aRect.yMin)
                );
                Gizmos.DrawLine( // left line
                    new Vector3(aRect.xMin, 0, aRect.yMin),
                    new Vector3(aRect.xMin, 0, aRect.yMax)
                );
                Gizmos.DrawLine( // right line
                    new Vector3(aRect.xMax, 0, aRect.yMin),
                    new Vector3(aRect.xMax, 0, aRect.yMax)
                );
                Gizmos.DrawLine( // top line
                    new Vector3(aRect.xMin, 0, aRect.yMax),
                    new Vector3(aRect.xMax, 0, aRect.yMax)
                );

                var bRect = edgeNode.b.rect;
                Gizmos.DrawLine( // bottom line
                    new Vector3(bRect.xMin, 0, bRect.yMin),
                    new Vector3(bRect.xMax, 0, bRect.yMin)
                );
                Gizmos.DrawLine( // left line
                    new Vector3(bRect.xMin, 0, bRect.yMin),
                    new Vector3(bRect.xMin, 0, bRect.yMax)
                );
                Gizmos.DrawLine( // right line
                    new Vector3(bRect.xMax, 0, bRect.yMin),
                    new Vector3(bRect.xMax, 0, bRect.yMax)
                );
                Gizmos.DrawLine( // top line
                    new Vector3(bRect.xMin, 0, bRect.yMax),
                    new Vector3(bRect.xMax, 0, bRect.yMax)
                );
            }
        }
        private void DrawTriangulation()
        {
            // Draw the triangulator output
            if (this.dungeon.delaunator == null)
                return;

            Gizmos.color = Color.green;

            this.dungeon.delaunator.ForEachTriangleEdge(edge =>
            {
                var pointA = edge.P.ToVector3();
                var pointB = edge.Q.ToVector3();

                Gizmos.DrawLine(new Vector3(pointA.x, 0, pointA.y), new Vector3(pointB.x, 0, pointB.y));
            });
        }
        private void DrawMinTreeSpan()
        {
            Gizmos.color = Color.black;

            foreach (var edgeNode in this.dungeon.treeEdgeNodes)
            {
                var pointA = edgeNode.a.rect.center;
                var pointB = edgeNode.b.rect.center;

                Gizmos.DrawLine(
                    new Vector3(pointA.x, 0, pointA.y),
                    new Vector3(pointB.x, 0, pointB.y)
                );
            }
        }
        private void DrawIntersections() {
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
        }
        private void DrawHallwayRect() {
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
        private void DrawRoomJoints() {
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
        }
        private void DrawClip()
        {
            if (this.dungeon.clipperOutput == null)
                return;

            Gizmos.color = Color.black;

            foreach (var path in this.dungeon.clipperOutput)
            {
                IntPoint previousPoint = path[0];
                for (var i = 1; i < path.Count; i++)
                {
                    Gizmos.DrawLine(
                        new Vector3(previousPoint.X, 0, previousPoint.Y),
                        new Vector3(path[i].X, 0, path[i].Y)
                    );

                    previousPoint = path[i];
                }

                // close the lines
                Gizmos.DrawLine(
                    new Vector3(path[0].X, 0, path[0].Y),
                    new Vector3(path[path.Count-1].X, 0, path[path.Count - 1].Y)
                );

                foreach (var point in path)
                {
                    Gizmos.DrawWireSphere(new Vector3(point.X, 0.5f, point.Y), 1f);
                }
            }

        }
        private void DrawEntryExitPoint()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(this.dungeon.entryPoint, 2f);
            Gizmos.DrawWireSphere(this.dungeon.exitPoint, 2f);
        }
    }
}

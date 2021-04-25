using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using DelaunatorSharp;
using DelaunatorSharp.Unity.Extensions;

// References: https://www.gamasutra.com/blogs/AAdonaac/20150903/252889/Procedural_Dungeon_Generation_Algorithm.php

namespace Assets.Scripts.ProceduralSystem
{
    [Serializable]
    public class Dungeon : IDungeon
    {
        public Gateway startGateway;
        public Gateway exitGateway;
        /// <summary>
        /// for entry to multiple dungeons
        /// </summary>
        public List<Gateway> subDungeons;

        public int id { get ; set ; }
        public IDungeon parentDungeon { get ; set ; }
        public List<FloorNode> floorNodes;

        public DungeonConfig config;

        // snapping to pixels
        private float mTileSize = 1;

        // main room selection
        private float mWidthMean = 0f;
        private float mHeightMean = 0f;

        //
        public Delaunator delaunator;
        public HashSet<EdgeNode> roomGraph;
        public List<EdgeNode> treeEdgeNodes;

        public Dungeon(DungeonConfig config , float tileSize)
        {
            this.config = config;
            this.mTileSize = tileSize;
        }

        public IEnumerator Generate(GameObject simulationCube)
        {
            GenerateRoom();
            yield return SeparateRoom(simulationCube);
            DetermineMainRooms();
            TriangulateRoom();
            GenerateGraph();
            MinimumSpanningTree(this.roomGraph);
            DetermineHallway();
        }

        private void GenerateRoom()
        {
            this.floorNodes = new List<FloorNode>();

            var roomCount = this.config.roomGenerateCountRange.GetRandom();
            var roomSize = this.config.roomGenerateSizeRange;
            
            for(var i = 0; i < roomCount; i++)
            {
                var position = GetRandomPointInCircle(this.config.dungeonRadius);
                var rect = new Rect(position.x, position.y, roomSize.GetRandom(), roomSize.GetRandom());
                var roomNode = new FloorNode(rect);

                this.floorNodes.Add(roomNode);
            }
        }

        private IEnumerator SeparateRoom(GameObject simulationCube)
        {
            var rigidbodies = new List<Rigidbody2D>();
            var shouldBreak = true;

            for(var i = 0; i < this.floorNodes.Count ; i++)
            {
                var room = this.floorNodes[i];
                var clone = GameObject.Instantiate(simulationCube, new Vector3(room.rect.center.x, room.rect.center.y, 0), Quaternion.identity).GetComponent<Rigidbody2D>();
                clone.transform.localScale = new Vector3(room.rect.width, room.rect.height, 1);

                rigidbodies.Add(clone);
            }

            while (shouldBreak)
            {
                shouldBreak = false;
                yield return new WaitForSeconds(1f);


                for (var i = 0; i < rigidbodies.Count; i++)
                {
                    var body = rigidbodies[i];
                    if (!body.IsSleeping())
                    {
                        shouldBreak = true;
                    }
                    var position = body.transform.position;

                    //body.transform.position = new Vector3(RoundM(position.x, this.mTileSize), RoundM(position.y, this.mTileSize), position.z);
                }
            }

            for (var i = 0; i < rigidbodies.Count; i++)
            {
                var body = rigidbodies[i];
                var position = body.transform.position;
                var room = this.floorNodes[i];

                var x = position.x - room.rect.width / 2; 
                var y = position.y - room.rect.height / 2;

                //this.rooms[i] = new Rect(RoundM(x , this.mTileSize) , RoundM(y , this.mTileSize) , room.width, room.height);
                this.floorNodes[i].rect = new Rect(x , y , room.rect.width, room.rect.height);

                this.mWidthMean += room.rect.width;
                this.mHeightMean += room.rect.height;

                GameObject.Destroy(body.gameObject);
            }

            Debug.Log("Everybody is sleeping now");
        }

        private void DetermineMainRooms(float minThreshold = 1.25f)
        {
            var roomCount = this.floorNodes.Count - 1;
            var mainRoomMinWidth = (this.mWidthMean / roomCount) * minThreshold;
            var mainRoomMinHeight = (this.mHeightMean / roomCount) * minThreshold;
            var mainRoomCount = 0;

            for (var i = 0; i < this.floorNodes.Count; i++)
            {
                var room = this.floorNodes[i];
                if(room.rect.width > mainRoomMinWidth && room.rect.height > mainRoomMinHeight)
                {
                    room.isMain = true;
                    mainRoomCount++;
                }
            }

            // Make sure have min 3 main rooms for triangulation
            if (mainRoomCount < this.config.minMainRoomCount)
                DetermineMainRooms(minThreshold - 0.05f);
        }

        private void TriangulateRoom()
        {
            List<IPoint> points = new List<IPoint>();
            foreach(var room in this.floorNodes)
            {
                if(room.isMain)
                    points.Add(new Point(room.rect.center.x, room.rect.center.y));
            }

            this.delaunator = new Delaunator(points.ToArray());
        }

        private void GenerateGraph()
        {
            this.roomGraph = new HashSet<EdgeNode>();

            var triangles = this.delaunator.GetTriangles();
            foreach(var triangle in triangles)
            {
                var mainNodes = new List<FloorNode>();
                foreach(var point in triangle.Points)
                {
                    mainNodes.Add(this.floorNodes.Find(node => Vector3.Distance(node.rect.center , point.ToVector3()) < 0.3f));
                }

                var edges = new List<EdgeNode>
                {
                    new EdgeNode(mainNodes[0],mainNodes[1]),
                    new EdgeNode(mainNodes[1],mainNodes[2]),
                    new EdgeNode(mainNodes[0],mainNodes[2])
                };

                this.roomGraph.UnionWith(edges);
            }
        }


        public void MinimumSpanningTree(IEnumerable<EdgeNode> graph)
        {
            List<EdgeNode> ans = new List<EdgeNode>();

            List<EdgeNode> edges = new List<EdgeNode>(graph);
            edges.Sort(EdgeNode.LengthComparison);

            HashSet<FloorNode> points = new HashSet<FloorNode>();
            foreach (var edge in edges)
            {
                points.Add(edge.a);
                points.Add(edge.b);
            }

            Dictionary<FloorNode, FloorNode> parents = new Dictionary<FloorNode, FloorNode>();
            foreach (var point in points)
                parents[point] = point;

            FloorNode UnionFind(FloorNode x)
            {
                if (parents[x] != x)
                    parents[x] = UnionFind(parents[x]);
                return parents[x];
            }

            foreach (var edge in edges)
            {
                var x = UnionFind(edge.a);
                var y = UnionFind(edge.b);
                if (x != y)
                {
                    ans.Add(edge);
                    parents[x] = y;
                }
            }

            this.treeEdgeNodes = ans;
        }

        public List<Vector2> interesectionPoints = new List<Vector2>();
        public void DetermineHallway()
        {
            var hallWayThickness = 5f;

            this.hallWayRects = new List<Rect>();
            this.jointRects = new List<Rect>();
           
            foreach(var edge in this.treeEdgeNodes)
            {
                var aRect = edge.a.rect;
                var bRect = edge.b.rect;
                var intersectionPoint = edge.FindRectanularIntersection();
                var joint = new Rect(intersectionPoint.x - hallWayThickness / 2, intersectionPoint.y - hallWayThickness / 2, hallWayThickness, hallWayThickness);

                var aHallway = EdgeNode.RectBetweemTwoRects(aRect, joint , hallWayThickness);
                var bHallway = EdgeNode.RectBetweemTwoRects(bRect, joint , hallWayThickness);

                this.jointRects.Add(joint);
                this.hallWayRects.Add(aHallway);
                this.hallWayRects.Add(bHallway);
            }
        }

        public List<Rect> hallWayRects;
        public List<Rect> jointRects;

        /// <summary>
        /// Check out TKdev's algorithm
        /// </summary>
        private Vector2 GetRandomPointInCircle(float radius)
        {
            float t = 2 * Mathf.PI * UnityEngine.Random.Range(0, 1f);
            float u = UnityEngine.Random.Range(0, 1f) + UnityEngine.Random.Range(0, 1f);
            float r = 0f;

            if (u > 1)
            {
                r = 2 - u;
            }
            else
            {
                r = u;
            }

            r *= radius;

            return new Vector2(
                RoundM(r * Mathf.Cos(t) , this.mTileSize),
                RoundM(r * Mathf.Sin(t) , this.mTileSize)
            );
        }

        private float RoundM(float value , float pixelSize){
            return Mathf.Floor(((value + pixelSize - 1)/pixelSize))*pixelSize;
        }

    }
}

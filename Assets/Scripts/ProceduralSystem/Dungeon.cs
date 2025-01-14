﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using DelaunatorSharp;
using DelaunatorSharp.Unity.Extensions;
using ClipperLib;

// References: https://www.gamasutra.com/blogs/AAdonaac/20150903/252889/Procedural_Dungeon_Generation_Algorithm.php

namespace Assets.Scripts.ProceduralSystem
{
    using Paths = List<List<IntPoint>>;

    [Serializable]
    public class Dungeon : IDungeon
    {
        public bool isReady = false;
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

        [HideInInspector] public Vector3 entryPoint;
        [HideInInspector] public Vector3 exitPoint;
        [HideInInspector] public List<Vector3> spawnPoints;

        // snapping to pixels
        private float mTileSize = 1;


        // main room selection
        private float mWidthMean = 0f;
        private float mHeightMean = 0f;

        //
        public Delaunator delaunator;
        public HashSet<EdgeNode> roomGraph;
        public List<EdgeNode> treeEdgeNodes;
        public List<Rect> hallWayRects;
        public List<Rect> jointRects;
        public Paths clipperOutput;

        private List<Transform> wallClones;
        private List<GameObject> floorClones;
        private List<GameObject> enemyClones;

        private GameObject mDungeonContainer;
        private LayerMask obstacleMask;

        public Dungeon(DungeonConfig config , float tileSize, LayerMask obstacleMask) {
            this.config = config;
            this.mTileSize = tileSize;
            this.obstacleMask = obstacleMask;

            this.mDungeonContainer = new GameObject("Dungeon");
        }

        public IEnumerator Generate(GameObject simulationCube, float yAxis , bool isSpawn)
        {
            GenerateRoom();
            yield return SeparateRoom(simulationCube);
            yield return DetermineMainRooms();
            TriangulateRoom();
            GenerateGraph();
            MinimumSpanningTree(this.roomGraph);
            DetermineHallway();
            ExtendRectSize(1.2f);
            ClipPaths();

            FindEntryExitPoint();
            FindRoomSpawnPoints();

            InstantiateWalls(yAxis);
            yield return InstantiateFloors(yAxis);
            InstantiateExitPortal(yAxis);

            //SpawnPlayer(yAxis);
            //SpawnEnemies(yAxis);
            if(isSpawn)
                Spawn(yAxis);

            this.isReady = true;
        }

        public void Spawn(float yAxis)
        {
            SpawnPlayer(yAxis);
            SpawnEnemies(yAxis);
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
                var roomNode = new FloorNode(i , rect);

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
                    //var position = body.transform.position;
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

        }

        private IEnumerator DetermineMainRooms(float minThreshold = 1.25f , int count = 0)
        {
            yield return new WaitForEndOfFrame(); 

            count++;
            //Debug.Log(minThreshold + " : " + count);
            var roomCount = this.floorNodes.Count - 1;
            var mainRoomMinWidth = (this.mWidthMean / roomCount) * minThreshold;
            var mainRoomMinHeight = (this.mHeightMean / roomCount) * minThreshold;
            var mainRoomCount = 0;

            for (var i = 0; i < this.floorNodes.Count; i++)
            {
                var room = this.floorNodes[i];
                if(room.rect.width > mainRoomMinWidth && room.rect.height > mainRoomMinHeight)
                {
                    if(CheckDistanceWithAllMainRooms(room))
                    {
                        room.isMain = true;
                        mainRoomCount++;
                    }
                }
            }

            // Make sure have min 3 main rooms for triangulation
            if (mainRoomCount < this.config.minMainRoomCount)
                yield return DetermineMainRooms(minThreshold - 0.01f , count);
        }

        private bool CheckDistanceWithAllMainRooms(FloorNode node)
        {
            var isFar = true;
            for (var j = 0; j < this.floorNodes.Count; j++)
            {
                var room = this.floorNodes[j];
                if (room.id != node.id && room.isMain)
                {
                    if (Vector3.Distance(room.rect.center, node.rect.center) < this.config.distanceBetweenMainRoom)
                    {
                        isFar = false;
                        break;
                    }
                }
            }

            return isFar;
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

        
        public void DetermineHallway()
        {
            var hallWayThickness = this.config.hallWayThickness;

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

        
        
        private void ExtendRectSize(float extendSize)
        {
            for(var i = 0; i  < this.jointRects.Count; i++)
            {
                var joint = this.jointRects[i];
                var width = joint.width * extendSize;
                var height = joint.height* extendSize;

                this.jointRects[i] = new Rect(joint.center.x - width / 2 , joint.center.y - height / 2, width , height);
            }

            for (var i = 0; i < this.hallWayRects.Count; i++)
            {
                var hallway = this.hallWayRects[i];
                var width = hallway.width * extendSize;
                var height = hallway.height * extendSize;

                this.hallWayRects[i] = new Rect(hallway.center.x - width / 2, hallway.center.y - height / 2, width, height);
            }

            for (var i = 0; i < this.floorNodes.Count; i++)
            {
                if (this.floorNodes[i].isMain)
                {
                    var floor = this.floorNodes[i].rect;

                    var width = floor.width * extendSize;
                    var height = floor.height * extendSize;

                    this.floorNodes[i].rect = new Rect(floor.center.x - width / 2, floor.center.y - height / 2, width, height);
                }
            }
        }

        private void ClipPaths()
        {
            var clipper = new Clipper();

            foreach (var hallWayRect in this.hallWayRects)
            {
                clipper.AddPath(
                    new List<IntPoint>
                    {
                        new IntPoint(hallWayRect.xMin , hallWayRect.yMin) ,
                        new IntPoint(hallWayRect.xMax , hallWayRect.yMin) ,
                        new IntPoint(hallWayRect.xMax , hallWayRect.yMax) ,
                        new IntPoint(hallWayRect.xMin , hallWayRect.yMax)
                    },
                    PolyType.ptClip,
                    true
                );
            }

            foreach (var jointRect in this.jointRects)
            {
                clipper.AddPath(
                    new List<IntPoint>
                    {
                        new IntPoint(jointRect.xMin , jointRect.yMin) ,
                        new IntPoint(jointRect.xMax , jointRect.yMin) ,
                        new IntPoint(jointRect.xMax , jointRect.yMax) ,
                        new IntPoint(jointRect.xMin , jointRect.yMax)
                    },
                    PolyType.ptClip,
                    true
                );
            }

            foreach (var floor in this.floorNodes)
            {
                if (floor.isMain)
                {
                    var rect = floor.rect;
                    
                    clipper.AddPath(
                        new List<IntPoint>
                        {
                        new IntPoint(rect.xMin , rect.yMin) ,
                        new IntPoint(rect.xMax , rect.yMin) ,
                        new IntPoint(rect.xMax , rect.yMax) ,
                        new IntPoint(rect.xMin , rect.yMax)
                        },
                        PolyType.ptSubject,
                        true
                    );
                }
            }


            Paths solution = new Paths();

            clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive);

            this.clipperOutput = solution;

            //Debug.Log("Tree Count: "+solution.Count);
        }

        private void InstantiateWalls(float yAxis)
        {
            this.wallClones = new List<Transform>();

            //var distanceBetweenWalls = 4;
            foreach (var path in this.clipperOutput)
            {
                IntPoint previousPoint = path[0];
                for (var i = 1; i < path.Count; i++)
                {
                    InstantiateWallBetweenPoints(
                        new Vector2(previousPoint.X, previousPoint.Y),
                        new Vector2(path[i].X, path[i].Y),
                        this.mTileSize,
                        yAxis
                    );

                    previousPoint = path[i];
                }

                InstantiateWallBetweenPoints(
                    new Vector2(path[0].X, path[0].Y),
                    new Vector2(path[path.Count - 1].X, path[path.Count - 1].Y) ,
                    this.mTileSize,
                    yAxis
                );
            }
        }

    
        private IEnumerator InstantiateFloors(float yAxis)
        {
            this.floorClones = new List<GameObject>();

            var position = this.treeEdgeNodes[0].a.rect.center;

            yield return CheckAndExpand(new Vector3(RoundM(position.x,1) , yAxis, RoundM(position.y,1)));

            foreach(var floor in this.floorClones)
            {
                GameObject.Destroy(floor.GetComponent<BoxCollider>());
            }
        }

        private void InstantiateExitPortal(float yAxis)
        {
            

            foreach(var floor in this.floorClones)
            {
                if(Vector3.Distance(floor.transform.position , new Vector3(exitPoint.x , yAxis , exitPoint.z)) < 3)
                {
                    var clone = GameObject.Instantiate(this.config.portalPrefab);

                    clone.transform.position = floor.transform.position;
                    GameObject.Destroy(floor);
                    break;
                }
            }
        }

        private IEnumerator CheckAndExpand(Vector3 point)
        {
            var casts = Physics.SphereCastAll(point, 0.5f , Vector3.up, this.obstacleMask);
            if (casts.Length == 0)
            {
                var clone = GameObject.Instantiate(this.config.floorPrefab);
                clone.transform.position = point;
                clone.transform.SetParent(this.mDungeonContainer.transform);

                this.floorClones.Add(clone);

                var leftPoint = new Vector3(point.x - 4, point.y, point.z);
                var rightPoint = new Vector3(point.x + 4, point.y, point.z);
                var upPoint = new Vector3(point.x, point.y, point.z + 4);
                var downPoint = new Vector3(point.x, point.y, point.z - 4);

                yield return null;

                yield return CheckAndExpand(upPoint);
                yield return CheckAndExpand(downPoint);
                yield return CheckAndExpand(leftPoint);
                yield return CheckAndExpand(rightPoint);       
            }
            else
            {
                var isWall = false;
                for (var i = 0; i < casts.Length; i++)
                {
                    if (casts[0].transform.tag == "Wall")
                    {
                        isWall = true;
                    }
                }
                if(isWall)
                {
                    var clone = GameObject.Instantiate(this.config.floorPrefab);
                    clone.transform.position = point;
                    clone.transform.SetParent(this.mDungeonContainer.transform);
                }
            }
        }

        private void FindEntryExitPoint()
        {
            var currentLeastFloorNode = this.floorNodes[0];
            var currentExtremeFloorNode = this.floorNodes[0];

            for(var i = 1; i < this.floorNodes.Count;i++)
            {
                if (this.floorNodes[i].isMain)
                {
                    if (this.floorNodes[i].rect.center.x < currentLeastFloorNode.rect.center.x)
                    {
                        currentLeastFloorNode = this.floorNodes[i];
                    }

                    if (this.floorNodes[i].rect.center.x > currentExtremeFloorNode.rect.center.x)
                    {
                        currentExtremeFloorNode = this.floorNodes[i];
                    }
                }
            }

            currentLeastFloorNode.isUsed = true;
            currentExtremeFloorNode.isUsed = true;

            this.entryPoint = new Vector3(currentLeastFloorNode.rect.center.x , 0 , currentLeastFloorNode.rect.center.y);
            this.exitPoint = new Vector3(currentExtremeFloorNode.rect.center.x, 0, currentExtremeFloorNode.rect.center.y);
        }

        private void FindRoomSpawnPoints()
        {
            this.spawnPoints = new List<Vector3>();

            foreach (var floor in this.floorNodes)
            {
                if (floor.isMain && !floor.isUsed)
                {   
                    spawnPoints.Add(new Vector3(floor.rect.center.x, 0, floor.rect.center.y));   
                }
            }
        }

        private void SpawnPlayer(float yAxis) {
            var player = GameManager.instance.player.transform;
            player.position = new Vector3(this.entryPoint.x , yAxis + 1, this.entryPoint.z);
            player.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        }

        private void SpawnEnemies(float yAxis)
        {
            this.enemyClones = new List<GameObject>();
            for (var i = 0; i < this.spawnPoints.Count; i++)
            {
                var spawnPosition = spawnPoints[i];

                var clone = UnityEngine.Object.Instantiate(config.enemyPrefabs[UnityEngine.Random.Range(0, config.enemyPrefabs.Count)], spawnPoints[i], Quaternion.identity);//ObjectPooler.instance.SpawnFromPool("Enemy", this.spawnPoints[i], Quaternion.identity, ObjectPooler.instance.pools[4]);
                if(clone.GetComponentInChildren<EnemyShoot>())
                {
                    clone.GetComponentInChildren<EnemyShoot>().Initialize();
                }

                clone.transform.position = new Vector3(spawnPosition.x, 0.2f + yAxis + clone.GetComponent<EnemyData>().aboveGround, spawnPosition.z);
                this.enemyClones.Add(clone);
            }
        }

        public void DestroyEnemies()
        {
            foreach(var enemy in this.enemyClones)
            {
                GameObject.Destroy(enemy);
            }
        }

        // ---------------------- extra functions

        public static bool AreNumberEqual(float a , float b , float diff = 0.3f)
        {
            return Mathf.Abs(Mathf.Abs(a) - Mathf.Abs(b)) < diff;
        }

        private void InstantiateWallBetweenPoints(Vector2 pointA , Vector2 pointB , float distanceBetween , float yAxis)
        {
            var angle = 45f;
            var direction = 1;

            if (Mathf.Abs(Mathf.Abs(pointA.y) - Mathf.Abs(pointB.y)) < 0.3f)
            {
                if (pointA.x < pointB.x)
                {
                    angle = 0f;
                    direction = 1;
                }
                else
                {
                    angle = 0f;
                    direction = -1;
                }
            }

            if (Mathf.Abs(Mathf.Abs(pointA.x) - Mathf.Abs(pointB.x)) < 0.3f)
            {
                if (pointA.y < pointB.y)
                {
                    angle = 90f;
                    direction = 1;
                }
                else
                {
                    angle = 90f;
                    direction = -1;
                }
            }

            angle *= Mathf.Deg2Rad;

            var distance = Vector3.Distance(pointA, pointB);
            var radius = 0f;

            while (radius < distance)
            {
                var newPoint = new Vector3(
                    pointA.x + direction * Mathf.Cos(angle) * radius,
                    0,
                    pointA.y + direction * Mathf.Sin(angle) * radius
                );

                var wall = GameObject.Instantiate(this.config.wallPrefab);
                wall.transform.position = new Vector3(RoundM(newPoint.x , 1), yAxis, RoundM(newPoint.z,1));
                wall.transform.SetParent(this.mDungeonContainer.transform);

                this.wallClones.Add(wall.transform);

                radius += distanceBetween;
            }
        }

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

using UnityEngine;
using System;
using System.Collections.Generic;

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
        public List<Rect> rooms;

        public DungeonConfig config;
        private float mTileSize = 1;

        public Dungeon(DungeonConfig config , float tileSize)
        {
            this.config = config;
            this.mTileSize = tileSize;

            GenerateRoom();
            //SeparateRoom();
            //DetermineMainRooms();
            //TriangulateRoom();
            //GenerateGraph();
            //MinSpanningTree();
            //DetermineHallway();
            //TriangulateRoom();
            //GenerateGraph();
            //MinSpanningTree();
            //DetermineHallway();
        }
        public void GenerateRoom()
        {
            this.rooms = new List<Rect>();

            var roomCount = this.config.roomGenerateCountRange.GetRandom();
            var roomSize = this.config.roomGenerateSizeRange;
            
            for(var i = 0; i < roomCount; i++)
            {
                var position = GetRandomPointInCircle(this.config.dungeonRadius);
                var rect = new Rect(position.x, position.y, roomSize.GetRandom(), roomSize.GetRandom());

                this.rooms.Add(rect);
            }
        }

        public void SeparateRoom()
        {
            throw new System.NotImplementedException();
        }

        public void DetermineMainRooms()
        {
            throw new System.NotImplementedException();
        }

        public void TriangulateRoom()
        {
            throw new System.NotImplementedException();
        }

        public void GenerateGraph()
        {
            throw new System.NotImplementedException();
        }

        public void MinSpanningTree()
        {
            throw new System.NotImplementedException();
        }

        public void DetermineHallway()
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// TKdev's algorithm
        /// </summary>
        public Vector2 GetRandomPointInCircle(float radius)
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

        public float RoundM(float value , float pixelSize){
            return Mathf.Floor(((value + pixelSize - 1)/pixelSize))*pixelSize;
        }
    }
}

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

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
        public List<RoomNode> rooms;

        public DungeonConfig config;
        private float mTileSize = 1;
        private float mWidthMean = 0f;
        private float mHeightMean = 0f;

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
            //TriangulateRoom();
            //GenerateGraph();
            //MinSpanningTree();
            //DetermineHallway();
            //TriangulateRoom();
            //GenerateGraph();
            //MinSpanningTree();
            //DetermineHallway();
        }

        private void GenerateRoom()
        {
            this.rooms = new List<RoomNode>();

            var roomCount = this.config.roomGenerateCountRange.GetRandom();
            var roomSize = this.config.roomGenerateSizeRange;
            
            for(var i = 0; i < roomCount; i++)
            {
                var position = GetRandomPointInCircle(this.config.dungeonRadius);
                var rect = new Rect(position.x, position.y, roomSize.GetRandom(), roomSize.GetRandom());
                var roomNode = new RoomNode(rect);

                this.rooms.Add(roomNode);
            }
        }

        private IEnumerator SeparateRoom(GameObject simulationCube)
        {
            var rigidbodies = new List<Rigidbody2D>();
            var shouldBreak = true;

            for(var i = 0; i < this.rooms.Count ; i++)
            {
                var room = this.rooms[i];
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
                var room = this.rooms[i];

                var x = position.x - room.rect.width / 2; 
                var y = position.y - room.rect.height / 2;

                //this.rooms[i] = new Rect(RoundM(x , this.mTileSize) , RoundM(y , this.mTileSize) , room.width, room.height);
                this.rooms[i].rect = new Rect(x , y , room.rect.width, room.rect.height);

                this.mWidthMean += room.rect.width;
                this.mHeightMean += room.rect.height;

                GameObject.Destroy(body.gameObject);
            }

            Debug.Log("Everybody is sleeping now");
        }

        private void DetermineMainRooms()
        {
            var roomCount = this.rooms.Count - 1;
            var mainRoomMinWidth = (this.mWidthMean / roomCount) * 1.25f;
            var mainRoomMinHeight = (this.mHeightMean / roomCount) * 1.25f;

            for (var i = 0; i < this.rooms.Count; i++)
            {
                var room = this.rooms[i];
                if(room.rect.width > mainRoomMinWidth && room.rect.height > mainRoomMinHeight)
                {
                    room.isMain = true;
                }
            }
        }

        private void TriangulateRoom()
        {
            throw new System.NotImplementedException();
        }

        private void GenerateGraph()
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

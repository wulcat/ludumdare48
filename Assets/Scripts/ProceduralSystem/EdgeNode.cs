using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.ProceduralSystem
{
    [Serializable]
    public class EdgeNode
    {
        //public int id = 0;
        //public Rect rect;
        //public List<RoomNode> children;
        public FloorNode a;
        public FloorNode b;

        private float length = -1;
        public float Length
        {
            get
            {
                if (length < 0)
                {
                    var centerA = a.rect.center;
                    var centerB = b.rect.center;

                    float dx = centerA.x - centerB.x;
                    float dy = centerA.y - centerB.y;
                    length = Mathf.Sqrt(dx * dx + dy * dy);
                }
                return length;
            }
        }

        //public EdgeNode(int id , Rect rect)
        //{
        //    this.id = id;
        //    this.rect = rect;
        //    this.children = new List<EdgeNode>();
        //}

        //public void AddChildren(EdgeNode child)
        //{
        //    this.children.Add(child);
        //}

        public EdgeNode(FloorNode a, FloorNode b)
        {
            if (a == b)
            {
                throw new DuplicateFloorNodeException(a, b);
            }
            else if (a < b)
            {
                this.a = a;
                this.b = b;
            }
            else
            {
                this.a = b;
                this.b = a;
            }
        }

        public static int LengthComparison(EdgeNode a, EdgeNode b)
        {
            float aLength = a.Length;
            float bLength = b.Length;

            if(Mathf.Approximately(aLength , bLength))
            {
                return 0;
            }
            else if(aLength > bLength)
            {
                return 1;
            }
            else
            {
                return -1;
            }

        }

        public Vector2 FindRectanularIntersection()
        {
            var posA = this.a.rect.center;
            var posB = this.b.rect.center;
  
            return new Vector2(posA.x, posB.y);
        }


        public static Rect RectBetweemTwoRects(Rect a, Rect b, float defaultSize)
        {
            var x = 0f;
            var y = 0f;

            var aCenter = a.center;
            var bCenter = b.center;

            var distanceX = Mathf.Abs(Mathf.Abs(a.center.x) - Mathf.Abs(b.center.x));
            var distanceY = Mathf.Abs(Mathf.Abs(a.center.y) - Mathf.Abs(b.center.y));

            //var width = distanceX - ((a.width + b.width) / 2);
            //var height = distanceY - ((a.height + b.height) / 2);

            var width = defaultSize;
            var height = defaultSize;

            if (aCenter.x < bCenter.x)
            {
                var minX = a.xMax;
                var maxX = b.xMin;

                if (distanceY < 0.03f)
                {
                    width = maxX - minX;
                }
                
                x = minX + (maxX - minX) / 2;
            }
            else
            {
                var minX = b.xMax;
                var maxX = a.xMin;

                if (distanceY < 0.03f)
                {
                    width = maxX - minX;
                }

                x = minX + (maxX - minX) / 2;
            }


            if (aCenter.y < bCenter.y)
            {
                var minY = a.yMax;
                var maxY = b.yMin;

                if (distanceX < 0.03f)
                {
                    height = maxY - minY;
                }

                y = minY + (maxY - minY) / 2;
            }
            else
            {
                var minY = b.yMax;
                var maxY = a.yMin;

                if (distanceX < 0.03f)
                {
                    height = maxY - minY;
                }

                y = minY + (maxY - minY) / 2;
            }

            if(distanceX < 0.03f)
            {
                x = a.center.x;
            }
            if(distanceY < 0.03f)
            {
                y = a.center.y;
            }

            return new Rect(x - width / 2, y - height / 2 , width, height);
        }
    }
}

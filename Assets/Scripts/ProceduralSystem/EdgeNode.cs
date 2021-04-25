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
    }
}

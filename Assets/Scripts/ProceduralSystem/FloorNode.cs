using System;
using UnityEngine;

namespace Assets.Scripts.ProceduralSystem
{
    /// <summary>
    /// Used to determine main rooms (only usefull interally for generation)
    /// </summary>
    [Serializable]
    public class FloorNode
    {
        public int id;
        public Rect rect;
        public bool isMain = false;

        public FloorNode(int id , Rect rect)
        {
            this.id = id;
            this.rect = rect;
        }

        public static bool operator <(FloorNode a, FloorNode b)
        {
            var lhs = a.rect.center;
            var rhs = b.rect.center;

            return (lhs.x < rhs.x) || ((lhs.x == rhs.x) && (lhs.y < rhs.y));
        }

        public static bool operator >(FloorNode a, FloorNode b)
        {
            var lhs = a.rect.center;
            var rhs = b.rect.center;

            return (lhs.x > rhs.x) || ((lhs.x == rhs.x) && (lhs.y > rhs.y));
        }
    }
}

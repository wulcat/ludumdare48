using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ProceduralSystem
{
    public class RoomNode
    {
        public int id = 0;
        public Rect rect;
        public List<RoomNode> children;

        public RoomNode(int id , Rect rect)
        {
            this.id = id;
            this.rect = rect;
            this.children = new List<RoomNode>();
        }

        public void AddChildren(RoomNode child)
        {
            this.children.Add(child);
        }
    }
}

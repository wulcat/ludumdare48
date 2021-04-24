using System;
using UnityEngine;

namespace Assets.Scripts.ProceduralSystem
{
    [Serializable]
    public class RoomNode
    {
        public Rect rect;
        public bool isMain = false;

        public RoomNode(Rect rect)
        {
            this.rect = rect;
        }
    }
}

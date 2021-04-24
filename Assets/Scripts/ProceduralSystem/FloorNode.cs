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
        public Rect rect;
        public bool isMain = false;

        public FloorNode(Rect rect)
        {
            this.rect = rect;
        }
    }
}

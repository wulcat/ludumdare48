using UnityEngine;

namespace Assets.Scripts.ProceduralSystem
{
    /// <summary>
    /// Defines entry and exit points. The locations for gateway can be same if we want entry and exit at same door (for ex. for sub dungeon with chests)
    /// </summary>
    public class Gateway : MonoBehaviour
    {
        public IDungeon subDungeon;
        /// <summary>
        /// Position for player spawn
        /// </summary>
        public Vector3 position;
        public bool isExit;
    }
}

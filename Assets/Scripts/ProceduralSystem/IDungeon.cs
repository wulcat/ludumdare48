using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.ProceduralSystem {
    public interface IDungeon
    {
        /// <summary>
        /// mapped with list from dungeons
        /// </summary>
        int id { get; set; }
        IDungeon parentDungeon { get; set; }
    }
}
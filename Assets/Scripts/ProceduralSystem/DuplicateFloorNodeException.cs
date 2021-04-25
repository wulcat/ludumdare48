using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ProceduralSystem
{
    class DuplicateFloorNodeException : ApplicationException
    {
        public FloorNode a;
        public FloorNode b;

        public DuplicateFloorNodeException(FloorNode a, FloorNode b) : base()
        {
            this.a = a;
            this.b = b;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Jam5Entry
{
    public class MemoryOrbPressurePad : PressurePad
    {
        public MemoryOrbController controller;

        protected override bool CheckForDetector(GameObject hitObj)
        {
            return base.CheckForDetector(hitObj) || hitObj.GetComponent<MemoryOrbGhostPlayer>() != null;
        }
    }
}

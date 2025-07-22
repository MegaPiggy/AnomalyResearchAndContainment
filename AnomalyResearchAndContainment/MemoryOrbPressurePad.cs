using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public class MemoryOrbPressurePad : PressurePad
    {
        protected override bool CheckForDetector(GameObject hitObj)
        {
            return base.CheckForDetector(hitObj) || hitObj.GetComponent<MemoryOrbGhostPlayer>() != null;
        }

        protected override void OnStep(GameObject hitObj)
        {
            Locator.GetShipLogManager().RevealFact("ARC_MEMORY_ORB_X2", true, true);
        }
    }
}

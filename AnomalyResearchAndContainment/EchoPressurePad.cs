using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace AnomalyResearchAndContainment
{
    public class EchoPressurePad : PressurePad
    {
        [SerializeField] private int _padIndex;

        protected override void OnStep(GameObject hitObj)
        {
            EchoCellController.Instance.RegisterPadStep(_padIndex);
        }
    }
}

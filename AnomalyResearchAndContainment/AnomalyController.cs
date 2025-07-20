using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public abstract class AnomalyController : MonoBehaviour
    {
        [SerializeField] protected AnomalyDoorController _doorController;

        private bool IsDoorClosed => (_doorController == null || !_doorController.IsOpen);
        private bool IsPowered => DialogueConditionManager.SharedInstance.GetConditionState("ActivateAnomalyStation");
        private bool CanActivate => IsPowered && IsDoorClosed;

        private bool _initialized = false;
        private bool _active = false;
        public bool IsActive => _active;

        protected virtual void Update()
        {
            SetActivation(CanActivate);
        }

        public void SetActivation(bool state)
        {
            if (state)
            {
                if (_initialized && _active) return;
                _initialized = true;
                _active = true;
                ActivatePuzzle();
            }
            else
            {
                if (_initialized && !_active) return;
                _initialized = true;
                _active = false;
                DeactivatePuzzle();
            }
        }

        /// <summary>
        /// Start puzzle logic, enable inputs, etc.
        /// </summary>
        public abstract void ActivatePuzzle();

        /// <summary>
        /// Stop puzzle logic, disable inputs, etc.
        /// </summary>
        public abstract void DeactivatePuzzle();

        public void OpenDoor()
        {
            if (_doorController == null) return;
            _doorController.Open();
        }

        public void CloseDoor()
        {
            if (_doorController == null) return;
            _doorController.Close();
        }
    }
}

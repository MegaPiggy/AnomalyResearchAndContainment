using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Jam5Entry
{
    public abstract class PressurePad : MonoBehaviour
    {
        [SerializeField] private OWTriggerVolume _triggerVolume;
        [SerializeField] private AnomalyController _controller;
        private bool _wasTriggered = false;

        private void Awake()
        {
            _triggerVolume.OnEntry += OnEntry;
        }

        private void OnDestroy()
        {
            _triggerVolume.OnEntry -= OnEntry;
        }

        protected virtual bool CheckForDetector(GameObject hitObj)
        {
            return hitObj.CompareTag("PlayerDetector");
        }

        private void OnEntry(GameObject hitObj)
        {
            if ((_controller != null && !_controller.IsActive) || _wasTriggered || !CheckForDetector(hitObj)) return;

            _wasTriggered = true;
            OnStep(hitObj);
            Invoke(nameof(ResetTrigger), 0.5f); // prevent multiple triggers
        }

        protected virtual void OnStep(GameObject hitObj)
        {

        }

        public bool WasTriggered()
        {
            return _wasTriggered;
        }

        private void ResetTrigger()
        {
            _wasTriggered = false;
        }
    }
}

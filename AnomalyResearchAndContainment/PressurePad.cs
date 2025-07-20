using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public abstract class PressurePad : MonoBehaviour
    {
        [SerializeField] private OWTriggerVolume _triggerVolume;
        [SerializeField] private AnomalyController _controller;
        private bool _wasTriggered = false;

        // Track all valid colliders currently on the pad
        private readonly HashSet<GameObject> _currentObjects = new HashSet<GameObject>();

        public bool IsTriggered => _currentObjects.Count > 0;
        public bool WasTriggered => _wasTriggered;

        private void Start()
        {
            _triggerVolume.OnEntry += OnEntry;
            _triggerVolume.OnExit += OnExit;
        }

        private void OnDestroy()
        {
            _triggerVolume.OnEntry -= OnEntry;
            _triggerVolume.OnExit -= OnExit;
        }

        protected virtual bool CheckForDetector(GameObject hitObj)
        {
            return hitObj.CompareTag("PlayerDetector");
        }

        private void OnEntry(GameObject hitObj)
        {
            AnomalyResearchAndContainment.Instance.ModHelper.Console.WriteLine("Entry: " + hitObj.name + " | " + hitObj.tag);
            if (_controller != null && !_controller.IsActive) return;
            if (!CheckForDetector(hitObj)) return;

            _currentObjects.Add(hitObj);

            if (_wasTriggered) return;

            _wasTriggered = true;
            OnStep(hitObj);
            Invoke(nameof(ResetWasTriggered), 0.5f); // prevent multiple rapid triggers
        }

        private void OnExit(GameObject hitObj)
        {
            AnomalyResearchAndContainment.Instance.ModHelper.Console.WriteLine("Exit: " + hitObj.name + " | " + hitObj.tag);
            _currentObjects.Remove(hitObj);

            if (_currentObjects.Count == 0)
            {
                ResetWasTriggered();
            }
        }

        protected virtual void OnStep(GameObject hitObj)
        {
        }

        private void ResetWasTriggered()
        {
            _wasTriggered = false;
        }
    }
}

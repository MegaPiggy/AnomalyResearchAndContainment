using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    [RequireComponent(typeof(OWTriggerVolume), typeof(OWAudioSource))]
    public abstract class PressurePad : MonoBehaviour
    {
        [SerializeField] private OWTriggerVolume _triggerVolume;
        [SerializeField] private AnomalyController _controller;
        [SerializeField] private OWAudioSource _audioSource;
        [SerializeField] private MeshRenderer _glowRenderer;

        private Color _inactiveColor = Color.black;
        private Color _activeColor = Color.cyan;
        private float _glowIntensity = 0.5f;
        private AudioType _tone = AudioType.NomaiOrbSlotActivated;

        private float _glowFraction;
        private bool _wasTriggered = false;

        // Track all valid colliders currently on the pad
        private readonly HashSet<GameObject> _currentObjects = new HashSet<GameObject>();

        public bool IsTriggered => _currentObjects.Count > 0;
        public bool WasTriggered => _wasTriggered;

        private void Start()
        {
            _glowRenderer = GetComponentInChildren<MeshRenderer>(true);
            _triggerVolume.OnEntry += OnEntry;
            _triggerVolume.OnExit += OnExit;
        }

        private void OnDestroy()
        {
            _triggerVolume.OnEntry -= OnEntry;
            _triggerVolume.OnExit -= OnExit;
        }

        private void Update()
        {
            float target = (IsTriggered ? 1f : 0f);
            _glowFraction = Mathf.MoveTowards(_glowFraction, target, Time.deltaTime * 3f);
            if (_glowRenderer != null)
            {
                var targetColor = Color.Lerp(_inactiveColor, _activeColor, _glowFraction);
                _glowRenderer.material.SetColor("_EmissionColor", targetColor);
            }
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
            if (_audioSource != null) _audioSource.PlayOneShot(_tone);
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

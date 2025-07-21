using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace AnomalyResearchAndContainment
{
    public class PrismBloomController : AnomalyController
    {
        [SerializeField] private Light _beamEmitter;
        [SerializeField] private float _beamLength = 20f;
        [SerializeField] private LayerMask _reflectionLayer = OWLayerMask.blockableInteractMask;
        [SerializeField] private int _maxReflections = 5;
        [SerializeField] private BeamSensor[] _sensors = new BeamSensor[0];
        [SerializeField] private ReflectiveSurface[] _reflectiveSurfaces = new ReflectiveSurface[0];
        [SerializeField] private KeyDropper _keyDropper;
        [SerializeField] private Indicator _indicator;

        private bool _completed;

        public void OnValidate()
        {
            _reflectionLayer = ((1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Primitive")) | (1 << LayerMask.NameToLayer("IgnoreSun")) | (1 << LayerMask.NameToLayer("PhysicalDetector")) | (1 << LayerMask.NameToLayer("ShipInterior")) | (1 << LayerMask.NameToLayer("IgnoreOrbRaycast")) | (1 << LayerMask.NameToLayer("Interactible"))) & ~(1 << LayerMask.NameToLayer("IgnoreOrbRaycast"));
            _sensors = GetComponentsInChildren<BeamSensor>();
            _reflectiveSurfaces = GetComponentsInChildren<ReflectiveSurface>();
        }

        protected override void Update()
        {
            base.Update();
            if (!IsActive || _completed) return;

            ResetSensors();

            if (_beamEmitter == null) return;

            Vector3 origin = _beamEmitter.transform.position;
            Vector3 direction = _beamEmitter.transform.forward;

            // Simulate beam
            for (int i = 0; i < _maxReflections; i++)
            {
                Debug.DrawRay(origin, direction * _beamLength, Color.cyan, 0.1f);
                if (Physics.Raycast(origin, direction, out RaycastHit hit, _beamLength, _reflectionLayer))
                {
                    // Check for sensor
                    var sensor = hit.collider.GetComponent<BeamSensor>();
                    if (sensor != null)
                    {
                        sensor.Trigger();
                    }

                    // Check for reflective object
                    var reflect = hit.collider.GetComponent<ReflectiveSurface>();
                    if (reflect != null)
                    {
                        origin = hit.point;
                        direction = Vector3.Reflect(direction, hit.normal);
                        continue;
                    }

                    // Non-reflective surface hit
                    break;
                }
                else
                {
                    break;
                }
            }

            if (AllSensorsActivated())
            {
                _completed = true;
                _indicator.PlaySuccessFeedback();
                if (_keyDropper != null) _keyDropper.DropKey();
                SetActivation(false);
                OpenDoor();
            }
        }

        private void ResetSensors()
        {
            foreach (var sensor in _sensors)
            {
                sensor.ResetTrigger();
            }
        }

        private bool AllSensorsActivated()
        {
            foreach (var sensor in _sensors)
            {
                if (!sensor.IsTriggered()) return false;
            }
            return true;
        }

        public override void ActivatePuzzle()
        {
        }

        public override void DeactivatePuzzle()
        {
        }
    }

    public class BeamSensor : MonoBehaviour
    {
        private bool _triggered;

        public void Trigger()
        {
            _triggered = true;
            // Visual feedback here, e.g., glow
        }

        public void ResetTrigger()
        {
            _triggered = false;
        }

        public bool IsTriggered() => _triggered;
    }

    /// <summary>
    /// Marker component
    /// </summary>
    public class ReflectiveSurface : MonoBehaviour
    {
    }
}

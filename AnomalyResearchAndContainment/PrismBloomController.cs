using UnityEngine;
using System.Collections.Generic;

namespace AnomalyResearchAndContainment
{
    public class PrismBloomController : AnomalyController
    {
        [SerializeField] private Light _beamEmitter;
        [SerializeField] private float _beamLength = 20f;
        [SerializeField] private LayerMask _reflectionLayer;
        [SerializeField] private int _maxReflections = 5;
        [SerializeField] private List<BeamSensor> _sensors = new List<BeamSensor>();
        [SerializeField] private KeyDropper _keyDropper;
        [SerializeField] private Indicator _indicator;

        private bool _completed;

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

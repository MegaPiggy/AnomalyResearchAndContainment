using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

namespace AnomalyResearchAndContainment
{
    public class PrismBloomController : AnomalyController
    {
        [SerializeField] private BeamEmitter _beamEmitter;

        private float _beamLength = 20f;
        private int _maxReflections = 5;
        private LayerMask _reflectionLayer;

        private BeamSensor[] _sensors = new BeamSensor[0];
        private ReflectiveSurface[] _reflectiveSurfaces = new ReflectiveSurface[0];

        public void Start()
        {
            _reflectionLayer = OWLayerMask.physicalMask;
            _beamEmitter = GetComponentInChildren<BeamEmitter>();
            _sensors = GetComponentsInChildren<BeamSensor>();
            _reflectiveSurfaces = GetComponentsInChildren<ReflectiveSurface>();
        }

        protected override void Update()
        {
            base.Update();
            if (!IsActive || Completed) return;

            CastBeam();

            if (AllSensorsActivated())
            {
                CompletePuzzle();
            }
        }

        public void CastBeam()
        {
            List<Vector3> beamPoints = new List<Vector3>();
            Vector3 origin = _beamEmitter.transform.position;
            Vector3 direction = _beamEmitter.transform.forward;
            beamPoints.Add(origin);

            ResetSensors();

            // Simulate beam
            for (int i = 0; i < _maxReflections; i++)
            {
                Debug.DrawRay(origin, direction * _beamLength, Color.cyan, 0.1f);
                if (Physics.Raycast(origin, direction, out RaycastHit hit, _beamLength, _reflectionLayer))
                {
                    beamPoints.Add(hit.point);

                    // Check for sensor
                    var sensor = hit.collider.GetComponent<BeamSensor>();
                    if (sensor != null) sensor.Trigger();

                    // Check for reflective object
                    var reflect = hit.collider.GetComponent<ReflectiveSurface>();
                    if (reflect != null)
                    {
                        // Root point
                        beamPoints.Add(reflect.GetOutputRoot());

                        // Update origin/direction for next segment
                        origin = reflect.GetOutputRoot(); //hit.point;
                        direction = reflect.GetOutputDirection(); //Vector3.Reflect(direction, hit.normal);
                        continue;
                    }

                    // Non-reflective surface hit
                    break;
                }
                else
                {
                    beamPoints.Add(origin + direction * _beamLength);
                    break;
                }
            }

            _beamEmitter.SetPositions(beamPoints);
        }

        public void ResetSensors()
        {
            foreach (var sensor in _sensors)
            {
                sensor.ResetTrigger();
            }
        }

        public bool AllSensorsActivated()
        {
            foreach (var sensor in _sensors)
            {
                if (!sensor.IsTriggered()) return false;
            }
            return true;
        }

        public override void CompletePuzzle()
        {
            base.CompletePuzzle();

            Locator.GetShipLogManager().RevealFact("ARC_PRISM_BLOOM_X2", true, true);
            ResetSensors();
        }

        public override void ResetPuzzle()
        {
            base.ResetPuzzle();

            ResetSensors();
        }

        public override void ActivatePuzzle()
        {
        }

        public override void DeactivatePuzzle()
        {
            ResetSensors();
        }
    }

    [RequireComponent(typeof(Light), typeof(LineRenderer))]
    public class BeamEmitter : MonoBehaviour
    {
        [SerializeField] private Light _light;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private GameObject _beamPointPrefab;

        private readonly List<GameObject> _beamPoints = new();
        private float _pointSpacing = 0.5f;
        private int _maxPoints = 100;

        public void OnValidate()
        {
            _light = GetComponent<Light>();
            _lineRenderer = GetComponent<LineRenderer>();
            SetPositionDefault();
        }

        public void Awake()
        {
            for (int i = 0; i < _maxPoints; i++)
            {
                GameObject point = Instantiate(_beamPointPrefab, transform);
                point.SetActive(false);
                _beamPoints.Add(point);
            }
            _light = GetComponent<Light>();
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.enabled = false;
            /*
            _lineRenderer.startColor = Color.white;
            _lineRenderer.endColor = new Color(_lineRenderer.startColor.r, _lineRenderer.startColor.g, _lineRenderer.startColor.b, 0f);
            _lineRenderer.startWidth = 0.05f;
            _lineRenderer.endWidth = 0.05f;
            */
            SetPositionDefault();
        }

        public void Start()
        {
        }

        private void SetPositionDefault()
        {
            SetPositions(new List<Vector3>
            {
                Vector3.zero,
                Vector3.forward
            });
        }

        public void SetPositions(List<Vector3> beamPoints)
        {
            DrawBeamPoints(beamPoints);
            /*
            if (_lineRenderer == null) return;
            _lineRenderer.positionCount = beamPoints.Count;
            _lineRenderer.SetPositions(beamPoints.ToArray());
            */
        }

        public void ClearBeamPoints()
        {
            foreach (var point in _beamPoints)
            {
                point.SetActive(false);
            }
        }

        public void DrawBeamPoints(List<Vector3> path)
        {
            ClearBeamPoints();
            int poolIndex = 0;

            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 start = path[i];
                Vector3 end = path[i + 1];
                float distance = Vector3.Distance(start, end);
                int steps = Mathf.CeilToInt(distance / _pointSpacing);

                for (int j = 0; j <= steps && poolIndex < _beamPoints.Count; j++)
                {
                    Vector3 pos = Vector3.Lerp(start, end, j / (float)steps);
                    _beamPoints[poolIndex].transform.position = pos;
                    _beamPoints[poolIndex].SetActive(true);
                    poolIndex++;
                }
            }
        }
    }

    public class BeamSensor : MonoBehaviour
    {
        [SerializeField] private Renderer _sensorRenderer;
        [SerializeField] private OWAudioSource _audioSource;

        private AudioType _activationSFX = AudioType.NomaiOrbSlotActivated;
        private Color _inactiveColor = Color.black;
        private Color _activeColor = Color.cyan;
        private float _glowIntensity = 1;

        private bool _triggered;
        private bool _lastTriggered;

        private void Start()
        {
            UpdateVisual();
        }
        
        private void LateUpdate()
        {
            _lastTriggered = _triggered;
        }

        public void Trigger()
        {
            if (_triggered) return;

            _triggered = true;
            UpdateVisual();

            if (!_lastTriggered)
            {
                if (_audioSource != null) _audioSource.PlayOneShot(_activationSFX);
                Locator.GetShipLogManager().RevealFact("ARC_PRISM_BLOOM_X1", true, true);
            }
        }

        public void ResetTrigger()
        {
            if (!_triggered) return;

            _triggered = false;
            UpdateVisual();
        }

        public bool IsTriggered() => _triggered;

        private void UpdateVisual()
        {
            if (_sensorRenderer == null || _sensorRenderer.material == null) return;

            Color targetColor = _triggered ? _activeColor : _inactiveColor;
            _sensorRenderer.material.SetColor("_EmissionColor", targetColor * _glowIntensity);
            _sensorRenderer.material.color = targetColor;
        }
    }

    /// <summary>
    /// Marker component
    /// </summary>
    public class ReflectiveSurface : MonoBehaviour
    {
        [SerializeField] private Transform _root;

        public Vector3 GetOutputRoot()
        {
            return _root.position; // Or customize if needed
        }

        public Vector3 GetOutputDirection()
        {
            return _root.forward; // Or customize if needed
        }
    }
}

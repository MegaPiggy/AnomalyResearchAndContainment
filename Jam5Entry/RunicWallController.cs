using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jam5Entry
{
    public class RunicWallController : AnomalyController
    {
        [SerializeField] private List<RunePanel> _panels = new List<RunePanel>();
        [SerializeField] private KeyDropper _keyDropper;
        [SerializeField] private float _resetDelay = 2f;
        [SerializeField] private OWAudioSource _audioSource;
        [SerializeField] private float _swapCooldown = 1.5f;

        private List<TransformData> _initialTransforms = new List<TransformData>();
        private bool _canSwap = true;

        private AudioType _successAudio = AudioType.NonDiaUIAffirmativeSFX;
        private AudioType _failAudio = AudioType.NonDiaUINegativeSFX;

        private PhotoTarget[] _targets;
        private List<int> _correctSequence = [5, 2, 4, 0];
        private List<int> _playerSequence = new List<int>();
        private bool _completed = false;

        private void Start()
        {
            foreach (var panel in _panels)
            {
                // Record initial positions and rotations
                _initialTransforms.Add(new TransformData
                {
                    position = panel.transform.localPosition,
                    rotation = panel.transform.localRotation
                });
                // Link probe targets to photo detection
                panel.photoTarget.OnPhotographed += OnTargetPhotographed;
            }
        }

        protected override void Update()
        {
            base.Update();
            if (!_canSwap) return;

            bool anyVisible = _panels.Any(p => p.isVisible);
            if (!anyVisible)
            {
                _canSwap = false;
                StartCoroutine(SwapPanelsRoutine());
            }
        }

        private void OnTargetPhotographed(PhotoTarget target)
        {
            if (!IsActive || _completed) return;

            Jam5Entry.Instance.ModHelper.Console.WriteLine("Probe: " + target.GetComponent<RunePanel>().id);
            foreach (var panel in _panels)
            {
                if (panel.MatchesTarget(target) && panel.isVisible)
                {
                    _playerSequence.Add(panel.id);
                    panel.SetVisibility(false);

                    if (_playerSequence.Count >= _correctSequence.Count)
                    {
                        if (IsCorrectSequence())
                        {
                            CompletePuzzle();
                        }
                        else
                        {
                            Invoke(nameof(ResetPuzzle), _resetDelay);
                        }
                    }

                    break;
                }
            }
        }

        private bool IsCorrectSequence()
        {
            if (_playerSequence.Count != _correctSequence.Count) return false;

            for (int i = 0; i < _correctSequence.Count; i++)
            {
                if (_playerSequence[i] != _correctSequence[i])
                    return false;
            }
            return true;
        }

        private void CompletePuzzle()
        {
            _completed = true;
            if (_audioSource != null) _audioSource.PlayOneShot(_successAudio);
            if (_keyDropper != null) _keyDropper.DropKey();
            SetActivation(false);
            OpenDoor();
        }

        private void ResetPuzzle()
        {
            _playerSequence.Clear();

            if (_audioSource != null) _audioSource.PlayOneShot(_failAudio);
            foreach (var panel in _panels)
            {
                panel.SetVisibility(false);
            }
        }

        public override void ActivatePuzzle()
        {
        }

        public override void DeactivatePuzzle()
        {
        }

        private IEnumerator SwapPanelsRoutine()
        {
            _canSwap = false;

            // Shuffle the transform data
            List<TransformData> shuffled = _initialTransforms.OrderBy(x => Random.value).ToList();

            for (int i = 0; i < _panels.Count; i++)
            {
                _panels[i].transform.localPosition = shuffled[i].position;
                _panels[i].transform.localRotation = shuffled[i].rotation;
            }

            yield return new WaitForSeconds(_swapCooldown);
            _canSwap = true;
        }

        private struct TransformData
        {
            public Vector3 position;
            public Quaternion rotation;
        }
    }

    public class RunePanel : MonoBehaviour
    {
        [SerializeField] public int id;
        [SerializeField] public GameObject wall;
        [SerializeField] public Renderer glowRenderer;
        [SerializeField] public PhotoTarget photoTarget;

        public static float revealAngle = 20f; // degrees
        public bool isVisible { get; private set; }

        private void Update()
        {
            Vector3 origin = transform.position + (-transform.forward * 0.1f); // slightly in front
            float threshold = Mathf.Cos(revealAngle * Mathf.Deg2Rad); // 60 degrees = 0.5

            bool isVisibleToPlayer = IsViewerLooking(origin, -transform.forward, Locator.GetPlayerTransform(), threshold, Locator.GetPlayerCamera().mainCamera);
            bool isVisibleToProbe = false;

            var probe = Locator.GetProbe();
            if (probe != null && probe.IsLaunched())
            {
                var probeCam = probe.IsAnchored() ? probe.GetRotatingCamera() : probe.GetForwardCamera();
                if (probeCam != null)
                {
                    isVisibleToProbe = IsViewerLooking(origin, -transform.forward, probeCam.transform, threshold, probeCam.GetOWCamera().mainCamera);
                }
            }

            SetVisibility(isVisibleToPlayer || isVisibleToProbe);
        }

        private bool IsViewerLooking(Vector3 origin, Vector3 normal, Transform viewer, float dotThreshold, Camera cam)
        {
            if (viewer == null || cam == null) return false;

            Vector3 toViewer = (viewer.position - origin).normalized;
            float dot = Vector3.Dot(normal, toViewer); // closer to 1 = head-on

            if (dot < dotThreshold) return false;

            // Viewport check
            Vector3 viewportPos = cam.WorldToViewportPoint(origin);
            return viewportPos.z > 0f && viewportPos.x is > 0f and < 1f && viewportPos.y is > 0f and < 1f;
        }


        public bool MatchesTarget(PhotoTarget target)
        {
            return target != null && photoTarget == target;
        }

        public void SetVisibility(bool visible)
        {
            if (isVisible != visible)
            {
                isVisible = visible;
                wall.SetActive(visible);
            }
        }
    }
}

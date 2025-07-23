using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public class RunicWallController : AnomalyController
    {
        [SerializeField] private List<RunePanel> _panels = new List<RunePanel>();
        [SerializeField] private float _resetDelay = 0.5f;
        [SerializeField] private float _swapCooldown = 1.5f;

        private List<TransformData> _initialTransforms = new List<TransformData>();
        private bool _canSwap = true;

        private List<int> _correctSequence = [5, 2, 4, 0];
        private List<int> _playerSequence = new List<int>();

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
            }

            GlobalMessenger<ProbeCamera>.AddListener("ProbeSnapshot", OnProbeSnapshot);
        }

        private void OnDestroy()
        {
            GlobalMessenger<ProbeCamera>.RemoveListener("ProbeSnapshot", OnProbeSnapshot);
        }

        protected override void Update()
        {
            base.Update();
            if (!_canSwap || !IsActive || Completed) return;

            bool anyVisible = _panels.Any(p => p.isVisible);
            if (!anyVisible)
            {
                _canSwap = false;
                Locator.GetShipLogManager().RevealFact("ARC_RUNIC_WALL_X2", true, true);
                StartCoroutine(SwapPanelsRoutine());
            }
        }

        private void OnProbeSnapshot(ProbeCamera probeCamera)
        {
            if (!IsActive || Completed) return;

            var photographed = _panels.Where(panel => panel.photoTarget.IsInProbeSnapshot(probeCamera)).ToList();

            if (photographed.Count == 0) return;

            if (photographed.Count > 1)
            {
                AnomalyResearchAndContainment.Instance.ModHelper.Console.WriteLine("Invalid photo — must include exactly one rune.");
                ResetPuzzle();
                return;
            }

            var rune = photographed.First();
            int runeId = rune.id;

            // Disallow duplicates in the sequence
            if (_playerSequence.Contains(runeId))
            {
                AnomalyResearchAndContainment.Instance.ModHelper.Console.WriteLine($"Duplicate rune {runeId} ignored.");
                return;
            }

            _playerSequence.Add(runeId);
            AnomalyResearchAndContainment.Instance.ModHelper.Console.WriteLine($"Photographed rune: {runeId}");

            if (_playerSequence.Count >= _correctSequence.Count)
            {
                if (IsCorrectSequence())
                {
                    CompletePuzzle();
                }
                else
                {
                    ResetPuzzle();
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

        public override void CompletePuzzle()
        {
            base.CompletePuzzle();

            Locator.GetShipLogManager().RevealFact("ARC_RUNIC_WALL_X4", true, true);
        }

        public void ResetAfterDelay()
        {
            Invoke(nameof(ResetPuzzle), _resetDelay);
        }

        public override void ResetPuzzle()
        {
            base.ResetPuzzle();

            _playerSequence.Clear();
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

            if (isVisibleToPlayer) Locator.GetShipLogManager().RevealFact("ARC_RUNIC_WALL_X1", true, true);
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

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

        private AudioType _successAudio = AudioType.NonDiaUIAffirmativeSFX;
        private AudioType _failAudio = AudioType.NonDiaUINegativeSFX;

        private ProbePhotoTarget[] _targets;
        private List<int> _correctSequence = [5, 2, 4, 1];
        private List<int> _playerSequence = new List<int>();
        private bool _completed = false;

        private void Start()
        {
            // Link probe targets to photo detection
            foreach (var panel in _panels)
            {
                panel.photoTarget.OnPhotographedByProbe += OnTargetPhotographed;
            }
        }

        private void OnTargetPhotographed(ProbePhotoTarget target, float score)
        {
            if (!IsActive || _completed) return;

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
    }

    public class RunePanel : MonoBehaviour
    {
        [SerializeField] public int id;
        [SerializeField] public GameObject wall;
        [SerializeField] public Renderer glowRenderer;
        [SerializeField] public ProbePhotoTarget photoTarget;

        [SerializeField] private float revealAngle = 30f; // degrees
        public bool isVisible { get; private set; }

        private void Update()
        {
            Vector3 toViewer = (Locator.GetPlayerTransform().position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, toViewer);

            SetVisibility(angle <= revealAngle);
        }

        public bool MatchesTarget(ProbePhotoTarget target)
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

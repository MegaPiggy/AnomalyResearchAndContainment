using System.Collections.Generic;
using UnityEngine;

namespace Jam5Entry
{
    public class RunicWallController : MonoBehaviour
    {
        [SerializeField] private List<RunePanel> _panels = new List<RunePanel>();
        [SerializeField] private List<int> _correctSequence = new List<int>();
        [SerializeField] private KeyDropper _keyDropper;
        [SerializeField] private float _cycleInterval = 5f;
        [SerializeField] private float _resetDelay = 2f;
        [SerializeField] private OWAudioSource _audioSource;
        private AudioType _successAudio = AudioType.NonDiaUIAffirmativeSFX;
        private AudioType _failAudio = AudioType.NonDiaUINegativeSFX;

        private List<int> _playerSequence = new List<int>();
        private float _lastInputTime;
        private int _currentStep = 0;
        private bool _completed = false;

        private void Start()
        {
            CycleRuneVisibility();
            InvokeRepeating(nameof(CycleRuneVisibility), _cycleInterval, _cycleInterval);
        }

        private void CycleRuneVisibility()
        {
            foreach (var panel in _panels)
            {
                panel.CycleRuneVisibility();
            }
        }

        public void OnPhotoTaken(List<int> panelIdsInPhoto)
        {
            if (_completed || panelIdsInPhoto == null || panelIdsInPhoto.Count == 0)
                return;

            foreach (var panelId in panelIdsInPhoto)
            {
                RunePanel panel = _panels.Find(p => p.id == panelId);
                if (panel != null && panel.isVisible)
                {
                    _playerSequence.Add(panelId);
                    panel.SetGlow(true);
                }
            }

            _lastInputTime = Time.time;

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
        }

        private bool IsCorrectSequence()
        {
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
        }

        private void ResetPuzzle()
        {
            _playerSequence.Clear();
            if (_audioSource != null) _audioSource.PlayOneShot(_failAudio);

            foreach (var panel in _panels)
            {
                panel.SetGlow(false);
            }
        }
    }
    public class RunePanel : MonoBehaviour
    {
        [SerializeField] public int id;
        [SerializeField] public Renderer glowRenderer;
        [SerializeField] public Color defaultColor = Color.gray;
        [SerializeField] public Color activeColor = Color.cyan;
        [SerializeField] public bool isVisible = true;

        public void SetGlow(bool on)
        {
            if (glowRenderer != null)
            {
                Color color = on ? activeColor : defaultColor;
                glowRenderer.material.SetColor("_EmissionColor", color);
            }
        }

        public void CycleRuneVisibility()
        {
            bool visible = Random.value > 0.5f;
            isVisible = visible;
            gameObject.SetActive(visible);
            SetGlow(false);
        }
    }
}

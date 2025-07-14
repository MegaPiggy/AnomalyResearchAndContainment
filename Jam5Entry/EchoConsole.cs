using System.Collections.Generic;
using UnityEngine;

namespace Jam5Entry
{
    public class EchoConsole : MonoBehaviour
    {
        [SerializeField] private List<int> _correctSequence = new List<int>();
        [SerializeField] private float _timeout = 2f;
        [SerializeField] private KeyDropper _keyDropper;
        [SerializeField] private OWAudioSource _audioSource;
        [SerializeField] private AudioType _successAudio; // AudioType.UI_Confirm
        [SerializeField] private AudioType _failAudio; // AudioType.UI_Error
        [SerializeField] private Renderer[] _indicatorRenderers;
        [SerializeField] private Color _defaultColor = Color.grey;
        [SerializeField] private Color _successColor = Color.green;
        [SerializeField] private Color _failColor = Color.red;
        [SerializeField] private float _flashDuration = 0.5f;

        private List<int> _inputSequence = new List<int>();
        private float _lastInputTime;
        private bool _completed = false;

        public void Start()
        {
            foreach (var renderer in _indicatorRenderers)
            {
                renderer.material.SetColor("_EmissionColor", _defaultColor);
            }
        }

        public void RegisterPadStep(int index)
        {
            if (_completed) return;

            if (Time.time - _lastInputTime > _timeout)
            {
                _inputSequence.Clear();
            }

            _lastInputTime = Time.time;
            _inputSequence.Add(index);

            if (_inputSequence.Count == _correctSequence.Count)
            {
                if (IsCorrectSequence())
                {
                    _completed = true;
                    _keyDropper?.DropKey();
                    PlaySuccessFeedback();
                }
                else
                {
                    _inputSequence.Clear();
                    PlayFailFeedback();
                }
            }
        }

        private bool IsCorrectSequence()
        {
            for (int i = 0; i < _correctSequence.Count; i++)
            {
                if (_correctSequence[i] != _inputSequence[i])
                    return false;
            }
            return true;
        }

        private void PlaySuccessFeedback()
        {
            _audioSource?.PlayOneShot(_successAudio);
            FlashIndicators(_successColor);
        }

        private void PlayFailFeedback()
        {
            _audioSource?.PlayOneShot(_failAudio);
            FlashIndicators(_failColor);
        }

        private void FlashIndicators(Color flashColor)
        {
            foreach (var renderer in _indicatorRenderers)
            {
                StartCoroutine(FlashRendererColor(renderer, flashColor));
            }
        }

        private System.Collections.IEnumerator FlashRendererColor(Renderer rend, Color flashColor)
        {
            if (rend == null) yield break;

            rend.material.SetColor("_EmissionColor", flashColor);
            yield return new WaitForSeconds(_flashDuration);
            rend.material.SetColor("_EmissionColor", _defaultColor);
        }

    }
}

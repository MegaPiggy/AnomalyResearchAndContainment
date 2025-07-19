using System.Collections.Generic;
using UnityEngine;

namespace Jam5Entry
{
    public class EchoConsole : MonoBehaviour
    {
        [SerializeField] public readonly EchoCellController controller;
        [SerializeField] private KeyDropper _keyDropper;
        [SerializeField] private OWAudioSource _audioSource;
        [SerializeField] private Renderer[] _indicatorRenderers;
        private AudioType _successAudio = AudioType.NonDiaUIAffirmativeSFX;
        private AudioType _failAudio = AudioType.NonDiaUINegativeSFX;
        private Color _defaultColor = Color.black;
        private Color _successColor = Color.green;
        private Color _failColor = Color.red;
        private float _flashDuration = 0.5f;
        private float _timeout = 10f;

        private List<int> _inputSequence = new List<int>();
        private float _lastInputTime;
        private bool _completed = false;

        public void Start()
        {
            if (_indicatorRenderers != null)
            {
                foreach (var renderer in _indicatorRenderers)
                {
                    SetFlashRendererColor(renderer, _defaultColor);
                }
            }
        }

        public void RegisterPadStep(int index)
        {
            if (!controller.IsActive || _completed) return;

            if (Time.time - _lastInputTime > _timeout)
            {
                _inputSequence.Clear();
                PlayFailFeedback();
            }

            _lastInputTime = Time.time;
            _inputSequence.Add(index);

            if (_inputSequence.Count >= controller.correctSequence.Count)
            {
                if (IsCorrectSequence())
                {
                    _completed = true;
                    if (_keyDropper != null) _keyDropper.DropKey();
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
            for (int i = 0; i < controller.correctSequence.Count; i++)
            {
                if (controller.correctSequence[i] != _inputSequence[i])
                    return false;
            }
            return true;
        }

        private void PlaySuccessFeedback()
        {
            if (_audioSource != null) _audioSource.PlayOneShot(_successAudio);
            FlashIndicators(_successColor);
        }

        private void PlayFailFeedback()
        {
            if (_audioSource != null) _audioSource.PlayOneShot(_failAudio);
            FlashIndicators(_failColor);
        }

        private void FlashIndicators(Color flashColor)
        {
            if (_indicatorRenderers != null)
            {
                foreach (var renderer in _indicatorRenderers)
                {
                    StartCoroutine(Flash(renderer, flashColor));
                }
            }
        }

        private System.Collections.IEnumerator Flash(Renderer renderer, Color flashColor)
        {
            if (renderer == null) yield break;

            SetFlashRendererColor(renderer, flashColor);
            yield return new WaitForSeconds(_flashDuration);
            SetFlashRendererColor(renderer, _defaultColor);
        }

        public static void SetFlashRendererColor(Renderer renderer, Color flashColor)
        {
            if (renderer == null) return;
            renderer.material.SetColor("_EmissionColor", flashColor);
        }
    }
}

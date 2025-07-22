using UnityEngine;
using NewHorizons.Handlers;
using System.Collections;

namespace AnomalyResearchAndContainment
{
    public class RotatingCrystal : ReflectiveSurface
    {
        [SerializeField] private SingleInteractionVolume _interactionVolume;
        [SerializeField] private Vector3 _rotationAxis = Vector3.up;
        [SerializeField] private float _rotationAmount = 30; // degrees per press
        [SerializeField] private bool _rotateClockwise = true;
        [SerializeField] private AnimationCurve _tweenCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private OWAudioSource _audioSource;

        private float _tweenTime = 0.5f;
        private AudioType _rotateSound = AudioType.ToolItemSharedStoneInsert;
        private Coroutine _rotateRoutine;

        public void Start()
        {
            if (_interactionVolume != null)
            {
                _interactionVolume.OnPressInteract += OnPressInteract;
                _interactionVolume.ChangePrompt((UITextType)TranslationHandler.AddUI("Rotate Crystal", false));
            }
        }

        public void OnDestroy()
        {
            if (_interactionVolume != null)
                _interactionVolume.OnPressInteract -= OnPressInteract;
        }

        private void OnPressInteract()
        {
            RotateCrystal();
            _interactionVolume.ResetInteraction();
        }

        public void RotateCrystal() => RotateCrystal(_rotateClockwise ? 1 : -1);

        public void RotateCrystal(float direction)
        {
            if (_rotateRoutine != null) return; // prevent spam while tweening
            if (_audioSource != null) _audioSource.PlayOneShot(_rotateSound);
            _rotateRoutine = StartCoroutine(TweenRotation(direction));
        }

        private IEnumerator TweenRotation(float direction)
        {
            Quaternion start = transform.localRotation;
            Quaternion end = start * Quaternion.AngleAxis(_rotationAmount * direction, _rotationAxis.normalized);

            float elapsed = 0f;
            while (elapsed < _tweenTime)
            {
                float t = elapsed / _tweenTime;
                float eased = _tweenCurve.Evaluate(t);
                transform.localRotation = Quaternion.Slerp(start, end, eased);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localRotation = end;
            _rotateRoutine = null;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public class GlassLiftController : MonoBehaviour
    {
        [Header("Glass Tube")]
        [SerializeField] private Transform _glass;

        [Header("Positions")]
        [SerializeField] private Transform _glassLowered;
        [SerializeField] private Transform _glassRaised;

        [Header("Tween")]
        [SerializeField] private float _tweenTime = 1f;
        [SerializeField] private AnimationCurve _tweenCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private bool _isRaised = false;
        private Coroutine _tweenRoutine;

        public bool IsRaised => _isRaised;
        public event System.Action OnFullyRaised;
        public event System.Action OnFullyLowered;

        public void Raise() => StartTween(true);
        public void Lower() => StartTween(false);
        public void Toggle() => StartTween(!_isRaised);
        public void Set(bool raised) => StartTween(raised);

        private void StartTween(bool raise)
        {
            if (_isRaised == raise) return;
            if (_tweenRoutine != null)
                StopCoroutine(_tweenRoutine);

            _isRaised = raise;
            _tweenRoutine = StartCoroutine(TweenGlass(
                raise ? _glassRaised : _glassLowered,
                raise));
        }

        private IEnumerator TweenGlass(Transform target, bool raising)
        {
            Vector3 startPos = _glass.localPosition;
            Quaternion startRot = _glass.localRotation;
            Vector3 startScale = _glass.localScale;

            Vector3 endPos = target.localPosition;
            Quaternion endRot = target.localRotation;
            Vector3 endScale = target.localScale;

            float elapsed = 0f;

            while (elapsed < _tweenTime)
            {
                float t = elapsed / _tweenTime;
                float eased = _tweenCurve.Evaluate(t);

                _glass.localPosition = Vector3.Lerp(startPos, endPos, eased);
                _glass.localRotation = Quaternion.Lerp(startRot, endRot, eased);
                _glass.localScale = Vector3.Lerp(startScale, endScale, eased);

                elapsed += Time.deltaTime;
                yield return null;
            }

            _glass.localPosition = endPos;
            _glass.localRotation = endRot;
            _glass.localScale = endScale;

            _tweenRoutine = null;

            if (raising)
                OnFullyRaised?.Invoke();
            else
                OnFullyLowered?.Invoke();
        }
    }
}

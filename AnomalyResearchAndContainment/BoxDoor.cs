using System.Collections;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public class BoxDoor : MonoBehaviour
    {
        [Header("Doors")]
        [SerializeField] private Transform _door;

        [Header("States")]
        [SerializeField] private Transform _doorClosed;
        [SerializeField] private Transform _doorOpen;

        [Header("Tween")]
        [SerializeField] private float _tweenTime = 1f;
        [SerializeField] private AnimationCurve _tweenCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private bool _isOpen = false;
        private Coroutine _tweenRoutine;

        public bool IsOpen => _isOpen;
        public event System.Action OnFullyOpen;
        public event System.Action OnFullyClosed;

        public void Open() => StartTween(true);
        public void Close() => StartTween(false);
        public void Toggle() => StartTween(!_isOpen);
        public void Set(bool open) => StartTween(open);

        private void StartTween(bool open)
        {
            if (_tweenRoutine != null)
                StopCoroutine(_tweenRoutine);

            _isOpen = open;
            _tweenRoutine = StartCoroutine(TweenDoor(
                open ? _doorOpen : _doorClosed,
                open));
        }

        private IEnumerator TweenDoor(Transform target, bool opening)
        {
            Vector3 startPos = _door.localPosition;
            Quaternion startRot = _door.localRotation;
            Vector3 startScae = _door.localScale;

            Vector3 endPos = target.localPosition;
            Quaternion endRot = target.localRotation;
            Vector3 endScae = target.localScale;

            float elapsed = 0f;

            while (elapsed < _tweenTime)
            {
                float t = elapsed / _tweenTime;
                float eased = _tweenCurve.Evaluate(t);

                _door.localPosition = Vector3.Lerp(startPos, endPos, eased);
                _door.localRotation = Quaternion.Lerp(startRot, endRot, eased);
                _door.localScale = Vector3.Lerp(startScae, endScae, eased);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure final values are set precisely
            _door.localPosition = endPos;
            _door.localRotation = endRot;
            _door.localScale = endScae;

            _tweenRoutine = null;

            if (opening)
                OnFullyOpen?.Invoke();
            else
                OnFullyClosed?.Invoke();
        }
    }
}

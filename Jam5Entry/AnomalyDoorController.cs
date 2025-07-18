using System.Collections;
using UnityEngine;

namespace Jam5Entry
{
    public class AnomalyDoorController : MonoBehaviour
    {
        [Header("Doors")]
        [SerializeField] private Transform _leftDoor;
        [SerializeField] private Transform _rightDoor;

        [Header("States")]
        [SerializeField] private Transform _leftDoorClosed;
        [SerializeField] private Transform _leftDoorOpen;
        [SerializeField] private Transform _rightDoorClosed;
        [SerializeField] private Transform _rightDoorOpen;

        [Header("Tween")]
        [SerializeField] private float _tweenTime = 1f;
        [SerializeField] private AnimationCurve _tweenCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private bool _isOpen = false;
        private Coroutine _tweenRoutine;

        public void Open() => StartTween(true);
        public void Close() => StartTween(false);
        public void Toggle() => StartTween(!_isOpen);
        public void Set(bool open) => StartTween(open);

        private void StartTween(bool open)
        {
            if (_tweenRoutine != null)
                StopCoroutine(_tweenRoutine);

            _isOpen = open;
            _tweenRoutine = StartCoroutine(TweenDoors(
                open ? _leftDoorOpen : _leftDoorClosed,
                open ? _rightDoorOpen : _rightDoorClosed));
        }

        private IEnumerator TweenDoors(Transform leftTarget, Transform rightTarget)
        {
            Vector3 lStartPos = _leftDoor.localPosition;
            Quaternion lStartRot = _leftDoor.localRotation;
            Vector3 lStartScale = _leftDoor.localScale;

            Vector3 rStartPos = _rightDoor.localPosition;
            Quaternion rStartRot = _rightDoor.localRotation;
            Vector3 rStartScale = _rightDoor.localScale;

            Vector3 lEndPos = leftTarget.localPosition;
            Quaternion lEndRot = leftTarget.localRotation;
            Vector3 lEndScale = leftTarget.localScale;

            Vector3 rEndPos = rightTarget.localPosition;
            Quaternion rEndRot = rightTarget.localRotation;
            Vector3 rEndScale = rightTarget.localScale;

            float elapsed = 0f;

            while (elapsed < _tweenTime)
            {
                float t = elapsed / _tweenTime;
                float eased = _tweenCurve.Evaluate(t);

                _leftDoor.localPosition = Vector3.Lerp(lStartPos, lEndPos, eased);
                _leftDoor.localRotation = Quaternion.Slerp(lStartRot, lEndRot, eased);
                _leftDoor.localScale = Vector3.Lerp(lStartScale, lEndScale, eased);

                _rightDoor.localPosition = Vector3.Lerp(rStartPos, rEndPos, eased);
                _rightDoor.localRotation = Quaternion.Slerp(rStartRot, rEndRot, eased);
                _rightDoor.localScale = Vector3.Lerp(rStartScale, rEndScale, eased);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure final values are set precisely
            _leftDoor.localPosition = lEndPos;
            _leftDoor.localRotation = lEndRot;
            _leftDoor.localScale = lEndScale;

            _rightDoor.localPosition = rEndPos;
            _rightDoor.localRotation = rEndRot;
            _rightDoor.localScale = rEndScale;

            _tweenRoutine = null;
        }
    }
}

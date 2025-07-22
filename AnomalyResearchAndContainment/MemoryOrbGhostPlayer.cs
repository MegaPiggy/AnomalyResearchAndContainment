using OWML.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    [RequireComponent(typeof(CapsuleCollider), typeof(CapsuleShape), typeof(Animator))]
    public class MemoryOrbGhostPlayer : Detector
    {
        [SerializeField] private Animator _animator;
        private RuntimeAnimatorController _baseAnimController;

        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private List<MemoryOrbRecorder.ActionFrame> _frames;
        private float _startTime;
        private int _currentIndex;
        private bool _isPlaying;

        private static readonly Detector.Name GhostPlayer = EnumUtils.Create<Detector.Name>("GhostPlayer");

        public override void Awake()
        {
            base.Awake();
            _name = GhostPlayer;
        }

        public void Start()
        {
            _baseAnimController = _animator.runtimeAnimatorController;
        }

        public void Playback(List<MemoryOrbRecorder.ActionFrame> frames)
        {
            _frames = frames;
            _currentIndex = 0;
            _isPlaying = false;

            if (_frames != null && _frames.Count > 0)
            {
                StartCoroutine(MoveToFirstFrame());
            }
        }

        private IEnumerator MoveToFirstFrame()
        {
            spawnPosition = transform.localPosition;
            spawnRotation = transform.localRotation;
            Vector3 startPos = transform.localPosition;
            Quaternion startRot = transform.localRotation;

            Vector3 endPos = _frames[0].localPosition - Vector3.up;
            Quaternion endRot = _frames[0].localRotation;

            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                transform.localPosition = Vector3.Lerp(startPos, endPos, t);
                transform.localRotation = Quaternion.Slerp(startRot, endRot, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = endPos;
            transform.localRotation = endRot;

            _startTime = Time.time;
            _isPlaying = true;
        }

        public void ReturnToSpawn()
        {
            StartCoroutine(ReturnToSpawnAndDestroy());
        }

        private IEnumerator ReturnToSpawnAndDestroy()
        {
            _isPlaying = false;

            Vector3 startPos = transform.localPosition;
            Quaternion startRot = transform.localRotation;
            Vector3 endPos = spawnPosition;
            Quaternion endRot = spawnRotation;

            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                transform.localPosition = Vector3.Lerp(startPos, endPos, t);
                transform.localRotation = Quaternion.Slerp(startRot, endRot, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = endPos;
            transform.localRotation = endRot;

            Destroy(gameObject);
        }


        private void Update()
        {
            if (!OWTime.IsPaused() && !_isPlaying || _frames == null)
                return;

            if (_currentIndex >= _frames.Count)
            {
                ReturnToSpawn();
                return;
            }

            float elapsed = Time.time - _startTime;

            while (_currentIndex < _frames.Count && _frames[_currentIndex].time <= elapsed)
            {
                transform.localPosition = _frames[_currentIndex].localPosition - Vector3.up;
                transform.localRotation = _frames[_currentIndex].localRotation;
                _currentIndex++;
            }
        }

        private void LateUpdate()
        {
            Vector3 vector = Vector3.zero;
            if (Mathf.Abs(vector.x) < 0.05f) vector.x = 0f;
            if (Mathf.Abs(vector.z) < 0.05f) vector.z = 0f;

            _animator.SetFloat("RunSpeedX", vector.x / 3f);
            _animator.SetFloat("RunSpeedY", vector.z / 3f);
            _animator.SetFloat("TurnSpeed", 0);
            _animator.SetBool("Grounded", true);
            _animator.SetLayerWeight(1, 0);
            _animator.SetFloat("FreefallSpeed", 0);
            _animator.SetBool("InZeroG", false);
            _animator.SetBool("UsingJetpack", true);
        }
    }
}

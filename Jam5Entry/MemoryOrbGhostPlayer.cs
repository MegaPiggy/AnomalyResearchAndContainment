using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Jam5Entry
{
    public class MemoryOrbGhostPlayer : MonoBehaviour
    {
        private List<MemoryOrbRecorder.ActionFrame> _frames;
        private float _startTime;
        private int _currentIndex;
        private bool _isPlaying;
        private Animator _animator;
        private RuntimeAnimatorController _baseAnimController;

        private void Awake()
        {
            this._animator = base.GetComponent<Animator>();
            this._baseAnimController = this._animator.runtimeAnimatorController;
        }

        public void Playback(List<MemoryOrbRecorder.ActionFrame> frames)
        {
            _frames = frames;
            _startTime = Time.time;
            _currentIndex = 0;
            _isPlaying = true;
        }

        private void Update()
        {
            if (!_isPlaying || _frames == null || _currentIndex >= _frames.Count)
                return;

            float elapsed = Time.time - _startTime;

            while (_currentIndex < _frames.Count && _frames[_currentIndex].time <= elapsed)
            {
                transform.localPosition = _frames[_currentIndex].localPosition;
                transform.localRotation = _frames[_currentIndex].localRotation;
                _currentIndex++;
            }
        }

        private void LateUpdate()
        {
            Vector3 vector = Vector3.zero;
            if (Mathf.Abs(vector.x) < 0.05f)
            {
                vector.x = 0f;
            }
            if (Mathf.Abs(vector.z) < 0.05f)
            {
                vector.z = 0f;
            }
            this._animator.SetFloat("RunSpeedX", vector.x / 3f);
            this._animator.SetFloat("RunSpeedY", vector.z / 3f);
        }
    }
}

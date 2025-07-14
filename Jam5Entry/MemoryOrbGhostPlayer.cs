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
                transform.position = _frames[_currentIndex].position;
                transform.rotation = _frames[_currentIndex].rotation;
                _currentIndex++;
            }
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Jam5Entry
{
    public class MemoryOrbRecorder : MonoBehaviour
    {
        public class ActionFrame
        {
            public float time;
            public Vector3 position;
            public Quaternion rotation;
        }

        private List<ActionFrame> _recordedFrames = new List<ActionFrame>();
        private float _startTime;
        private bool _isRecording;

        public void StartRecording()
        {
            _recordedFrames.Clear();
            _startTime = Time.time;
            _isRecording = true;
        }

        public void StopRecording()
        {
            _isRecording = false;
        }

        public List<ActionFrame> GetRecording()
        {
            return _recordedFrames;
        }

        private void Update()
        {
            if (_isRecording)
            {
                _recordedFrames.Add(new ActionFrame
                {
                    time = Time.time - _startTime,
                    position = transform.position,
                    rotation = transform.rotation
                });
            }
        }
    }
}

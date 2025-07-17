using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Jam5Entry
{
    public class MemoryOrbRecorder : MonoBehaviour
    {
        [System.Serializable]
        public class ActionFrame
        {
            public float time;
            public Vector3 localPosition;
            public Quaternion localRotation;
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
                var playerTransform = Locator.GetPlayerTransform();
                _recordedFrames.Add(new ActionFrame
                {
                    time = Time.time - _startTime,
                    localPosition = transform.InverseTransformPoint(playerTransform.position),
                    localRotation = transform.InverseTransformRotation(playerTransform.rotation)
                });
            }
        }
    }
}

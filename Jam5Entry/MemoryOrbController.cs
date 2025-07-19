using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Jam5Entry
{
    public class MemoryOrbController : AnomalyController
    {
        [SerializeField] private GameObject _ghostPlayerPrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private MemoryOrbRecorder _recorder;
        [SerializeField] private OWAudioSource _audioSource;
        private AudioType _recordStartSFX => Jam5Entry.TickUp;
        private AudioType _recordStopSFX = AudioType.NonDiaUINegativeSFX;
        private AudioType _playbackStartSFX => Jam5Entry.TickDown;
        [SerializeField] private KeyDropper _keyDropper;
        [SerializeField] private List<MemoryOrbPressurePad> _pads;

        private MemoryOrbGhostPlayer _activeGhost;
        private bool _puzzleComplete;

        private void Start()
        {
            _puzzleComplete = false;
        }

        public void StartRecording()
        {
            if (!IsActive || _puzzleComplete) return;
            if (_activeGhost != null)
            {
                Destroy(_activeGhost.gameObject);
            }
            _recorder.StartRecording();
            _audioSource.PlayOneShot(_recordStartSFX);
        }

        public void StopRecording()
        {
            if (!IsActive || _puzzleComplete) return;
            if (_activeGhost != null)
            {
                Destroy(_activeGhost.gameObject);
            }
            _recorder.StopRecording();
            _audioSource.PlayOneShot(_recordStopSFX);
        }

        public void StartPlayback()
        {
            if (!IsActive || _puzzleComplete) return;

            if (_activeGhost != null)
            {
                Destroy(_activeGhost.gameObject);
            }

            GameObject ghostObj = Instantiate(_ghostPlayerPrefab, _spawnPoint.position, _spawnPoint.rotation, _recorder.transform);
            ghostObj.transform.localPosition = Vector3.zero;
            ghostObj.transform.localEulerAngles = Vector3.zero;
            ghostObj.transform.localScale = Vector3.one * 0.1f;
            _activeGhost = ghostObj.GetAddComponent<MemoryOrbGhostPlayer>();
#if DEBUG
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(ghostObj.transform, false);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localEulerAngles = Vector3.zero;
            cube.transform.localScale = Vector3.one;
            Destroy(cube.GetComponentInChildren<Collider>());
#endif
            _activeGhost.Playback(_recorder.GetRecording());
            _audioSource.PlayOneShot(_playbackStartSFX);

            StartCoroutine(CheckPadsAfterDelay());
        }

        private IEnumerator CheckPadsAfterDelay()
        {
            yield return new WaitForSeconds(2f);

            if (_pads.All(pad => pad.WasTriggered()))
            {
                CompletePuzzle();
            }
        }

        public void CompletePuzzle()
        {
            if (_puzzleComplete) return;

            _puzzleComplete = true;
            if (_keyDropper != null) _keyDropper.DropKey();
            SetActivation(false);
            OpenDoor();
        }

        public override void ActivatePuzzle()
        {
        }

        public override void DeactivatePuzzle()
        {
        }
    }
}

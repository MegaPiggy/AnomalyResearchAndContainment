using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace AnomalyResearchAndContainment
{
    public class MemoryOrbController : AnomalyController
    {
        [SerializeField] private GameObject _ghostPlayerPrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private MemoryOrbConsole _console;
        [SerializeField] private MemoryOrbRecorder _recorder;
        [SerializeField] private OWAudioSource _audioSource;
        private AudioType _recordStartSFX => AnomalyResearchAndContainment.TickUp;
        private AudioType _recordStopSFX = AudioType.NonDiaUINegativeSFX;
        private AudioType _playbackStartSFX => AnomalyResearchAndContainment.TickDown;
        [SerializeField] private List<MemoryOrbPressurePad> _pads;

        private MemoryOrbGhostPlayer _activeGhost;

        public void StartRecording()
        {
            if (!IsActive || Completed) return;

            if (_activeGhost != null)
            {
                _activeGhost.ReturnToSpawn();
                _activeGhost = null;
            }

            _recorder.StartRecording();
            _audioSource.PlayOneShot(_recordStartSFX);
        }

        public void StopRecording()
        {
            if (!IsActive || Completed) return;

            if (_activeGhost != null)
            {
                _activeGhost.ReturnToSpawn();
                _activeGhost = null;
            }

            _recorder.StopRecording();
            _audioSource.PlayOneShot(_recordStopSFX);
        }


        public void StartPlayback()
        {
            if (!IsActive || Completed) return;

            if (_activeGhost != null)
            {
                _activeGhost.ReturnToSpawn();
                _activeGhost = null;
            }

            GameObject ghostObj = Instantiate(_ghostPlayerPrefab, _spawnPoint.position, _spawnPoint.rotation, _recorder.transform);
            ghostObj.transform.position = _spawnPoint.position;
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
            Locator.GetShipLogManager().RevealFact("ARC_MEMORY_ORB_X1", true, true);
        }

        protected override void Update()
        {
            base.Update();

            if (_pads.All(pad => pad.IsTriggered))
            {
                CompletePuzzle();
            }
        }

        public override void CompletePuzzle()
        {
            base.CompletePuzzle();

            Locator.GetShipLogManager().RevealFact("ARC_MEMORY_ORB_X3", true, true);
        }

        public override void ActivatePuzzle()
        {
            _console.ActivateInteraction();
        }

        public override void DeactivatePuzzle()
        {
            _console.DeactivateInteraction();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OWAudioType = global::AudioType;

namespace AnomalyResearchAndContainment
{
    public class EchoCellController : AnomalyController
    {
        public static EchoCellController Instance { get; private set; }

        [System.Serializable]
        public class EchoEvent
        {
            public OWAudioType audioType;
            public float delaySeconds = 1.5f;
            public float pitch = 1f;
        }

        [SerializeField] public OWAudioSource echoSourcePrefab;
        [NonSerialized] public List<int> correctSequence = new List<int> { 3, 2, 0, 1 };
        private float randomDelaySecondsMin = 0.5f;
        private float randomDelaySecondsMax = 1.5f;
        private float randomPitchRange = 0.3f;
        private float randomPosRadius = 2f;

        [SerializeField] private List<Transform> echoPadPositions;
        private float echoLoopDelay = 10f;
        private OWAudioType padEchoAudioType = OWAudioType.NomaiOrbStartDrag;

        private Coroutine _echoLoopCoroutine;
        private bool _playerInteracted;

        private List<OWAudioType> recentSounds = new();

        private List<int> _inputSequence = new List<int>();
        private float _lastInputTime;
        //private float _timeout = 10f;

        public void RegisterPadStep(int index)
        {
            if (!IsActive || Completed) return;

            //if (Time.time - _lastInputTime > _timeout)
            //{
            //    ResetPuzzle();
            //}

            _lastInputTime = Time.time;
            _inputSequence.Add(index);

            if (_inputSequence.Count >= correctSequence.Count)
            {
                if (IsCorrectSequence())
                {
                    CompletePuzzle();
                }
                else
                {
                    ResetPuzzle();
                }
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (correctSequence.Count > 0 && echoPadPositions.Count > 0)
            {
                _echoLoopCoroutine = StartCoroutine(PlayEchoPatternLoop());
            }
        }

        public void RegisterSound(OWAudioType audioType)
        {
            if (!IsActive || audioType == OWAudioType.None) return;

            recentSounds.SafeAdd(audioType);
            EchoEvent echo = new EchoEvent
            {
                audioType = audioType,
                delaySeconds = UnityEngine.Random.Range(randomDelaySecondsMin, randomDelaySecondsMax),
                pitch = 1f + UnityEngine.Random.Range(-randomPitchRange, randomPitchRange)
            };

            StartCoroutine(PlayEchoWithDelay(echo));

            // Stop loop when player interacts
            if (!_playerInteracted)
            {
                _playerInteracted = true;
                if (_echoLoopCoroutine != null) StopCoroutine(_echoLoopCoroutine);
                Invoke(nameof(RestartEchoLoop), echoLoopDelay);
            }
        }

        private IEnumerator PlayEchoWithDelay(EchoEvent echo)
        {
            yield return new WaitForSeconds(echo.delaySeconds);
            PlayEcho(echo);
        }

        private void PlayEcho(EchoEvent echo)
        {
            Vector3 randomPos = UnityEngine.Random.insideUnitSphere * randomPosRadius;
            OWAudioSource echoSource = Instantiate(echoSourcePrefab, randomPos, Quaternion.identity, transform);
            echoSource.transform.localPosition = randomPos;
            echoSource.pitch = echo.pitch;
            echoSource.spatialBlend = 1f;
            AudioClip clip = echoSource.PlayOneShot(echo.audioType);
            Destroy(echoSource.gameObject, clip.length / echo.pitch + 1f);
            Locator.GetShipLogManager().RevealFact("ARC_ECHO_CELL_X1", true, true);
        }

        private IEnumerator PlayEchoPatternLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                if (IsActive)
                {
                    foreach (int padIndex in correctSequence)
                    {
                        if (padIndex >= 0 && padIndex < echoPadPositions.Count)
                        {
                            Vector3 pos = echoPadPositions[padIndex].position;
                            OWAudioSource echoSource = Instantiate(echoSourcePrefab, pos, Quaternion.identity, transform);
                            echoSource.pitch = 1f;
                            echoSource.spatialBlend = 1f;
                            echoSource.SetLocalVolume(10);
                            echoSource.SetMaxVolume(10);
                            AudioClip clip = echoSource.PlayOneShot(padEchoAudioType);
                            Destroy(echoSource.gameObject, clip.length + 1f);
                            yield return new WaitForSeconds(1.2f);
                        }
                    }
                }

                yield return new WaitForSeconds(echoLoopDelay);
            }
        }

        private void RestartEchoLoop()
        {
            _playerInteracted = false;
            _echoLoopCoroutine = StartCoroutine(PlayEchoPatternLoop());
        }

        private bool IsCorrectSequence()
        {
            for (int i = 0; i < correctSequence.Count; i++)
            {
                if (correctSequence[i] != _inputSequence[i])
                    return false;
            }
            return true;
        }

        public override void CompletePuzzle()
        {
            base.CompletePuzzle();

            Locator.GetShipLogManager().RevealFact("ARC_ECHO_CELL_X2", true, true);
        }

        public override void ResetPuzzle()
        {
            base.ResetPuzzle();

            _inputSequence.Clear();
        }

        public override void ActivatePuzzle()
        {
        }

        public override void DeactivatePuzzle()
        {
        }
    }
}

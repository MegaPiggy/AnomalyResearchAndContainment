using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OWAudioType = global::AudioType;

namespace Jam5Entry
{
    public class EchoCellController : MonoBehaviour
    {
        [System.Serializable]
        public class EchoEvent
        {
            public OWAudioType audioType;
            public float delaySeconds = 1.5f;
            public float pitch = 1f;
        }

        [SerializeField] public OWAudioSource echoSourcePrefab;
        [SerializeField] private float randomDelaySecondsMin = 0.5f;
        [SerializeField] private float randomDelaySecondsMax = 1.5f;
        [SerializeField] private float randomPitchRange = 0.3f;
        [SerializeField] private float randomPosRadius = 2f;

        [SerializeField] private List<Transform> echoPadPositions;
        [SerializeField] private List<int> echoPattern = new(); // Indexes into echoPadPositions
        [SerializeField] private float echoLoopDelay = 10f;
        private OWAudioType padEchoAudioType = OWAudioType.NonDiaUIAffirmativeSFX;

        private Coroutine _echoLoopCoroutine;
        private bool _playerInteracted;

        private List<OWAudioType> recentSounds = new();

        private void Start()
        {
            if (echoPattern.Count > 0 && echoPadPositions.Count > 0)
            {
                _echoLoopCoroutine = StartCoroutine(PlayEchoPatternLoop());
            }
        }

        public void RegisterSound(OWAudioType audioType)
        {
            if (audioType == OWAudioType.None) return;

            recentSounds.SafeAdd(audioType);
            EchoEvent echo = new EchoEvent
            {
                audioType = OWAudioType.NonDiaUINegativeSFX,
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
        }

        private IEnumerator PlayEchoPatternLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                foreach (int padIndex in echoPattern)
                {
                    if (padIndex >= 0 && padIndex < echoPadPositions.Count)
                    {
                        Vector3 pos = echoPadPositions[padIndex].position;
                        OWAudioSource echoSource = Instantiate(echoSourcePrefab, pos, Quaternion.identity, transform);
                        echoSource.pitch = 1f;
                        echoSource.spatialBlend = 1f;
                        AudioClip clip = echoSource.PlayOneShot(padEchoAudioType);
                        Destroy(echoSource.gameObject, clip.length + 1f);
                        yield return new WaitForSeconds(1.2f);
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
    }
}

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

        private List<OWAudioType> recentSounds = new List<OWAudioType>();

        public void RegisterSound(OWAudioType audioType)
        {
            if (audioType == OWAudioType.None) return;

            recentSounds.SafeAdd(audioType);
            EchoEvent echo = new EchoEvent
            {
                audioType = audioType,
                delaySeconds = UnityEngine.Random.Range(randomDelaySecondsMin, randomDelaySecondsMax),
                pitch = 1f + UnityEngine.Random.Range(-randomPitchRange, randomPitchRange)
            };

            StartCoroutine(PlayEchoWithDelay(echo));
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public class Indicator : MonoBehaviour
    {
        public enum IndicatorState
        {
            Off,
            Success,
            Fail
        }

        public IndicatorState State => _indicatorState;

        [SerializeField] private OWAudioSource _audioSource;
        [SerializeField] private Renderer _indicatorOff;
        [SerializeField] private Renderer _indicatorSuccess;
        [SerializeField] private Renderer _indicatorFail;
        private AudioType _successAudio = AudioType.NonDiaUIAffirmativeSFX;
        private AudioType _failAudio = AudioType.NonDiaUINegativeSFX;
        private IndicatorState _indicatorState = IndicatorState.Off;
        private float _flashDuration = 0.5f;
        private Coroutine _flashRoutine;

        public void Start()
        {
            SetIndicatorState(IndicatorState.Off);
        }

        public void PlaySuccessFeedback()
        {
            if (_audioSource != null) _audioSource.PlayOneShot(_successAudio);
            FlashIndicators(IndicatorState.Success);
        }

        public void PlayFailFeedback()
        {
            if (_audioSource != null) _audioSource.PlayOneShot(_failAudio);
            FlashIndicators(IndicatorState.Fail);
        }

        private void FlashIndicators(IndicatorState flashState)
        {
            if (_flashRoutine != null)
                StopCoroutine(_flashRoutine);

            _flashRoutine = StartCoroutine(Flash(flashState));
        }

        private System.Collections.IEnumerator Flash(IndicatorState flashState)
        {
            SetIndicatorState(flashState);
            yield return new WaitForSeconds(_flashDuration);
            SetIndicatorState(IndicatorState.Off);
        }

        public void SetIndicatorState(IndicatorState state)
        {
            _indicatorState = state;
            _indicatorOff.gameObject.SetActive(state == IndicatorState.Off);
            _indicatorSuccess.gameObject.SetActive(state == IndicatorState.Success);
            _indicatorFail.gameObject.SetActive(state == IndicatorState.Fail);
        }
    }
}

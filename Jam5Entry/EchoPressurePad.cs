using UnityEngine;

namespace Jam5Entry
{
    public class EchoPressurePad : MonoBehaviour
    {
        [SerializeField] private int _padIndex;
        [SerializeField] private EchoConsole _console;
        [SerializeField] private OWTriggerVolume _triggerVolume;
        [SerializeField] private OWAudioSource _audioSource;
        [SerializeField] private AudioType _tone;

        private bool _wasTriggered = false;

        private void Awake()
        {
            _triggerVolume.OnEntry += OnStep;
        }

        private void OnDestroy()
        {
            _triggerVolume.OnEntry -= OnStep;
        }

        private void OnStep(GameObject obj)
        {
            if (_wasTriggered || !obj.CompareTag("PlayerDetector")) return;

            _wasTriggered = true;
            _audioSource?.PlayOneShot(_tone);
            _console?.RegisterPadStep(_padIndex);
            Invoke(nameof(ResetTrigger), 0.5f); // prevent multiple triggers
        }

        private void ResetTrigger()
        {
            _wasTriggered = false;
        }
    }
}
